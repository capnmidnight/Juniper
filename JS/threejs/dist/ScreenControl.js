import { isDefined, makeErrorMessage, rad2deg } from "@juniper-lib/util";
import { elementSetDisplay, hasFullscreenAPI, hasVR, hasWebVR, hasWebXR, isDisplayed, isMobileVR } from "@juniper-lib/dom";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import WebXRPolyfill from "webxr-polyfill/src/WebXRPolyfill";
import { ScreenMode } from "./ScreenMode";
import { AnaglyphEffect } from "./examples/effects/AnaglyphEffect";
if (!navigator.xr) {
    console.info("Polyfilling WebXR API");
    new WebXRPolyfill();
}
export class XRSessionToggleEvent extends TypedEvent {
    constructor(type, mode, session, referenceSpaceType, sessionMode) {
        super(type);
        this.mode = mode;
        this.session = session;
        this.referenceSpaceType = referenceSpaceType;
        this.sessionMode = sessionMode;
    }
}
export class XRSessionStartedEvent extends XRSessionToggleEvent {
    constructor(mode, session, referenceSpaceType, sessionMode) {
        super("sessionstarted", mode, session, referenceSpaceType, sessionMode);
    }
}
export class XRSessionStoppedEvent extends XRSessionToggleEvent {
    constructor(mode, session, referenceSpaceType, sessionMode) {
        super("sessionstopped", mode, session, referenceSpaceType, sessionMode);
    }
}
const xrModes = new Map([
    [ScreenMode.VR, {
            referenceSpaceType: "local-floor",
            sessionMode: "immersive-vr"
        }],
    [ScreenMode.AR, {
            referenceSpaceType: "local-floor",
            sessionMode: "immersive-ar"
        }]
]);
export class ScreenControl extends TypedEventTarget {
    constructor(renderer, camera, fullscreenElement, enableFullResolution, enableAnaglyph) {
        super();
        this.renderer = renderer;
        this.camera = camera;
        this.fullscreenElement = fullscreenElement;
        this.enableFullResolution = enableFullResolution;
        this.enableAnaglyph = enableAnaglyph;
        this._currentMode = ScreenMode.None;
        this.buttons = new Map();
        this.currentSession = null;
        this.screenUI = null;
        this.wasVisible = new Map();
        this.lastFOV = 50;
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
    setUI(screenUI, anaglyphButton, fullscreenButton, vrButton, arButton) {
        this.screenUI = screenUI;
        for (const btn of [anaglyphButton, fullscreenButton, vrButton, arButton]) {
            if (btn) {
                this.buttons.set(btn.mode, btn);
            }
        }
        for (const button of this.buttons.values()) {
            this.wasVisible.set(button, button.visible);
            button.content.addEventListener("click", this.toggleMode.bind(this, button.mode));
        }
        anaglyphButton.available = this.enableAnaglyph;
        fullscreenButton.available = !isMobileVR() && hasFullscreenAPI();
        vrButton.available = hasVR();
        arButton.available = hasWebXR();
        this.refresh();
    }
    get visible() {
        return isDisplayed(this.renderer.domElement);
    }
    set visible(v) {
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
    get currentMode() {
        return this._currentMode;
    }
    resize() {
        if (!this.renderer.xr.isPresenting) {
            this.renderer.domElement.style.width = "";
            this.renderer.domElement.style.height = "";
            const { clientWidth, clientHeight, width, height } = this.renderer.domElement;
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
    setMetrics(width, height, pixelRatio, fov) {
        this.renderer.setPixelRatio(pixelRatio);
        this.renderer.setSize(width, height, false);
        this.camera.aspect = width / height;
        this.camera.fov = fov;
        this.camera.updateProjectionMatrix();
    }
    async refresh() {
        const toCheck = Array.from(this.buttons.values())
            .filter((btn) => btn.available
            && btn.mode !== ScreenMode.Fullscreen
            && btn.mode !== ScreenMode.Anaglyph);
        await Promise.all(toCheck
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
    async toggleMode(mode) {
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
    async start(startMode) {
        let mode = startMode;
        if (startMode === ScreenMode.Anaglyph && this.currentMode === ScreenMode.Fullscreen
            || startMode === ScreenMode.Fullscreen && this.currentMode === ScreenMode.Anaglyph) {
            mode = ScreenMode.FullscreenAnaglyph;
        }
        if (mode !== this.currentMode) {
            await this.toggleMode(this.currentMode);
            await this.toggleMode(startMode);
        }
    }
    async stop() {
        await this.toggleMode(this.currentMode);
    }
    get isFullscreen() {
        return "fullscreenElement" in document && isDefined(document.fullscreenElement)
            || "fullscreen" in document && document.fullscreen;
    }
    async startFullscreen(wasAnaglyph) {
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
    async stopFullscreen(wasAnaglyph) {
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
    async toggleFullscreen(wasAnaglyph) {
        if (this.isFullscreen) {
            await this.stopFullscreen(wasAnaglyph);
        }
        else {
            await this.startFullscreen(wasAnaglyph);
        }
    }
    async toggleXR(mode) {
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
                    this.renderer.xr.setSession(session);
                }
                catch (exp) {
                    console.error(makeErrorMessage(`Couldn't start session type '${xrMode.sessionMode}'. Reason: $1`, exp));
                }
            }
        }
    }
    onSessionStarted() {
        const mode = this.currentMode;
        const xrMode = xrModes.get(this.currentMode);
        const session = this.currentSession;
        if (session.supportedFrameRates) {
            const max = Math.max(...session.supportedFrameRates);
            console.log("Changing framerate to", max);
            session.updateTargetFrameRate(max);
        }
        this.dispatchEvent(new XRSessionStartedEvent(mode, session, xrMode.referenceSpaceType, xrMode.sessionMode));
    }
    onSessionEnded() {
        const mode = this.currentMode;
        const xrMode = xrModes.get(this.currentMode);
        const session = this.currentSession;
        this.currentSession = null;
        this.renderer.xr.setSession(null);
        this.setActive(ScreenMode.None);
        this.dispatchEvent(new XRSessionStoppedEvent(mode, session, xrMode.referenceSpaceType, xrMode.sessionMode));
    }
    setActive(mode) {
        for (const button of this.buttons.values()) {
            button.active = button.mode === mode;
            button.visible = this.wasVisible.get(button)
                && ((mode !== ScreenMode.VR && mode !== ScreenMode.AR)
                    || button.mode === mode);
        }
        this._currentMode = mode;
    }
    render(scene, camera) {
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
//# sourceMappingURL=ScreenControl.js.map