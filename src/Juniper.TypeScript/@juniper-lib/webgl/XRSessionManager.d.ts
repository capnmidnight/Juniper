/// <reference types="webxr" />
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { Task } from "@juniper-lib/events/Task";
import type { Camera } from "./Camera";
type TickCallback = (t: number, dt: number, frame: XRFrame) => void;
export declare class XRSessionStartedEvent extends TypedEvent<"sessionstarted"> {
    session: XRSession;
    views: ReadonlyArray<XRView>;
    constructor(session: XRSession, views: ReadonlyArray<XRView>);
}
export declare class XRSessionManager extends TypedEventTarget<{
    sessionstarted: XRSessionStartedEvent;
    sessionended: TypedEvent<"sessionended">;
}> {
    private gl;
    private cam;
    private callbacks;
    private lt;
    private sessionEndedEvt;
    private timerSource;
    private timer;
    private _sessionType;
    private _xrSession;
    private _baseRefSpace;
    private readonly starter;
    private readonly animator;
    private doneTask;
    fps: number;
    get done(): Task<void>;
    constructor(gl: WebGL2RenderingContext, cam: Camera);
    getSessionModes(): Promise<XRSessionMode[]>;
    get inSession(): boolean;
    get xrSession(): XRSession;
    set xrSession(v: XRSession);
    get sessionType(): XRSessionMode;
    get baseRefSpace(): XRReferenceSpace;
    addTickCallback(callback: TickCallback): void;
    removeTickCallback(callback: TickCallback): void;
    startAnimation(): void;
    private pauseAnimation;
    private getFrame;
    stopAnimation(): Promise<void>;
    private requestSession;
    startSession(type: XRSessionMode): Promise<void>;
    endSession(): Promise<void>;
}
export {};
//# sourceMappingURL=XRSessionManager.d.ts.map