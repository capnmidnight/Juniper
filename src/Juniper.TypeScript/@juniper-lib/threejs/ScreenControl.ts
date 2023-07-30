import { hasFullscreenAPI } from "@juniper-lib/dom/fullscreen";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventBase";
import { hasVR, hasWebVR, hasWebXR, isMobileVR } from "@juniper-lib/tslib/flags";
import { rad2deg } from "@juniper-lib/tslib/math";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { PerspectiveCamera, Scene, WebGLRenderer } from "three";
import WebXRPolyfill from "webxr-polyfill/src/WebXRPolyfill";
import { ScreenMode } from "./ScreenMode";
import type { ScreenUI } from "./ScreenUI";
import { AnaglyphEffect } from "./examples/effects/AnaglyphEffect";
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

type ScreenControlEvents = {
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
    extends TypedEventTarget<ScreenControlEvents> {

    private _currentMode: ScreenMode = ScreenMode.None;
    private buttons = new Map<ScreenMode, ScreenModeToggleButton>();
    private currentSession: XRSession = null;
    private screenUI: ScreenUI = null;
    private readonly wasVisible = new Map<ScreenModeToggleButton, boolean>();
    private lastFOV = 50;
    private readonly anaglyph: AnaglyphEffect;
    constructor(
        private readonly renderer: WebGLRenderer,
        private readonly camera: PerspectiveCamera,
        readonly fullscreenElement: HTMLElement,
        private readonly enableFullResolution: boolean,
        private readonly enableAnaglyph: boolean) {
        super();

        this.addEventListener("sessionstarted", (evt) => {
            if (evt.sessionMode === "inline") {
                this.lastFOV = this.camera.fov;
                this.camera.fov = rad2deg(evt.session.renderState.inlineVerticalFieldOfView);
            }
        });

        this.addEventListener("sessionstopped", (evt) => {
            if (evt.sessionMode === "inline") {
                this.camera.fov = this.lastFOV;
            }
        });

        this.renderer.xr.addEventListener("sessionstart", () => this.onSessionStarted());
        this.renderer.xr.addEventListener("sessionend", () => this.onSessionEnded());

        this.anaglyph = new AnaglyphEffect(this.renderer);

        this.refresh();
    }

    setUI(screenUI: ScreenUI, anaglyphButton: ScreenModeToggleButton, fullscreenButton: ScreenModeToggleButton, vrButton: ScreenModeToggleButton, arButton: ScreenModeToggleButton) {
        this.screenUI = screenUI;

        for (const btn of [anaglyphButton, fullscreenButton, vrButton, arButton]) {
            if (btn) {
                this.buttons.set(btn.mode, btn);
            }
        }

        for (const button of this.buttons.values()) {
            this.wasVisible.set(button, button.visible);
            button.addEventListener("click", this.toggleMode.bind(this, button.mode));
        }

        anaglyphButton.available = this.enableAnaglyph;
        fullscreenButton.available = !isMobileVR() && hasFullscreenAPI();
        vrButton.available = hasVR();
        arButton.available = hasWebXR();

        this.refresh();
    }

    get visible() {
        return elementIsDisplayed(this.renderer.domElement);
    }

    set visible(v: boolean) {
        elementSetDisplay(this.renderer.domElement, v);
        if (this.screenUI) {
            if (v) {
                this.screenUI.show();
            }
            else {
                this.screenUI.hide();
            }
        }
    }

    get currentMode(): ScreenMode {
        return this._currentMode;
    }

    resize(): void {
        if (!this.renderer.xr.isPresenting) {
            this.renderer.domElement.style.width = "";
            this.renderer.domElement.style.height = "";

            const {
                clientWidth,
                clientHeight,
                width,
                height
            } = this.renderer.domElement;

            const nextWidth = Math.floor(clientWidth * devicePixelRatio);
            const nextHeight = Math.floor(clientHeight * devicePixelRatio);

            if (clientWidth > 0
                && clientHeight > 0
                && (width !== nextWidth
                    || height !== nextHeight)) {
                this.renderer.setPixelRatio(devicePixelRatio);
                this.renderer.setSize(clientWidth, clientHeight, false);
                this.anaglyph.setSize(clientWidth, clientHeight);
                this.camera.aspect = clientWidth / clientHeight;
                this.camera.updateProjectionMatrix();
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
        const toCheck = Array.from(this.buttons.values())
            .filter((btn) =>
                btn.available
                && btn.mode !== ScreenMode.Fullscreen
                && btn.mode !== ScreenMode.Anaglyph);
        await Promise.all(
            toCheck
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
        if (mode === ScreenMode.None) {
            throw new Error("Cannot toggle 'None' Screen Mode");
        }
        else if (mode === ScreenMode.VR
            || mode === ScreenMode.AR) {
            await this.toggleXR(mode);
        }
        else {
            const isFullscreen = mode.indexOf("Fullscreen") >= 0;
            const isAnaglyph = mode.indexOf("Anaglyph") >= 0;
            const wasFullscreen = this.currentMode.indexOf("Fullscreen") >= 0;
            const wasAnaglyph = this.currentMode.indexOf("Anaglyph") >= 0;

            if (isFullscreen) {
                await this.toggleFullscreen(wasAnaglyph);
            }
            else {
                this.setActive(isAnaglyph !== wasAnaglyph
                    ? wasFullscreen
                        ? ScreenMode.FullscreenAnaglyph
                        : ScreenMode.Anaglyph
                    : wasFullscreen
                        ? ScreenMode.Fullscreen
                        : ScreenMode.None);
            }
        }
    }

    async start(startMode: ScreenMode): Promise<void> {
        let mode: ScreenMode = startMode;
        if (startMode === ScreenMode.Anaglyph && this.currentMode === ScreenMode.Fullscreen
            || startMode === ScreenMode.Fullscreen && this.currentMode === ScreenMode.Anaglyph) {
            mode = ScreenMode.FullscreenAnaglyph;
        }

        if (mode !== this.currentMode) {
            await this.toggleMode(this.currentMode);
            await this.toggleMode(startMode);
        }
    }

    async stop(): Promise<void> {
        await this.toggleMode(this.currentMode);
    }

    get isFullscreen(): boolean {
        return "fullscreenElement" in document && isDefined(document.fullscreenElement)
            || "fullscreen" in document && document.fullscreen;
    }

    private async startFullscreen(wasAnaglyph: boolean) {
        if (!this.isFullscreen) {
            await this.fullscreenElement.requestFullscreen({
                navigationUI: "show"
            });
            if (wasAnaglyph) {
                this.setActive(ScreenMode.FullscreenAnaglyph);
                this.dispatchEvent(new XRSessionStartedEvent(ScreenMode.FullscreenAnaglyph, null, null, null));
            }
            else {
                this.setActive(ScreenMode.Fullscreen);
                this.dispatchEvent(new XRSessionStartedEvent(ScreenMode.Fullscreen, null, null, null));
            }
        }
    }

    private async stopFullscreen(wasAnaglyph: boolean) {
        if (this.isFullscreen) {
            await document.exitFullscreen();
            if (wasAnaglyph) {
                this.setActive(ScreenMode.Anaglyph);
                this.dispatchEvent(new XRSessionStoppedEvent(ScreenMode.FullscreenAnaglyph, null, null, null));
            }
            else {
                this.setActive(ScreenMode.None);
                this.dispatchEvent(new XRSessionStoppedEvent(ScreenMode.Fullscreen, null, null, null));
            }
        }
    }

    private async toggleFullscreen(wasAnaglyph: boolean): Promise<void> {
        if (this.isFullscreen) {
            await this.stopFullscreen(wasAnaglyph);
        }
        else {
            await this.startFullscreen(wasAnaglyph);
        }
    }

    private async toggleXR(mode: ScreenMode): Promise<void> {
        const xrMode = xrModes.get(mode);
        if (isDefined(xrMode)) {
            if (this.currentSession) {
                this.currentSession.end();
            }
            else if (navigator.xr) {
                this.camera.position.setScalar(0);
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
                    console.error(`Couldn't start session type '${xrMode.sessionMode}'. Reason: ${exp && exp.message || exp || "UNKNOWN"}`);
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
                && ((mode !== ScreenMode.VR && mode !== ScreenMode.AR)
                    || button.mode === mode);
        }

        this._currentMode = mode;
    }

    render(scene: Scene, camera: PerspectiveCamera) {
        this.renderer.clear();

        if (this.currentMode === ScreenMode.Anaglyph
            || this.currentMode === ScreenMode.FullscreenAnaglyph) {
            this.anaglyph.render(scene, camera);
        }
        else {
            this.renderer.render(scene, camera);
        }
    }
}