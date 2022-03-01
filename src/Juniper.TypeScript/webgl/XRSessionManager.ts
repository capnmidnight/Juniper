import { arrayRemove, deg2rad, Exception, isDefined, isNullOrUndefined, TypedEvent, TypedEventBase } from "juniper-tslib";
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
    private sessionEndedEvt = new TypedEvent("sessionended");

    private timerCallback: XRFrameRequestCallback = null;
    private timerSource: (typeof globalThis | XRSession) = globalThis;
    private timer: number = null;
    private _sessionType: XRSessionMode = null;
    private _xrSession: XRSession = null;
    private _baseRefSpace: XRReferenceSpace = null;

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
                this.startSession(evt && evt.session && evt.session.mode || "immersive-vr");
            });
        }

        (globalThis as any)["xr"] = this;
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

        return await navigator.xr.requestSession(type, init);
    }

    async startSession(type: XRSessionMode): Promise<void> {
        if (!this.inSession) {
            const xrSession = await this.requestSession(type);

            this._sessionType = type;

            xrSession.addEventListener("end", () => {
                this.xrSession = null;
                this._sessionType = null;
                this._baseRefSpace = null;
                this.dispatchEvent(this.sessionEndedEvt);
            });

            this._baseRefSpace = await xrSession.requestReferenceSpace(
                type === "inline"
                    ? "viewer"
                    : "local-floor");
            const attrib = this.gl.getContextAttributes();
            const baseLayer = new XRWebGLLayer(xrSession, this.gl, {
                alpha: attrib.alpha,
                antialias: attrib.antialias,
                depth: attrib.depth,
                stencil: attrib.stencil
            });

            const renderState: XRRenderStateInit = {
                baseLayer,
                depthNear: this.cam.near,
                depthFar: this.cam.far
            }

            if (type === "inline") {
                renderState.inlineVerticalFieldOfView = deg2rad(this.cam.fov);
            }

            await xrSession.updateRenderState(renderState);

            this.xrSession = xrSession;
            this.dispatchEvent(new XRSessionStartedEvent(xrSession, baseLayer, 2));
        }
    }

    async endSession(): Promise<void> {
        if (this.inSession) {
            await this.xrSession.end();
        }
    }
}
