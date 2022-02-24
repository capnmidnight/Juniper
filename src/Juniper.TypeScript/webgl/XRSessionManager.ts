import { arrayClear, arrayRemove, deg2rad, Exception, isDefined, isNullOrUndefined, TypedEvent, TypedEventBase } from "juniper-tslib";
import type { Camera } from "./Camera";

type TickCallback = (t: number, dt: number, frame: XRFrame) => void;

export class XRSessionStartedEvent extends TypedEvent<"sessionstarted">{
    constructor(public session: XRSession, public layer: XRWebGLLayer, public numViews: number) {
        super("sessionstarted");
    }
}

export class XRSessionManager extends TypedEventBase<{
    sessionstarted: XRSessionStartedEvent;
    sessionended: TypedEvent<"sessionended">;
}> {
    private callbacks = new Array<TickCallback>();
    private timerSource: (typeof globalThis | XRSession) = globalThis;
    private lt = -1;
    private tryLayers = true;
    private supportsLayers = false;
    private layers = new Array<XRLayer>();
    private sessionEndedEvt = new TypedEvent("sessionended");

    private timer: number = null;
    private _sessionType: XRSessionMode = null;
    private xrSession: XRSession = null;
    private _baseRefSpace: XRReferenceSpace = null;
    private _layerFactory: XRWebGLBinding = null;
    private finish: () => void = null;

    private task: Promise<void>;
    private starter: (t: number, frame?: XRFrame) => void;
    private animator: (t: number, frame?: XRFrame) => void;

    constructor(private gl: WebGL2RenderingContext, private cam: Camera) {
        super();

        this.starter = (t: number, frame?: XRFrame) => {
            if ((this._sessionType === "immersive-ar"
                || this._sessionType === "immersive-vr")
                && (isNullOrUndefined(frame)
                    || isNullOrUndefined(frame.session.renderState.baseLayer.framebuffer))) {
                this.lt = 0.001 * t;
                this.timerSource.requestAnimationFrame(this.starter);
            }
            else {
                let numViews = 1;
                let baseLayer: XRWebGLLayer = null;
                if (isDefined(frame)) {
                    const pose = frame.getViewerPose(this._baseRefSpace);
                    if (isNullOrUndefined(pose)) {
                        this.lt = 0.001 * t;
                        this.timerSource.requestAnimationFrame(this.starter);
                        return;
                    }
                    else {
                        numViews = pose.views.length;
                        baseLayer = frame.session.renderState.baseLayer;
                    }
                }

                this.dispatchEvent(new XRSessionStartedEvent(this.xrSession, baseLayer, numViews));

                this.animator(t, frame);
            }
        };

        this.animator = (t: number, frame?: XRFrame) => {
            t *= 0.001;
            const dt = t - this.lt;
            this.lt = t;
            this.timer = this.timerSource.requestAnimationFrame(this.animator);
            for (const callback of this.callbacks) {
                callback(t, dt, frame);
            }
        };

        if (navigator.xr) {
            navigator.xr.addEventListener("sessiongranted", (evt: any) =>
                this.startSession(evt && evt.session && evt.session.mode || "immersive-vr"));
        }
    }

    get layerFactory(): XRWebGLBinding {
        return this._layerFactory;
    }

    get isRunning(): boolean {
        return isDefined(this.task);
    }

    get isAnimating(): boolean {
        return isDefined(this.timer);
    }

    get inSession(): boolean {
        return isDefined(this.xrSession);
    }

    get sessionType(): XRSessionMode {
        return this._sessionType;
    }

    get baseRefSpace(): XRReferenceSpace {
        return this._baseRefSpace;
    }

    addTickCallback(callback: TickCallback): void {
        this.callbacks.push(callback);
    }

    removeTickCallback(callback: TickCallback): void {
        arrayRemove(this.callbacks, callback);
    }

    startAnimation(): Promise<void> {
        if (this.isRunning) {
            return this.task;
        }

        this.resumeAnimation();

        return this.task = new Promise<void>((resolve) => {
            this.finish = resolve;
        });
    }

    pauseAnimation(): void {
        if (this.isAnimating) {
            this.timerSource.cancelAnimationFrame(this.timer);
            this.timer = null;
            this.lt = -1;
        }
    }

    resumeAnimation(): void {
        if (!this.isAnimating) {
            this.timerSource.requestAnimationFrame(this.starter);
        }
    }

    private async requestSession(type: XRSessionMode): Promise<XRSession> {
        if (!navigator.xr) {
            throw new Exception("No webXR");
        }

        if (this.tryLayers) {
            try {
                this.supportsLayers = true;
                return await navigator.xr.requestSession(type, {
                    requiredFeatures: [
                        "layers"
                    ],
                    optionalFeatures: [
                        "local-floor"
                    ]
                });
            }
            catch (err) {
                this.tryLayers = false;
                this.supportsLayers = false;
            }
        }

        return await navigator.xr.requestSession(type, {
            optionalFeatures: [
                "local-floor"
            ]
        });
    }

    async startSession(type: XRSessionMode): Promise<void> {
        if (!this.inSession) {
            this.pauseAnimation();

            this.timerSource
                = this.xrSession
                = await this.requestSession(type);

            if (this.supportsLayers) {
                this._layerFactory = new XRWebGLBinding(this.xrSession, this.gl);
            }

            this._sessionType = type;

            this.xrSession.addEventListener("end", () => {
                this.pauseAnimation();
                this.timerSource = globalThis;
                this.xrSession = null;
                this._sessionType = null;
                this._baseRefSpace = null;
                this._layerFactory = null;
                arrayClear(this.layers);
                this.dispatchEvent(this.sessionEndedEvt);
                this.resumeAnimation();
            });

            this._baseRefSpace = await this.xrSession.requestReferenceSpace(
                type === "inline"
                    ? "viewer"
                    : "local-floor");

            const layerInit: XRWebGLLayerInit = {
                antialias: true,
                alpha: true,
                depth: true,
                stencil: false,
                framebufferScaleFactor: 1
            };

            const baseLayer = new XRWebGLLayer(this.xrSession, this.gl, layerInit);
            this.layers.push(baseLayer);

            const renderState: XRRenderStateInit = {
                layers: this.layers,
                depthNear: this.cam.near,
                depthFar: this.cam.far
            }

            if (type === "inline") {
                renderState.inlineVerticalFieldOfView = deg2rad(this.cam.fov);
            }

            await this.xrSession.updateRenderState(renderState);
        }

        this.resumeAnimation();
    }

    async addLayer(layer: XRLayer): Promise<void> {
        this.layers.push(layer);
        await this.xrSession.updateRenderState({
            layers: this.layers
        });
    }

    async removeLayer(layer: XRLayer): Promise<void> {
        arrayRemove(this.layers, layer);
        await this.xrSession.updateRenderState({
            layers: this.layers
        });
    }

    async endSession(): Promise<void> {
        if (this.inSession) {
            this.pauseAnimation();
            await this.xrSession.end();
        }
    }

    async stop(): Promise<void> {
        if (this.isRunning) {
            await this.endSession();
            this.pauseAnimation();
            this.finish();
            this.finish = null;
            this.task = null;
        }
    }
}
