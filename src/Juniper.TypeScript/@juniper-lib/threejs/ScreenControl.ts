import { hasFullscreenAPI } from "@juniper-lib/dom/fullscreen";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { hasVR, hasWebVR, hasWebXR, isDefined, isMobileVR, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import WebXRPolyfill from "webxr-polyfill/src/WebXRPolyfill";
import { ScreenMode } from "./ScreenMode";
import type { ScreenUI } from "./ScreenUI";
import type { ScreenModeToggleButton } from "./widgets/ScreenModeToggleButton";

if (!navigator.xr) {
    console.info("Polyfilling WebXR API");
    new WebXRPolyfill();
}

export class XRSessionToggleEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T,
        public readonly mode: ScreenMode,
        public readonly session: XRSession,
        public readonly referenceSpaceType: XRReferenceSpaceType,
        public readonly sessionMode: XRSessionMode) {
        super(type);
    }
}

export class XRSessionStartedEvent extends XRSessionToggleEvent<"sessionstarted"> {
    constructor(mode: ScreenMode, session: XRSession, referenceSpaceType: XRReferenceSpaceType, sessionMode: XRSessionMode) {
        super("sessionstarted", mode, session, referenceSpaceType, sessionMode);
    }
}

export class XRSessionStoppedEvent extends XRSessionToggleEvent<"sessionstopped"> {
    constructor(mode: ScreenMode, session: XRSession, referenceSpaceType: XRReferenceSpaceType, sessionMode: XRSessionMode) {
        super("sessionstopped", mode, session, referenceSpaceType, sessionMode);
    }
}

interface ScreenControlEvents {
    sessionstarted: XRSessionStartedEvent;
    sessionstopped: XRSessionStoppedEvent;
}

const xrModes = new Map<ScreenMode, XRMode>([
    [ScreenMode.VR, {
        referenceSpaceType: "local-floor",
        sessionMode: "immersive-vr"
    }],
    [ScreenMode.AR, {
        referenceSpaceType: "local-floor",
        sessionMode: "immersive-ar"
    }]
]);

interface XRMode {
    referenceSpaceType: XRReferenceSpaceType;
    sessionMode: XRSessionMode;
}

export class ScreenControl
    extends TypedEventBase<ScreenControlEvents> {

    private _currentMode: ScreenMode = ScreenMode.None;
    private buttons = new Map<ScreenMode, ScreenModeToggleButton>();
    private currentSession: XRSession = null;
    private screenUI: ScreenUI = null;
    private readonly wasVisible = new Map<ScreenModeToggleButton, boolean>();

    constructor(
        private readonly renderer: THREE.WebGLRenderer,
        private readonly camera: THREE.PerspectiveCamera,
        private readonly fullscreenElement: HTMLElement,
        private readonly enableFullResolution: boolean) {
        super();

        this.addEventListener("sessionstarted", (evt) => {
            if (evt.sessionMode === "inline") {
                this.camera.fov = evt.session.renderState.inlineVerticalFieldOfView * 180 / Math.PI;
            }
        });

        this.addEventListener("sessionstopped", (evt) => {
            if (evt.sessionMode === "inline") {
                this.camera.fov = 50;
            }
        });

        this.renderer.xr.addEventListener("sessionstart", () => this.onSessionStarted());
        this.renderer.xr.addEventListener("sessionend", () => this.onSessionEnded());

        this.refresh();
    }

    setUI(screenUI: ScreenUI, fullscreenButton: ScreenModeToggleButton, vrButton: ScreenModeToggleButton, arButton?: ScreenModeToggleButton) {
        this.screenUI = screenUI;

        this.buttons.set(fullscreenButton.mode, fullscreenButton);
        this.buttons.set(vrButton.mode, vrButton);

        if (arButton) {
            this.buttons.set(arButton.mode, arButton);
            arButton.available = hasWebXR();
        }

        for (const button of this.buttons.values()) {
            this.wasVisible.set(button, button.visible);
            button.addEventListener("click", this.toggleMode.bind(this, button.mode));
        }

        fullscreenButton.available = !isMobileVR() && hasFullscreenAPI();
        vrButton.available = hasVR();
    }

    get visible() {
        return elementIsDisplayed(this.renderer.domElement);
    }

    set visible(v: boolean) {
        elementSetDisplay(this.renderer.domElement, v);
        if (this.screenUI) {
            elementSetDisplay(this.screenUI, v, "grid");
        }
    }

    get currentMode(): ScreenMode {
        return this._currentMode;
    }

    private lastWidth: number = 0;
    private lastHeight: number = 0;
    resize(): void {
        if (!this.renderer.xr.isPresenting) {
            const width = this.renderer.domElement.clientWidth;
            const height = this.renderer.domElement.clientHeight;
            if (width > 0
                && height > 0
                && (width !== this.lastWidth
                    || height !== this.lastHeight)) {
                this.renderer.setPixelRatio(devicePixelRatio);
                this.renderer.setSize(width, height, false);
                this.camera.aspect = width / height;
                this.camera.updateProjectionMatrix();
                this.lastWidth = width;
                this.lastHeight = height;
            }
        }
    }

    getMetrics() {
        const width = this.renderer.domElement.clientWidth;
        const height = this.renderer.domElement.clientHeight;
        const pixelRatio = this.renderer.getPixelRatio();
        const fov = this.camera.fov;
        return { width, height, pixelRatio, fov };
    }

    setMetrics(width: number, height: number, pixelRatio: number, fov: number): void {
        this.renderer.setPixelRatio(pixelRatio);
        this.renderer.setSize(width, height, false);
        this.camera.aspect = width / height;
        this.camera.fov = fov;
        this.camera.updateProjectionMatrix();
    }

    async refresh(): Promise<void> {
        await Promise.all(
            Array.from(this.buttons.values())
                .filter((btn) =>
                    btn.available
                    && btn.mode !== ScreenMode.Fullscreen)
                .map(async (btn) => {
                    const xrMode = xrModes.get(btn.mode);
                    btn.available = isDefined(xrMode);
                    if (btn.available) {
                        const typeSupported = navigator.xr
                            && await navigator.xr.isSessionSupported(xrMode.sessionMode);
                        const webVROverride = !hasWebXR()
                            && hasWebVR()
                            && xrMode.sessionMode === "immersive-vr"
                            && xrMode.referenceSpaceType === "local-floor";
                        btn.available = typeSupported
                            || webVROverride;
                    }
                }));
    }

    private async toggleMode(mode: ScreenMode): Promise<void> {
        if (mode === ScreenMode.Fullscreen) {
            await this.toggleFullscreen();
        }
        else if (mode !== ScreenMode.None) {
            await this.toggleXR(mode);
        }
    }

    async start(mode: ScreenMode): Promise<void> {
        if (mode !== this.currentMode) {
            await this.toggleMode(this.currentMode);
            await this.toggleMode(mode);
        }
    }

    async stop(): Promise<void> {
        await this.toggleMode(this.currentMode);
    }

    get isFullscreen(): boolean {
        return document.fullscreen;
    }

    private async startFullscreen() {
        if (!this.isFullscreen) {
            await this.fullscreenElement.requestFullscreen({
                navigationUI: "show"
            });
            this.setActive(ScreenMode.Fullscreen);
            this.dispatchEvent(new XRSessionStartedEvent(ScreenMode.Fullscreen, null, null, null));
        }
    }

    private async stopFullscreen() {
        if (this.isFullscreen) {
            await document.exitFullscreen();
            this.setActive(ScreenMode.None);
            this.dispatchEvent(new XRSessionStoppedEvent(ScreenMode.Fullscreen, null, null, null));
        }
    }

    private async toggleFullscreen(): Promise<void> {
        if (this.isFullscreen) {
            await this.stopFullscreen();
        }
        else {
            await this.startFullscreen();
        }
    }

    private async toggleXR(mode: ScreenMode): Promise<void> {
        const xrMode = xrModes.get(mode);
        if (isDefined(xrMode)) {
            if (this.currentSession) {
                this.currentSession.end();
            }
            else if (navigator.xr) {
                this.camera.position.set(0, 0, 0);
                this.camera.quaternion.identity();
                // WebXR"s requestReferenceSpace only works if the corresponding feature
                // was requested at session creation time. For simplicity, just ask for
                // the interesting ones as optional features, but be aware that the
                // requestReferenceSpace call will fail if it turns out to be unavailable.
                // ("local" is always available for immersive sessions and doesn"t need to
                // be requested separately.)
                try {
                    const session = await navigator.xr.requestSession(xrMode.sessionMode, {
                        optionalFeatures: [
                            "local-floor",
                            "bounded-floor",
                            "high-refresh-rate",
                            "hand-tracking",
                            "layers"
                        ]
                    });


                    this.setActive(mode);
                    this.currentSession = session;
                    this.renderer.xr.setReferenceSpaceType(xrMode.referenceSpaceType);
                    if (this.enableFullResolution
                        && "XRWebGLLayer" in window
                        && "getNativeFramebufferScaleFactor" in XRWebGLLayer) {
                        const size = XRWebGLLayer.getNativeFramebufferScaleFactor(session);
                        this.renderer.xr.setFramebufferScaleFactor(size);
                    }
                    this.renderer.xr.setSession(session as any);
                }
                catch (exp) {
                    console.error(`Couldn't start session type '${xrMode.sessionMode}'. Reason: ${exp && exp.message || exp || 'UNKNOWN'}`);
                }
            }
        }
    }

    private onSessionStarted() {
        const mode = this.currentMode;
        const xrMode = xrModes.get(this.currentMode);
        const session = this.currentSession;

        if (session.supportedFrameRates) {
            const max = Math.max(...session.supportedFrameRates);
            console.log("Changing framerate to", max);
            session.updateTargetFrameRate(max);
        }

        this.dispatchEvent(new XRSessionStartedEvent(
            mode,
            session,
            xrMode.referenceSpaceType,
            xrMode.sessionMode));
    }

    private onSessionEnded() {
        const mode = this.currentMode;
        const xrMode = xrModes.get(this.currentMode);
        const session = this.currentSession;
        this.currentSession = null;
        this.renderer.xr.setSession(null);
        this.setActive(ScreenMode.None);
        this.dispatchEvent(new XRSessionStoppedEvent(
            mode,
            session,
            xrMode.referenceSpaceType,
            xrMode.sessionMode));
    }

    private setActive(mode: ScreenMode): void {
        for (const button of this.buttons.values()) {
            button.active = button.mode === mode;
            button.visible = this.wasVisible.get(button)
                && (mode === ScreenMode.None
                    || button.mode === mode
                    || mode === ScreenMode.Fullscreen);
        }

        this._currentMode = mode;
    }
}