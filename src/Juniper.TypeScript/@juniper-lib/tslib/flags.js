export function isChrome() {
    return "chrome" in globalThis && !navigator.userAgent.match("CriOS");
}
export function isFirefox() {
    return "InstallTrigger" in globalThis;
}
export function isSafari() {
    return /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
}
export function isMacOS() {
    return /^mac/i.test(navigator.platform);
}
export function isIOS() {
    return /iP(ad|hone|od)/.test(navigator.platform)
        || /Macintosh(.*?) FxiOS(.*?)\//.test(navigator.platform)
        || isMacOS()
            && "maxTouchPoints" in navigator
            && navigator.maxTouchPoints > 2;
}
export function isApple() {
    return isIOS()
        || isMacOS();
}
export function isMobileVR() {
    return /Mobile VR/.test(navigator.userAgent)
        || /Pico Neo 3 Link/.test(navigator.userAgent)
        || isOculusBrowser;
}
export function hasWebXR() {
    return "xr" in navigator
        && "isSessionSupported" in navigator.xr;
}
export function hasWebVR() {
    return "getVRDisplays" in navigator;
}
export function hasVR() {
    return hasWebXR() || hasWebVR();
}
export function isMobile() {
    return /Android/.test(navigator.userAgent)
        || /BlackBerry/.test(navigator.userAgent)
        || /(UC Browser |UCWEB)/.test(navigator.userAgent)
        || isIOS()
        || isMobileVR();
}
export function isDesktop() {
    return !isMobile();
}
const oculusBrowserPattern = /*@__PURE__*/ /OculusBrowser\/(\d+)\.(\d+)\.(\d+)/i;
const oculusMatch = /*@__PURE__*/ navigator.userAgent.match(oculusBrowserPattern);
export const isOculusBrowser = /*@__PURE__*/ !!oculusMatch;
export const oculusBrowserVersion = /*@__PURE__*/ isOculusBrowser && {
    major: parseFloat(oculusMatch[1]),
    minor: parseFloat(oculusMatch[2]),
    patch: parseFloat(oculusMatch[3])
};
export const isOculusGo = /*@__PURE__*/ isOculusBrowser && /pacific/i.test(navigator.userAgent);
export const isOculusQuest = /*@__PURE__*/ isOculusBrowser && /quest/i.test(navigator.userAgent);
export const isOculusQuest2 = /*@__PURE__*/ isOculusBrowser && /quest 2/i.test(navigator.userAgent);
export const isOculusQuest1 = /*@__PURE__*/ isOculusBrowser && !isOculusQuest2;
export const isWorkerSupported = /*@__PURE__*/ "Worker" in globalThis;
//# sourceMappingURL=flags.js.map