import type { CanvasTypes } from "juniper-dom/canvas";
import { createUtilityCanvas } from "juniper-dom/canvas";
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
} from "juniper-dom/css";
import { Style } from "juniper-dom/tags";
import type { IFetcher } from "juniper-fetcher";
import { TimerTickEvent } from "juniper-timers";
import {
    arrayRemove,
    arraySortByKeyInPlace,
    deg2rad,
    IProgress,
    isDefined,
    isDesktop,
    isFirefox,
    progressOfArray,
    TypedEvent,
    TypedEventBase
} from "juniper-tslib";
import { feet2Meters } from "juniper-units/length";
import { BodyFollower } from "../animation/BodyFollower";
import { updateScalings } from "../animation/scaleOnHover";
import { AvatarLocal } from "../AvatarLocal";
import { CameraControl } from "../CameraFOVControl";
import { cleanup } from "../cleanup";
import { Cursor3D } from "../eventSystem/Cursor3D";
import { EventSystem } from "../eventSystem/EventSystem";
import type { InteractiveObject3D } from "../eventSystem/InteractiveObject3D";
import { GLTFLoader } from "../examples/loaders/GLTFLoader";
import { Fader } from "../Fader";
import { Image2DMesh } from "../Image2DMesh";
import { deepSetLayer, FOREGROUND, PHOTOSPHERE_CAPTURE, PURGATORY } from "../layers";
import { LoadingBar } from "../LoadingBar";
import { obj, objGraph } from "../objects";
import { PhotosphereCaptureResolution } from "../PhotosphereCaptureResolution";
import { resolveCamera } from "../resolveCamera";
import { ScreenControl } from "../ScreenControl";
import { Skybox } from "../Skybox";
import { ThreeJSTimer } from "../ThreeJSTimer";
import { isMesh } from "../typeChecks";

const spectator = new THREE.PerspectiveCamera();
const lastViewport = new THREE.Vector4();
const curViewport = new THREE.Vector4();

const gridWidth = 15;
const gridSize = feet2Meters(gridWidth);

const FOVOffsets = new Map<PhotosphereCaptureResolution, number>([
    [PhotosphereCaptureResolution.Low, 4],
    [PhotosphereCaptureResolution.Medium, 8],
    [PhotosphereCaptureResolution.High, 3],
    [PhotosphereCaptureResolution.Fine, 2],
]);

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

export abstract class BaseEnvironment<Events>
    extends TypedEventBase<Events & BaseEnvironmentEvents> {

    private baseLayer: XRProjectionLayer;
    private readonly layers = new Array<XRLayer>();
    private readonly layerSortOrder = new Map<XRLayer, number>();

    private readonly fader: Fader;
    private fadeDepth = 0;

    readonly cursor3D: Cursor3D = new Cursor3D();

    readonly renderer: THREE.WebGLRenderer;
    readonly timer: ThreeJSTimer;
    readonly camera = new THREE.PerspectiveCamera(50, 1, 0.01, 1000);
    readonly scene = new THREE.Scene();
    readonly stage = obj("Stage");
    readonly ambient = new THREE.AmbientLight(0xffffff, 0.5);
    readonly sun = new THREE.DirectionalLight(0xffffff, 0.75);
    readonly ground = new THREE.GridHelper(gridSize, gridWidth, 0xc0c0c0, 0x808080);
    readonly foreground = obj("Foreground");
    readonly worldUISpace: BodyFollower;
    readonly loadingBar = new LoadingBar();

    readonly skybox: Skybox;
    readonly avatar: AvatarLocal;
    readonly cameraControl: CameraControl;
    readonly screenControl: ScreenControl;
    readonly eventSystem: EventSystem;

    constructor(canvas: CanvasTypes, public readonly fetcher: IFetcher, public readonly defaultAvatarHeight: number, enableFullResolution: boolean) {
        super();

        this.renderer = new THREE.WebGLRenderer({
            canvas,
            powerPreference: "high-performance",
            precision: "lowp",
            antialias: true,
            depth: true,
            stencil: true,
            premultipliedAlpha: true,
            logarithmicDepthBuffer: true,
            alpha: false,
            preserveDrawingBuffer: false
        });

        this.renderer.domElement.tabIndex = 1;

        this.cameraControl = new CameraControl(this.camera);

        this.screenControl = new ScreenControl(
            this.renderer,
            this.camera,
            this.renderer.domElement.parentElement,
            enableFullResolution);


        this.fader = new Fader("ViewFader");

        this.worldUISpace = new BodyFollower("WorldUISpace", 0.2, 20, 0.125);

        this.avatar = new AvatarLocal(
            this.renderer,
            this.camera,
            this.fader,
            defaultAvatarHeight);


        this.eventSystem = new EventSystem(this);

        this.skybox = new Skybox(this);

        this.timer = new ThreeJSTimer(this.renderer);

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

        this.timer.addTickHandler((evt) => this.update(evt));

        objGraph(this.scene,
            this.sun,
            this.ambient,
            objGraph(this.stage,
                this.ground,
                this.camera,
                this.avatar,
                ...this.eventSystem.hands
            ),
            this.foreground,
            objGraph(this.worldUISpace,
                this.loadingBar
            )
        );

        this.timer.start();

        (globalThis as any).env = this;
    }

    private update(evt: TimerTickEvent): void {
        if (this.screenControl.visible) {
            const session = this.renderer.xr.getSession() as any as XRSession;
            const baseLayer = session && (this.renderer.xr as any).getBaseLayer() as XRProjectionLayer;
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
            this.eventSystem.update();
            this.cameraControl.update(evt.dt);
            this.avatar.update(evt.dt);
            this.worldUISpace.update(this.avatar.height, this.avatar.worldPos, this.avatar.worldHeading, evt.dt);
            this.fader.update(evt.dt);
            updateScalings(evt.dt);
            this.loadingBar.update(evt.sdt);

            this.preRender(evt);

            this.skybox.update(evt.frame);

            const cam = resolveCamera(this.renderer, this.camera);
            if (cam !== this.camera) {
                const vrCam = cam as THREE.ArrayCamera;
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

    protected preRender(_evt: TimerTickEvent) {
    }

    protected async onQuitting(): Promise<void> {
        this.dispatchEvent(new TypedEvent("quitting"));
        window.location.href = "/";
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
        const session = this.renderer.xr.getSession() as any as XRSession;
        if (isDefined(session)) {
            console.log(this.layers.length, this.layers);
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
            this.loadingBar.report(0, 1);
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

    async constructPhotosphere(quality: PhotosphereCaptureResolution, fixWatermarks: boolean, getImagePath: (fov: number, heading: number, pitch: number) => string, downloadProg: IProgress): Promise<THREE.Object3D> {
        this.avatar.reset();

        const photosphere = obj("Photosphere");
        photosphere.layers.set(PHOTOSPHERE_CAPTURE);
        objGraph(this.foreground, photosphere);

        const FOV = quality as number;

        // Overlap images to crop copyright notices out of
        // most of the images...
        const dFOV = fixWatermarks
            ? FOVOffsets.get(FOV)
            : 0;

        photosphere.position.y = this.camera.getWorldPosition(new THREE.Vector3()).y;
        const size = 2;

        const angles = new Array<[number, number, number, number]>();

        for (let pitch = -90 + FOV; pitch < 90; pitch += FOV) {
            for (let heading = -180; heading < 180; heading += FOV) {
                angles.push([heading, pitch, FOV + dFOV, size]);
            }
        }

        // Include the top and the bottom
        angles.push([0, -90, FOV + dFOV, size]);
        angles.push([0, 90, FOV + dFOV, size]);

        if (fixWatermarks) {
            // Include an uncropped image so that
            // at least one of the copyright notices is visible.
            angles.push([0, -90, FOV, 0.5 * size]);
            angles.push([0, 90, FOV, 0.5 * size]);
        }

        this.camera.layers.set(PHOTOSPHERE_CAPTURE);

        await progressOfArray(downloadProg, angles, async (set, prog) => {
            const [heading, pitch, fov, size] = set;
            const halfFOV = 0.5 * deg2rad(fov);
            const dist = 0.5 * size / Math.tan(halfFOV);
            const path = getImagePath(fov, heading, pitch);
            const frame = new Image2DMesh(this, path, { transparent: false, side: THREE.DoubleSide });
            deepSetLayer(frame, PHOTOSPHERE_CAPTURE);
            await frame.loadImage(path, prog);
            const euler = new THREE.Euler(deg2rad(pitch), -deg2rad(heading), 0, "YXZ");
            const quat = new THREE.Quaternion().setFromEuler(euler);
            const pos = new THREE.Vector3(0, 0, -dist)
                .applyQuaternion(quat);
            frame.scale.setScalar(size);
            frame.quaternion.copy(quat);
            frame.position.copy(pos);
            photosphere.add(frame);
        });

        this.camera.layers.set(FOREGROUND);

        return photosphere;
    }

    async capturePhotosphere(renderProg: IProgress, quadFaceSize: number): Promise<CanvasTypes> {
        const metrics = this.screenControl.getMetrics();
        const { heading, pitch } = this.avatar;
        this.screenControl.setMetrics(quadFaceSize, quadFaceSize, 1, 90);

        const canv = createUtilityCanvas(quadFaceSize * 4, quadFaceSize * 3);
        const g = canv.getContext("2d");

        const captureParams = [
            [0, 1, 1, 0],
            [1, 0, 0, 1],
            [0, 0, 1, 1],
            [-1, 0, 2, 1],
            [2, 0, 3, 1],
            [0, -1, 1, 2]
        ];

        this.camera.layers.set(PHOTOSPHERE_CAPTURE);

        for (let i = 0; i < captureParams.length; ++i) {
            const [h, p, dx, dy] = captureParams[i];
            renderProg.report(i, captureParams.length, "rendering");
            this.avatar.setOrientationImmediate(h * Math.PI / 2, p * Math.PI / 2);
            this.renderer.render(this.scene, this.camera);
            g.drawImage(this.renderer.domElement, dx * quadFaceSize, dy * quadFaceSize);
            renderProg.report(i + 1, captureParams.length, "rendering");
        }

        this.camera.layers.set(FOREGROUND);


        this.screenControl.setMetrics(metrics.width, metrics.height, metrics.pixelRatio, metrics.fov);
        this.avatar.setOrientationImmediate(heading, pitch);
        return canv;
    }

    async loadModel(path: string, onProgress?: IProgress): Promise<THREE.Group> {
        const loader = new GLTFLoader();
        const model = await loader.loadAsync(path, (evt) => {
            onProgress.report(evt.loaded, evt.total, path);
        });
        model.scene.traverse((m: THREE.Object3D) => {
            if (isMesh(m)) {
                (m as InteractiveObject3D).isCollider = true;
                const material = m.material as (THREE.MeshStandardMaterial | THREE.MeshBasicMaterial);
                material.side = THREE.FrontSide;
                material.needsUpdate = true;
            }
        });
        return model.scene;
    }

    async load3DCursor(path: string, onProgress?: IProgress) {
        const model = await this.loadModel(path, onProgress);
        const children = model.children.slice(0);
        for (const child of children) {
            this.cursor3D.add(child.name, child);
        }
        this.eventSystem.refreshCursors();
        this.dispatchEvent(new TypedEvent("newcursorloaded"));
    }

    async load(onProgress?: IProgress) {
        await this.load3DCursor("/models/Cursors.glb", onProgress);
    }
}
