import { CanvasTypes, isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { BaseAsset, isAsset } from "@juniper-lib/fetcher/Asset";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { arrayRemove, arraySortByKeyInPlace } from "@juniper-lib/collections/arrays";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/TypedEventBase";
import { isDesktop, isFirefox, isOculusBrowser, oculusBrowserVersion } from "@juniper-lib/tslib/flags";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { TimerTickEvent } from "@juniper-lib/timers/ITimer";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { feet2Meters } from "@juniper-lib/tslib/units/length";
import { AmbientLight, Color, ColorManagement, DirectionalLight, GridHelper, Group, LinearEncoding, PerspectiveCamera, Scene, Vector4, WebGLRenderTarget, WebGLRenderer, WebXRArrayCamera, sRGBEncoding } from "three";
import { AssetGltfModel } from "../../AssetGltfModel";
import { AvatarLocal } from "../../AvatarLocal";
import { Fader } from "../../Fader";
import { LoadingBar } from "../../LoadingBar";
import { ScreenControl } from "../../ScreenControl";
import { Skybox } from "../../Skybox";
import { BodyFollower } from "../../animation/BodyFollower";
import { updateScalings } from "../../animation/scaleOnHover";
import { cleanup } from "../../cleanup";
import { EventSystem } from "../../eventSystem/EventSystem";
import { Cursor3D } from "../../eventSystem/cursors/Cursor3D";
import { GLTF, GLTFLoader } from "../../examples/loaders/GLTFLoader";
import { XRHandModelFactory } from "../../examples/webxr/XRHandModelFactory";
import { FOREGROUND, PURGATORY } from "../../layers";
import { convertMaterials, materialStandardToBasic } from "../../materials";
import { obj, objGraph } from "../../objects";
import { resolveCamera } from "../../resolveCamera";
import { XRTimer, XRTimerTickEvent } from "../XRTimer";

import "./style.css";

const gridWidth = 15;
const gridSize = feet2Meters(gridWidth);

type BaseEnvironmentEvents = {
    sceneclearing: TypedEvent<"sceneclearing">;
    scenecleared: TypedEvent<"scenecleared">;
    quitting: TypedEvent<"quitting">;
    newcursorloaded: TypedEvent<"newcursorloaded">;
    update: XRTimerTickEvent;
}

export class BaseEnvironment<Events = unknown>
    extends TypedEventBase<Events & BaseEnvironmentEvents> {

    private baseLayer: XRWebGLLayer | XRProjectionLayer;
    private readonly layers = new Array<XRLayer>();
    private readonly layerSortOrder = new Map<XRLayer, number>();
    private readonly spectator = new PerspectiveCamera();
    private readonly lastViewport = new Vector4();
    private readonly curViewport = new Vector4();
    private readonly gltfLoader = new GLTFLoader();

    protected readonly fader: Fader;

    private fadeDepth = 0;

    readonly defaultAvatarHeight: number;
    readonly scene = new Scene();
    readonly stage = obj("Stage");
    readonly ambient = new AmbientLight(0xffffff, 0.5);
    readonly sun = new DirectionalLight(0xffffff, 0.75);
    readonly ground = new GridHelper(gridSize, gridWidth, 0xc0c0c0, 0x808080);
    readonly foreground = obj("Foreground");
    readonly loadingBar = new LoadingBar();
    readonly handModelFactory = new XRHandModelFactory(new Color(0xb7c8e9), "mesh");

    DEBUG: boolean;
    readonly camera: PerspectiveCamera;
    readonly renderer: WebGLRenderer;
    readonly timer: XRTimer;
    readonly worldUISpace: BodyFollower;
    readonly skybox: Skybox;
    readonly avatar: AvatarLocal;
    readonly cursor3D: Cursor3D;
    readonly screenControl: ScreenControl = null;
    readonly eventSys: EventSystem;

    enableSpectator = false;

    constructor(canvas: CanvasTypes,
        private readonly styleSheetPath: string,
        public readonly fetcher: IFetcher,
        enableFullResolution: boolean,
        enableAnaglyph: boolean,
        DEBUG: boolean = null,
        defaultAvatarHeight: number = null,
        defaultFOV: number = null) {
        super();

        this.DEBUG = DEBUG || false;
        this.defaultAvatarHeight = defaultAvatarHeight || 1.75;
        defaultFOV = defaultFOV || 60;

        this.camera = new PerspectiveCamera(defaultFOV, 1, 0.01, 1000);
        this.cursor3D = new Cursor3D(this);

        if (isHTMLCanvas(canvas)) {
            canvas.style.backgroundColor = "black";
            if (isNullOrUndefined(canvas.parentElement)) {
                throw new Error("The provided canvas must be included in a parent element before constructing the environment.");
            }
        }

        this.renderer = new WebGLRenderer({
            canvas,
            powerPreference: "high-performance",
            precision: "lowp",
            antialias: true,
            alpha: true,
            premultipliedAlpha: true,
            depth: true,
            logarithmicDepthBuffer: true,
            stencil: false,
            preserveDrawingBuffer: false
        });

        this.renderer.domElement.setAttribute("touch-action", "none");
        this.renderer.domElement.tabIndex = 1;

        this.useNewColorModel = false;

        if (isHTMLCanvas(canvas)) {
            this.screenControl = new ScreenControl(
                this.renderer,
                this.camera,
                this.renderer.domElement.parentElement,
                enableFullResolution,
                enableAnaglyph);
        }


        this.fader = new Fader("ViewFader");

        this.worldUISpace = new BodyFollower("WorldUISpace", 0.2, 20, 0.125);

        this.avatar = new AvatarLocal(this, this.fader);

        this.eventSys = new EventSystem(this);

        this.timer = new XRTimer(this.renderer);

        this.skybox = new Skybox(this);

        this.renderer.xr.enabled = true;
        this.sun.name = "Sun";
        this.sun.position.set(0, 1, 1);
        this.sun.lookAt(0, 0, 0);
        this.sun.layers.enableAll();

        const showGround = () => {
            this.ground.visible = this.renderer.xr.isPresenting;
        };
        this.screenControl.addEventListener("sessionstarted", showGround);
        this.screenControl.addEventListener("sessionstopped", showGround);
        showGround();

        this.ambient.name = "Fill";
        this.ambient.layers.enableAll();

        this.loadingBar.object.name = "MainLoadingBar";
        this.loadingBar.object.position.set(0, -0.25, -2);

        this.scene.layers.enableAll();

        this.avatar.addFollower(this.worldUISpace);

        objGraph(this.scene,
            this.sun,
            this.ambient,
            objGraph(this.stage,
                this.ground,
                this.camera,
                this.avatar,
                ...this.eventSys.hands
            ),
            this.foreground,
            objGraph(this.worldUISpace,
                this.loadingBar
            )
        );


        this.timer.addTickHandler((evt) => this.update(evt));

        this._start();

        (globalThis as any).env = this;
    }

    get useNewColorModel(): boolean {
        return ColorManagement.enabled;
    }

    set useNewColorModel(enabled: boolean) {
        ColorManagement.enabled = enabled;

        // THREE v0.150
        this.renderer.outputEncoding = enabled
            ? sRGBEncoding
            : LinearEncoding;

        // THREE v0.151+
        //this.renderer.outputColorSpace = enabled
        //    ? SRGBColorSpace
        //    : LinearSRGBColorSpace;
    }

    private async _start() {
        if (isDefined(this.styleSheetPath)) {
            await this.fetcher.get(this.styleSheetPath).style();
        }
        this.timer.start();
    }

    get gl(): WebGLRenderingContext | WebGL2RenderingContext {
        return this.renderer.getContext();
    }

    get referenceSpace(): XRReferenceSpace {
        return this.renderer.xr.getReferenceSpace();
    }

    private update(evt: XRTimerTickEvent): void {
        this.dispatchEvent(evt);
        if (this.screenControl.visible) {
            const session = this.xrSession;

            this._xrBinding = this.renderer.xr.getBinding();

            if (this.hasXRMediaLayers && (this._xrMediaBinding === null) === this.renderer.xr.isPresenting) {
                if (this._xrMediaBinding === null && isDefined(session)) {
                    this._xrMediaBinding = new XRMediaBinding(session);
                }
                else {
                    this._xrMediaBinding = null;
                }
            }

            const baseLayer = session && this.renderer.xr.getBaseLayer();
            if (baseLayer !== this.baseLayer) {
                if (isDefined(this.baseLayer)) {
                    this.removeWebXRLayer(this.baseLayer);
                    this.baseLayer = null;
                }

                if (isDefined(baseLayer)) {
                    this.baseLayer = baseLayer;
                    this.addWebXRLayer(baseLayer, 0);
                }
            }

            this.screenControl.resize();
            this.eventSys.update();
            this.avatar.update(evt.dt);
            this.worldUISpace.update(this.avatar.height, this.avatar.worldPos, this.avatar.worldHeadingRadians, evt.dt);
            this.fader.update(evt.dt);
            updateScalings(evt.dt);
            this.loadingBar.update(evt.sdt);

            this.preRender(evt);

            const cam = resolveCamera(this.renderer, this.camera);
            if (cam !== this.camera) {
                const vrCam = cam as WebXRArrayCamera;
                vrCam.layers.mask = this.camera.layers.mask;
                for (let i = 0; i < vrCam.cameras.length; ++i) {
                    const subCam = vrCam.cameras[i];
                    subCam.layers.mask = this.camera.layers.mask;
                    subCam.layers.enable(i + 1);
                    vrCam.layers.enable(i + 1);
                }
            }

            this.screenControl.render(this.scene, this.camera);

            if (this.enableSpectator) {
                if (!this.renderer.xr.isPresenting) {
                    this.lastViewport.copy(this.curViewport);
                    this.renderer.getViewport(this.curViewport);
                }
                else if (isDesktop()
                    && !isFirefox()) {
                    this.drawSnapshot();
                }
            }
        }
    }

    drawSnapshot() {
        const isPresenting = this.renderer.xr.isPresenting;
        let curRT: WebGLRenderTarget = null;
        if (isPresenting) {
            const cam = resolveCamera(this.renderer, this.camera);
            this.spectator.projectionMatrix.copy(this.camera.projectionMatrix);
            this.spectator.position.copy(cam.position);
            this.spectator.quaternion.copy(cam.quaternion);
            curRT = this.renderer.getRenderTarget();
            this.renderer.xr.isPresenting = false;
            this.renderer.setRenderTarget(null);
            this.renderer.setViewport(this.lastViewport);
        }

        this.screenControl.render(this.scene, isPresenting ? this.spectator : this.camera);

        if (isPresenting) {
            this.renderer.setViewport(this.curViewport);
            this.renderer.setRenderTarget(curRT);
            this.renderer.xr.isPresenting = true;
        }
    }

    protected preRender(_evt: TimerTickEvent) {
    }

    protected async onQuitting(): Promise<void> {
        this.dispatchEvent(new TypedEvent("quitting"));
        window.location.href = "/";
    }

    get hasAlpha() {
        return this.renderer.getContextAttributes().alpha;
    }

    get xrSession(): XRSession {
        return this.renderer.xr.getSession();
    }

    private _xrBinding: XRWebGLBinding = null;
    get xrBinding(): XRWebGLBinding {
        return this._xrBinding;
    }

    private _xrMediaBinding: XRMediaBinding = null;
    get xrMediaBinding(): XRMediaBinding {
        return this._xrMediaBinding;
    }

    private get isReadyForLayers(): boolean {
        return this.hasAlpha
            && (!isOculusBrowser
                || oculusBrowserVersion.major >= 15);
    }

    private _hasXRMediaLayers: boolean = null;
    get hasXRMediaLayers(): boolean {
        if (this._hasXRMediaLayers === null) {
            this._hasXRMediaLayers = this.isReadyForLayers
                && "XRMediaBinding" in globalThis
                && isFunction(XRMediaBinding.prototype.createQuadLayer);
        }

        return this._hasXRMediaLayers;
    }

    private _hasXRCompositionLayers: boolean = null;
    get hasXRCompositionLayers(): boolean {
        if (this._hasXRCompositionLayers === null) {
            this._hasXRCompositionLayers = this.isReadyForLayers
                && "XRWebGLBinding" in globalThis
                && isFunction(XRWebGLBinding.prototype.createCubeLayer);
        }

        return this._hasXRCompositionLayers;
    }

    addWebXRLayer(layer: XRLayer, sortOrder: number) {
        this.layers.push(layer);
        this.layerSortOrder.set(layer, sortOrder);
        arraySortByKeyInPlace(this.layers, (l) => -this.layerSortOrder.get(l));
        this.updateLayers();
    }

    removeWebXRLayer(layer: XRLayer) {
        this.layerSortOrder.delete(layer);
        arrayRemove(this.layers, layer);
        this.updateLayers();
    }

    private updateLayers() {
        const session = this.xrSession;
        if (isDefined(session)) {
            session.updateRenderState({
                layers: this.layers
            });
        }
    }

    clearScene() {
        this.dispatchEvent(new TypedEvent("sceneclearing"));
        cleanup(this.foreground);
        this.dispatchEvent(new TypedEvent("scenecleared"));
    }

    async fadeOut() {
        ++this.fadeDepth;
        if (this.fadeDepth === 1) {
            await this.fader.fadeOut();
            this.skybox.visible = false;
            this.camera.layers.set(PURGATORY);
            this.loadingBar.start();
            await this.fader.fadeIn();
        }
    }

    async fadeIn() {
        if (this.fadeDepth === 1) {
            await this.fader.fadeOut();
            this.camera.layers.set(FOREGROUND);
            this.skybox.visible = true;
            await this.fader.fadeIn();
        }
        --this.fadeDepth;
    }

    async withFade<T>(action: () => Promise<T>) {
        try {
            await this.fadeOut();
            return await action();
        }
        finally {
            await this.fadeIn();
        }
    }

    get showWebXRLayers() {
        return this.fadeDepth === 0;
    }

    private set3DCursor(model: Group): void {
        const children = model.children.slice(0);
        for (const child of children) {
            this.cursor3D.add(child.name, child);
        }
        this.eventSys.refreshCursors();
        this.dispatchEvent(new TypedEvent("newcursorloaded"));
    }

    async load(prog: IProgress, ...assets: BaseAsset[]): Promise<void>;
    async load(...assets: BaseAsset[]): Promise<void>;
    async load(progOrAsset: IProgress | BaseAsset, ...assets: BaseAsset[]): Promise<void> {
        let prog: IProgress = null;
        if (isAsset(progOrAsset)) {
            assets.push(progOrAsset);
        }
        else {
            prog = progOrAsset;
        }

        const cursor3d = new AssetGltfModel(this, "/models/Cursors.glb", Model_Gltf_Binary, !this.DEBUG);
        assets.push(cursor3d);

        await this.fetcher.assets(prog, ...assets);

        convertMaterials(cursor3d.result.scene, materialStandardToBasic);

        this.set3DCursor(cursor3d.result.scene);
    }

    loadGltf(file: string): Promise<GLTF> {
        return this.gltfLoader.loadAsync(file);
    }
}
