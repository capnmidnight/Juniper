/// <reference types="webxr" />
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { PerspectiveCamera, Scene, WebGLRenderer } from "three";
import { ScreenMode } from "./ScreenMode";
import type { ScreenUI } from "./ScreenUI";
import type { ScreenModeToggleButton } from "./widgets/ScreenModeToggleButton";
export declare class XRSessionToggleEvent<T extends string> extends TypedEvent<T> {
    readonly mode: ScreenMode;
    readonly session: XRSession;
    readonly referenceSpaceType: XRReferenceSpaceType;
    readonly sessionMode: XRSessionMode;
    constructor(type: T, mode: ScreenMode, session: XRSession, referenceSpaceType: XRReferenceSpaceType, sessionMode: XRSessionMode);
}
export declare class XRSessionStartedEvent extends XRSessionToggleEvent<"sessionstarted"> {
    constructor(mode: ScreenMode, session: XRSession, referenceSpaceType: XRReferenceSpaceType, sessionMode: XRSessionMode);
}
export declare class XRSessionStoppedEvent extends XRSessionToggleEvent<"sessionstopped"> {
    constructor(mode: ScreenMode, session: XRSession, referenceSpaceType: XRReferenceSpaceType, sessionMode: XRSessionMode);
}
type ScreenControlEvents = {
    sessionstarted: XRSessionStartedEvent;
    sessionstopped: XRSessionStoppedEvent;
};
export declare class ScreenControl extends TypedEventTarget<ScreenControlEvents> {
    private readonly renderer;
    private readonly camera;
    readonly fullscreenElement: HTMLElement;
    private readonly enableFullResolution;
    private readonly enableAnaglyph;
    private _currentMode;
    private buttons;
    private currentSession;
    private screenUI;
    private readonly wasVisible;
    private lastFOV;
    private readonly anaglyph;
    constructor(renderer: WebGLRenderer, camera: PerspectiveCamera, fullscreenElement: HTMLElement, enableFullResolution: boolean, enableAnaglyph: boolean);
    setUI(screenUI: ScreenUI, anaglyphButton: ScreenModeToggleButton, fullscreenButton: ScreenModeToggleButton, vrButton: ScreenModeToggleButton, arButton: ScreenModeToggleButton): void;
    get visible(): boolean;
    set visible(v: boolean);
    get currentMode(): ScreenMode;
    resize(): void;
    getMetrics(): {
        width: number;
        height: number;
        pixelRatio: number;
        fov: number;
    };
    setMetrics(width: number, height: number, pixelRatio: number, fov: number): void;
    refresh(): Promise<void>;
    private toggleMode;
    start(startMode: ScreenMode): Promise<void>;
    stop(): Promise<void>;
    get isFullscreen(): boolean;
    private startFullscreen;
    private stopFullscreen;
    private toggleFullscreen;
    private toggleXR;
    private onSessionStarted;
    private onSessionEnded;
    private setActive;
    render(scene: Scene, camera: PerspectiveCamera): void;
}
export {};
//# sourceMappingURL=ScreenControl.d.ts.map