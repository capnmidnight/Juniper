import { arrayRemove, deg2rad, isDefined, isNullOrUndefined } from "@juniper-lib/util";
import { Task, TypedEvent, TypedEventTarget } from "@juniper-lib/events";
export class XRSessionStartedEvent extends TypedEvent {
    constructor(session, views) {
        super("sessionstarted");
        this.session = session;
        this.views = views;
    }
}
export class XRSessionManager extends TypedEventTarget {
    get done() {
        return this.doneTask;
    }
    constructor(gl, cam) {
        super();
        this.gl = gl;
        this.cam = cam;
        this.callbacks = new Array();
        this.lt = -1;
        this.sessionEndedEvt = new TypedEvent("sessionended");
        this.timerSource = globalThis;
        this.timer = null;
        this._sessionType = null;
        this._xrSession = null;
        this._baseRefSpace = null;
        this.fps = null;
        this.doneTask = new Task();
        this.starter = (t, frame) => {
            const session = frame && frame.session;
            const renderState = session && session.renderState;
            const baseLayer = renderState && renderState.baseLayer;
            const framebuffer = baseLayer && baseLayer.framebuffer;
            const ready = isNullOrUndefined(this._sessionType)
                || (isDefined(frame)
                    && isDefined(baseLayer)
                    && (this._sessionType === "inline"
                        || isDefined(framebuffer)));
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
        this.animator = (t, frame) => {
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
    async getSessionModes() {
        if (!navigator.xr) {
            return [];
        }
        const sessionTypes = [
            "immersive-vr",
            "inline",
            "immersive-ar"
        ];
        const supportedSessions = (await Promise.all(sessionTypes.map(async (type) => [type, await navigator.xr.isSessionSupported(type)])))
            .filter((v) => v[1])
            .map((v) => v[0]);
        return supportedSessions;
    }
    get inSession() {
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
    get sessionType() {
        return this._sessionType;
    }
    get baseRefSpace() {
        return this._baseRefSpace;
    }
    addTickCallback(callback) {
        this.callbacks.push(callback);
    }
    removeTickCallback(callback) {
        arrayRemove(this.callbacks, callback);
    }
    startAnimation() {
        this.getFrame(this.starter);
    }
    pauseAnimation() {
        if (this.timer) {
            this.timerSource.cancelAnimationFrame(this.timer);
            this.timer = null;
        }
    }
    getFrame(func) {
        this.pauseAnimation();
        this.timerSource = this.xrSession || globalThis;
        this.timer = this.timerSource.requestAnimationFrame((dt, frame) => {
            this.timer = null;
            func(dt, frame);
        });
    }
    async stopAnimation() {
        await this.endSession();
        this.pauseAnimation();
        if (this.doneTask) {
            this.doneTask.resolve();
            this.doneTask = null;
        }
    }
    async requestSession(type) {
        if (!navigator.xr) {
            throw new Error("No webXR");
        }
        const init = {
            optionalFeatures: [
                "local-floor"
            ]
        };
        return await navigator.xr.requestSession(type, init);
    }
    async startSession(type) {
        if (!this.inSession) {
            const xrSession = await this.requestSession(type);
            this._sessionType = type;
            xrSession.addEventListener("end", () => {
                this.xrSession = null;
                this._sessionType = null;
                this._baseRefSpace = null;
                this.dispatchEvent(this.sessionEndedEvt);
            });
            this._baseRefSpace = await xrSession.requestReferenceSpace(type === "inline"
                ? "viewer"
                : "local-floor");
            const attrib = this.gl.getContextAttributes();
            const baseLayer = new XRWebGLLayer(xrSession, this.gl, {
                alpha: attrib.alpha,
                antialias: attrib.antialias,
                depth: attrib.depth,
                stencil: attrib.stencil
            });
            const renderState = {
                baseLayer,
                depthNear: this.cam.near,
                depthFar: this.cam.far
            };
            if (type === "inline") {
                renderState.inlineVerticalFieldOfView = deg2rad(this.cam.fov);
            }
            await xrSession.updateRenderState(renderState);
            this.xrSession = xrSession;
        }
    }
    async endSession() {
        if (this.inSession) {
            await this.xrSession.end();
        }
    }
}
//# sourceMappingURL=XRSessionManager.js.map