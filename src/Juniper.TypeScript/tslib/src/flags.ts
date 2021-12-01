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
            && (navigator as any).maxTouchPoints > 2;
}

export function isApple() {
    return isIOS()
        || isMacOS();
}

export function isMobileVR() {
    return /Mobile VR/.test(navigator.userAgent)
        || isOculusBrowser;
}

export function hasWebXR() {
    return "xr" in navigator
        && "isSessionSupported" in (navigator as any).xr;
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

const oculusBrowserPattern = /OculusBrowser\/(\d+)\.(\d+)\.(\d+)/i;
const oculusMatch = navigator.userAgent.match(oculusBrowserPattern);
export const isOculusBrowser = !!oculusMatch;
export const oculusBrowserVersion: { major: number, minor: number, patch: number } = isOculusBrowser && {
    major: parseFloat(oculusMatch[1]),
    minor: parseFloat(oculusMatch[2]),
    patch: parseFloat(oculusMatch[3])
};

export const isOculusGo = isOculusBrowser && /pacific/i.test(navigator.userAgent);
export const isOculusQuest = isOculusBrowser && /quest/i.test(navigator.userAgent);
export const isOculusQuest2 = isOculusBrowser && /quest 2/i.test(navigator.userAgent);
export const isOculusQuest1 = isOculusBrowser && !isOculusQuest2;

export const isWorker = !("Document" in globalThis);