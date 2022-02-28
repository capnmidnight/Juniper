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
    private lt = -1;
    private tryLayers = true;
    private supportsLayers: boolean = null;
    private layers = new Array<XRLayer>();
    private sessionEndedEvt = new TypedEvent("sessionended");

    private timerCallback: XRFrameRequestCallback = null;
    private timerSource: (typeof globalThis | XRSession) = globalThis;
    private timer: number = null;
    private _sessionType: XRSessionMode = null;
    private _xrSession: XRSession = null;
    private _baseRefSpace: XRReferenceSpace = null;
    private _layerFactory: XRWebGLBinding = null;

    private readonly starter: (t: number, frame?: XRFrame) => void;
    private readonly animator: (t: number, frame?: XRFrame) => void;

    private finished: () => void;
    private task: Promise<void>;
    get done() {
        return this.task;
    }

    constructor(private gl: WebGL2RenderingContext, private cam: Camera) {
        super();
        this.task = new Promise<void>(resolve => this.finished = resolve);

        this.starter = (t: number, frame?: XRFrame) => {
            console.log("Starter", this._sessionType, frame);
            if ((this._sessionType === "immersive-ar"
                || this._sessionType === "immersive-vr")
                && (isNullOrUndefined(frame)
                    || isNullOrUndefined(frame.session.renderState.baseLayer.framebuffer))) {
                this.lt = 0.001 * t;
                this.resumeAnimation(this.starter);
            }
            else {
                this.resumeAnimation(this.animator);
            }
        };

        this.animator = (t: number, frame?: XRFrame) => {
            t *= 0.001;
            const dt = t - this.lt;
            this.lt = t;
            this.resumeAnimation(this.animator);
            for (const callback of this.callbacks) {
                callback(t, dt, frame);
            }
        };

        if (navigator.xr) {
            navigator.xr.addEventListener("sessiongranted", (evt: any) => {
                console.log("Session granted");
                this.startSession(evt && evt.session && evt.session.mode || "immersive-vr");
            });
        }

        (globalThis as any)["xr"] = this;
    }

    get layerFactory(): XRWebGLBinding {
        return this._layerFactory;
    }

    get inSession(): boolean {
        return isDefined(this._xrSession);
    }

    get xrSession() {
        return this._xrSession;
    }

    set xrSession(v) {
        const callback = this.timerCallback;
        this.pauseAnimation();
        this._xrSession = v;
        this.resumeAnimation(callback);
        if (v) {
            console.log("GOGOGO", v);
            const go = (dt: number, frame: XRFrame) => {
                console.log("go", dt, frame, v);
                v.requestAnimationFrame(go);
            }
            v.requestAnimationFrame(go);
        }
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

    startAnimation(): void {
        this.resumeAnimation(this.starter);
    }

    private pauseAnimation() {
        if (this.timer) {
            this.timerSource.cancelAnimationFrame(this.timer);
            this.timer = null;
        }
    }

    private resumeAnimation(func: XRFrameRequestCallback) {
        this.pauseAnimation();
        this.timerCallback = func;
        this.timerSource = this.xrSession || globalThis;
        this.timer = this.timerSource.requestAnimationFrame((dt, frame?: XRFrame) => {
            this.timer = null;
            func(dt, frame);
        });
    }

    async stopAnimation(): Promise<void> {
        await this.endSession();
        this.pauseAnimation();
        if (this.task) {
            this.finished();
            this.finished = null;
            this.task = null;
        }
    }

    private async requestSession(type: XRSessionMode): Promise<XRSession> {
        if (!navigator.xr) {
            throw new Exception("No webXR");
        }

        const init: XRSessionInit = {
            optionalFeatures: [
                "local-floor"
            ]
        };

        if (this.tryLayers) {
            init.optionalFeatures.push("layers");
        }

        try {
            const session = await navigator.xr.requestSession(type, init);
            this.supportsLayers = this.tryLayers;
            return session;
        }
        catch (err) {
            this.supportsLayers = false;
            this.tryLayers = false;
            return await this.requestSession(type);
        }
    }

    async startSession(type: XRSessionMode): Promise<void> {
        if (!this.inSession) {
            const xrSession = await this.requestSession(type);

            if (this.supportsLayers) {
                this._layerFactory = new XRWebGLBinding(xrSession, this.gl);
            }

            this._sessionType = type;

            xrSession.addEventListener("end", () => {
                console.log("SESSION ENDED!");
                this.xrSession = null;
                this._sessionType = null;
                this._baseRefSpace = null;
                this._layerFactory = null;
                arrayClear(this.layers);
                this.dispatchEvent(this.sessionEndedEvt);
            });

            this._baseRefSpace = await xrSession.requestReferenceSpace(
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

            const baseLayer = new XRWebGLLayer(xrSession, this.gl, layerInit);
            this.layers.push(baseLayer);

            const renderState: XRRenderStateInit = {
                layers: this.layers,
                depthNear: this.cam.near,
                depthFar: this.cam.far
            }

            if (type === "inline") {
                renderState.inlineVerticalFieldOfView = deg2rad(this.cam.fov);
            }

            await xrSession.updateRenderState(renderState);
            
            this.dispatchEvent(new XRSessionStartedEvent(xrSession, baseLayer, 2));
            this.xrSession = xrSession;
        }
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
            console.log("Ending session  aaaaaaaaaaaaaaaaaaaaaaa");
            await this.xrSession.end();
        }
    }
}
