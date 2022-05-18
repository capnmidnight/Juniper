import { arrayRemove, deg2rad, Exception, isDefined, isNullOrUndefined, Task, TypedEvent, TypedEventBase } from "@juniper/tslib";
import type { Camera } from "./Camera";

type TickCallback = (t: number, dt: number, frame: XRFrame) => void;

export class XRSessionStartedEvent extends TypedEvent<"sessionstarted">{
    constructor(public session: XRSession, public views: ReadonlyArray<XRView>) {
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

    private timerSource: (typeof globalThis | XRSession) = globalThis;
    private timer: number = null;
    private _sessionType: XRSessionMode = null;
    private _xrSession: XRSession = null;
    private _baseRefSpace: XRReferenceSpace = null;

    private readonly starter: (t: number, frame?: XRFrame) => void;
    private readonly animator: (t: number, frame?: XRFrame) => void;

    private doneTask: Task<void>;

    fps: number = null;

    get done() {
        return this.doneTask;
    }

    constructor(private gl: WebGL2RenderingContext, private cam: Camera) {
        super();
        this.doneTask = new Task();

        this.starter = (t: number, frame?: XRFrame) => {
            const session = frame && frame.session;
            const renderState = session && session.renderState;
            const baseLayer = renderState && renderState.baseLayer;
            const framebuffer = baseLayer && baseLayer.framebuffer;
            const ready = isNullOrUndefined(this._sessionType)
                || (isDefined(frame)
                    && isDefined(baseLayer)
                    && (this._sessionType === "inline"
                        || isDefined(framebuffer)))

            if (ready) {
                if (this.xrSession) {
                    const pose = frame.getViewerPose(this.baseRefSpace);
                    this.dispatchEvent(new XRSessionStartedEvent(this.xrSession, pose.views));
                }
            }
            else {
                this.getFrame(this.animator);
                this.lt = 0.001 * t;
                this.getFrame(this.starter);
            }
        };

        this.animator = (t: number, frame?: XRFrame) => {
            t *= 0.001;
            const dt = t - this.lt;
            this.fps = 1 / dt;
            this.lt = t;
            this.getFrame(this.animator);
            for (const callback of this.callbacks) {
                callback(t, dt, frame);
            }
        };

        if (navigator.xr) {
            navigator.xr.addEventListener("sessiongranted", (evt) => {
                this.startSession(evt && evt.session && evt.session.mode || "immersive-vr");
            });
        }
    }

    async getSessionModes(): Promise<XRSessionMode[]> {
        if (!navigator.xr) {
            return [];
        }

        const sessionTypes: XRSessionMode[] = [
            "immersive-vr",
            "inline",
            "immersive-ar"
        ];
        const supportedSessions = (await Promise.all(sessionTypes.map<Promise<[XRSessionMode, boolean]>>(async (type) => [type, await navigator.xr.isSessionSupported(type)])))
            .filter((v) => v[1])
            .map((v) => v[0]);

        return supportedSessions;
    }

    get inSession(): boolean {
        return isDefined(this._xrSession);
    }

    get xrSession() {
        return this._xrSession;
    }

    set xrSession(v) {
        this.pauseAnimation();
        this._xrSession = v;
        this.startAnimation();
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
        this.getFrame(this.starter);
    }

    private pauseAnimation() {
        if (this.timer) {
            this.timerSource.cancelAnimationFrame(this.timer);
            this.timer = null;
        }
    }

    private getFrame(func: XRFrameRequestCallback) {
        this.pauseAnimation();
        this.timerSource = this.xrSession || globalThis;
        this.timer = this.timerSource.requestAnimationFrame((dt, frame?: XRFrame) => {
            this.timer = null;
            func(dt, frame);
        });
    }

    async stopAnimation(): Promise<void> {
        await this.endSession();
        this.pauseAnimation();
        if (this.doneTask) {
            this.doneTask.resolve();
            this.doneTask = null;
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
            console.log({ xrSession });
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
        }
    }

    async endSession(): Promise<void> {
        if (this.inSession) {
            await this.xrSession.end();
        }
    }
}
