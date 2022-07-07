import { CanvasTypes, isHTMLCanvas } from "@juniper-lib/dom/canvas";
import {
    border,
    height,
    left,
    margin,
    padding,
    position,
    rule,
    top,
    touchAction,
    width
} from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";
import { AssetCustom, BaseAsset, IFetcher } from "@juniper-lib/fetcher";
import { MediaType, Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import {
    arrayRemove, arraySortByKeyInPlace, IProgress, isDefined,
    isDesktop,
    isFirefox, isFunction, isOculusBrowser, oculusBrowserVersion, TimerTickEvent, TypedEvent, TypedEventBase
} from "@juniper-lib/tslib";
import { feet2Meters } from "@juniper-lib/tslib/units/length";
import { BodyFollower } from "../animation/BodyFollower";
import { updateScalings } from "../animation/scaleOnHover";
import { AvatarLocal } from "../AvatarLocal";
import { cleanup } from "../cleanup";
import { Cursor3D } from "../eventSystem/Cursor3D";
import { PointerManager } from "../eventSystem/PointerManager";
import { GLTFLoader } from "../examples/loaders/GLTFLoader";
import { Fader } from "../Fader";
import { IModelLoader } from "../IModelLoader";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { FOREGROUND, PURGATORY } from "../layers";
import { LoadingBar } from "../LoadingBar";
import { convertMaterials, materialStandardToBasic } from "../materials";
import { obj, objGraph } from "../objects";
import { resolveCamera } from "../resolveCamera";
import { ScreenControl } from "../ScreenControl";
import { Skybox } from "../Skybox";
import { XRTimer, XRTimerTickEvent } from "./XRTimer";


const spectator = new THREE.PerspectiveCamera();
const lastViewport = new THREE.Vector4();
const curViewport = new THREE.Vector4();

const gridWidth = 15;
const gridSize = feet2Meters(gridWidth);

interface BaseEnvironmentEvents {
    sceneclearing: TypedEvent<"sceneclearing">;
    scenecleared: TypedEvent<"scenecleared">;
    quitting: TypedEvent<"quitting">;
    newcursorloaded: TypedEvent<"newcursorloaded">;
}

Style(
    rule("#frontBuffer",
        position("absolute"),
        left(0),
        top(0),
        width("100%"),
        height("100%"),
        margin(0),
        padding(0),
        border(0),
        touchAction("none")
    )
);

export class BaseEnvironment<Events = unknown>
    extends TypedEventBase<Events & BaseEnvironmentEvents>
    implements IWebXRLayerManager, IModelLoader {

    private baseLayer: XRWebGLLayer | XRProjectionLayer;
    private readonly layers = new Array<XRLayer>();
    private readonly layerSortOrder = new Map<XRLayer, number>();

    private readonly fader: Fader;
    private fadeDepth = 0;

    readonly cursor3D: Cursor3D;
    readonly camera = new THREE.PerspectiveCamera(50, 1, 0.01, 1000);
    readonly scene = new THREE.Scene();
    readonly stage = obj("Stage");
    readonly ambient = new THREE.AmbientLight(0xffffff, 0.5);
    readonly sun = new THREE.DirectionalLight(0xffffff, 0.75);
    readonly ground = new THREE.GridHelper(gridSize, gridWidth, 0xc0c0c0, 0x808080);
    readonly foreground = obj("Foreground");
    readonly loadingBar = new LoadingBar();

    readonly renderer: THREE.WebGLRenderer;
    readonly timer: XRTimer;
    readonly worldUISpace: BodyFollower;
    readonly skybox: Skybox;
    readonly avatar: AvatarLocal;
    readonly screenControl: ScreenControl;
    readonly pointers: PointerManager;

    enableSpectator = false;

    constructor(canvas: CanvasTypes, public readonly fetcher: IFetcher, public readonly defaultAvatarHeight: number, enableFullResolution: boolean, public DEBUG: boolean) {
        super();

        this.cursor3D = new Cursor3D(this);

        if (isHTMLCanvas(canvas)) {
            canvas.style.backgroundColor = "black";
        }

        this.renderer = new THREE.WebGLRenderer({
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

        this.renderer.domElement.tabIndex = 1;

        this.screenControl = new ScreenControl(
            this.renderer,
            this.camera,
            this.renderer.domElement.parentElement,
            enableFullResolution);


        this.fader = new Fader("ViewFader");

        this.worldUISpace = new BodyFollower("WorldUISpace", 0.2, 20, 0.125);

        this.avatar = new AvatarLocal(
            this,
            this.fader,
            defaultAvatarHeight);


        this.pointers = new PointerManager(this);

        this.skybox = new Skybox(this);

        this.timer = new XRTimer(this.renderer);

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
                ...this.pointers.hands
            ),
            this.foreground,
            objGraph(this.worldUISpace,
                this.loadingBar
            )
        );

        this.timer.addTickHandler((evt) => this.update(evt));
        this.timer.start();

        (globalThis as any).env = this;
    }

    get gl(): WebGLRenderingContext {
        return this.renderer.getContext();
    }

    get referenceSpace(): XRReferenceSpace {
        return this.renderer.xr.getReferenceSpace();
    }

    private update(evt: XRTimerTickEvent): void {
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
            this.pointers.update();
            this.avatar.update(evt.dt);
            this.worldUISpace.update(this.avatar.height, this.avatar.worldPos, this.avatar.worldHeading, evt.dt);
            this.fader.update(evt.dt);
            updateScalings(evt.dt);
            this.loadingBar.update(evt.sdt);

            this.preRender(evt);

            this.skybox.update(evt.frame);

            const cam = resolveCamera(this.renderer, this.camera);
            if (cam !== this.camera) {
                const vrCam = cam as THREE.WebXRArrayCamera;
                vrCam.layers.mask = this.camera.layers.mask;
                for (let i = 0; i < vrCam.cameras.length; ++i) {
                    const subCam = vrCam.cameras[i];
                    subCam.layers.mask = this.camera.layers.mask;
                    subCam.layers.enable(i + 1);
                    vrCam.layers.enable(i + 1);
                }
            }

            this.renderer.clear();
            this.renderer.render(this.scene, this.camera);
            if (this.enableSpectator) {
                if (!this.renderer.xr.isPresenting) {
                    lastViewport.copy(curViewport);
                    this.renderer.getViewport(curViewport);
                }
                else if (isDesktop()
                    && !isFirefox()) {
                    spectator.projectionMatrix.copy(this.camera.projectionMatrix);
                    spectator.position.copy(cam.position);
                    spectator.quaternion.copy(cam.quaternion);
                    const curRT = this.renderer.getRenderTarget();
                    this.renderer.xr.isPresenting = false;
                    this.renderer.setRenderTarget(null);
                    this.renderer.setViewport(lastViewport);
                    this.renderer.clear();
                    this.renderer.render(this.scene, spectator);
                    this.renderer.setViewport(curViewport);
                    this.renderer.setRenderTarget(curRT);
                    this.renderer.xr.isPresenting = true;
                }
            }
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
        --this.fadeDepth;
        if (this.fadeDepth === 0) {
            await this.fader.fadeOut();
            this.camera.layers.set(FOREGROUND);
            this.skybox.visible = true;
            await this.fader.fadeIn();
        }
    }

    modelAsset(path: string): AssetCustom<THREE.Group> {
        return new AssetCustom(path, Model_Gltf_Binary, (fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) => this.getModel(fetcher, path, type, prog));
    }

    private getModel(fetcher: IFetcher, path: string, type: string | MediaType, prog?: IProgress) {
        return fetcher
            .get(path)
            .useCache(!this.DEBUG)
            .progress(prog)
            .file(type)
            .then(response => this.loadModel(response.content));
    }

    async loadModel(path: string, prog?: IProgress): Promise<THREE.Group> {
        const loader = new GLTFLoader();
        const model = await loader.loadAsync(path, (evt) => {
            if (isDefined(prog)) {
                prog.report(evt.loaded, evt.total, path);
            }
        });

        return model.scene;
    }

    private set3DCursor(model: THREE.Group): void {
        const children = model.children.slice(0);
        for (const child of children) {
            this.cursor3D.add(child.name, child);
        }
        this.pointers.refreshCursors();
        this.dispatchEvent(new TypedEvent("newcursorloaded"));
    }

    async load(prog: IProgress, ...assets: BaseAsset[]): Promise<void>;
    async load(...assets: BaseAsset[]): Promise<void>;
    async load(progOrAsset: IProgress | BaseAsset, ...assets: BaseAsset[]): Promise<void> {
        let prog: IProgress = null;
        if (progOrAsset instanceof BaseAsset) {
            assets.push(progOrAsset);
        }
        else {
            prog = progOrAsset
        }

        const cursor3d = this.modelAsset("/models/Cursors.glb");
        assets.push(cursor3d);

        await this.fetcher.assets(prog, ...assets);

        convertMaterials(cursor3d.result, materialStandardToBasic);

        this.set3DCursor(cursor3d.result);
    }
}
