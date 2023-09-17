import { compareBy, insertSorted, removeSorted } from "@juniper-lib/collections/dist/arrays";
import { isHTMLCanvas } from "@juniper-lib/dom/dist/canvas";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isAsset } from "@juniper-lib/fetcher/dist/Asset";
import { Model_Gltf_Binary } from "@juniper-lib/mediatypes";
import { isDesktop, isFirefox, isOculusBrowser, oculusBrowserVersion } from "@juniper-lib/tslib/dist/flags";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import { feet2Meters } from "@juniper-lib/tslib/dist/units/length";
import { AmbientLight, Color, ColorManagement, DirectionalLight, GridHelper, LinearEncoding, PerspectiveCamera, Scene, Vector4, WebGLRenderer, sRGBEncoding } from "three";
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
import { GLTFLoader } from "../../examples/loaders/GLTFLoader";
import { XRHandModelFactory } from "../../examples/webxr/XRHandModelFactory";
import { FOREGROUND, PURGATORY } from "../../layers";
import { convertMaterials, materialStandardToBasic } from "../../materials";
import { obj, objGraph } from "../../objects";
import { resolveCamera } from "../../resolveCamera";
import { XRTimer } from "../XRTimer";
import "./style.css";
const gridWidth = 15;
const gridSize = feet2Meters(gridWidth);
export class BaseEnvironment extends TypedEventTarget {
    constructor(canvas, styleSheetPath, fetcher, enableFullResolution, enableAnaglyph, DEBUG = null, defaultAvatarHeight = null, defaultFOV = null) {
        super();
        this.styleSheetPath = styleSheetPath;
        this.fetcher = fetcher;
        this.layers = new Array();
        this.layerSortOrder = new Map();
        this.spectator = new PerspectiveCamera();
        this.lastViewport = new Vector4();
        this.curViewport = new Vector4();
        this.gltfLoader = new GLTFLoader();
        this.fadeDepth = 0;
        this.scene = new Scene();
        this.stage = obj("Stage");
        this.ambient = new AmbientLight(0xffffff, 0.5);
        this.sun = new DirectionalLight(0xffffff, 0.75);
        this.ground = new GridHelper(gridSize, gridWidth, 0xc0c0c0, 0x808080);
        this.foreground = obj("Foreground");
        this.loadingBar = new LoadingBar();
        this.handModelFactory = new XRHandModelFactory(new Color(0xb7c8e9), "mesh");
        this.screenControl = null;
        this.enableSpectator = false;
        this._xrBinding = null;
        this._xrMediaBinding = null;
        this._hasXRMediaLayers = null;
        this._hasXRCompositionLayers = null;
        this.layerSorter = compareBy("descending", l => this.layerSortOrder.get(l));
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
            this.screenControl = new ScreenControl(this.renderer, this.camera, this.renderer.domElement.parentElement, enableFullResolution, enableAnaglyph);
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
        objGraph(this.scene, this.sun, this.ambient, objGraph(this.stage, this.ground, this.camera, this.avatar, ...this.eventSys.hands), this.foreground, objGraph(this.worldUISpace, this.loadingBar));
        this.timer.addTickHandler((evt) => this.update(evt));
        this._start();
        globalThis.env = this;
    }
    get useNewColorModel() {
        return ColorManagement.enabled;
    }
    set useNewColorModel(enabled) {
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
    async _start() {
        if (isDefined(this.styleSheetPath)) {
            await this.fetcher.get(this.styleSheetPath).style();
        }
        this.timer.start();
    }
    get gl() {
        return this.renderer.getContext();
    }
    get referenceSpace() {
        return this.renderer.xr.getReferenceSpace();
    }
    update(evt) {
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
                const vrCam = cam;
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
        let curRT = null;
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
    preRender(_evt) {
    }
    async onQuitting() {
        this.dispatchEvent(new TypedEvent("quitting"));
        window.location.href = "/";
    }
    get hasAlpha() {
        return this.renderer.getContextAttributes().alpha;
    }
    get xrSession() {
        return this.renderer.xr.getSession();
    }
    get xrBinding() {
        return this._xrBinding;
    }
    get xrMediaBinding() {
        return this._xrMediaBinding;
    }
    get isReadyForLayers() {
        return this.hasAlpha
            && (!isOculusBrowser
                || oculusBrowserVersion.major >= 15);
    }
    get hasXRMediaLayers() {
        if (this._hasXRMediaLayers === null) {
            this._hasXRMediaLayers = this.isReadyForLayers
                && "XRMediaBinding" in globalThis
                && isFunction(XRMediaBinding.prototype.createQuadLayer);
        }
        return this._hasXRMediaLayers;
    }
    get hasXRCompositionLayers() {
        if (this._hasXRCompositionLayers === null) {
            this._hasXRCompositionLayers = this.isReadyForLayers
                && "XRWebGLBinding" in globalThis
                && isFunction(XRWebGLBinding.prototype.createCubeLayer);
        }
        return this._hasXRCompositionLayers;
    }
    addWebXRLayer(layer, sortOrder) {
        this.layerSortOrder.set(layer, sortOrder);
        insertSorted(this.layers, layer, this.layerSorter);
        this.updateLayers();
    }
    removeWebXRLayer(layer) {
        removeSorted(this.layers, layer, this.layerSorter);
        this.layerSortOrder.delete(layer);
        this.updateLayers();
    }
    updateLayers() {
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
    async withFade(action) {
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
    set3DCursor(model) {
        const children = model.children.slice(0);
        for (const child of children) {
            this.cursor3D.add(child.name, child);
        }
        this.eventSys.refreshCursors();
        this.dispatchEvent(new TypedEvent("newcursorloaded"));
    }
    async load(progOrAsset, ...assets) {
        let prog = null;
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
    loadGltf(file) {
        return this.gltfLoader.loadAsync(file);
    }
}
//# sourceMappingURL=index.js.map