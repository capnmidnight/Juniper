export function hasFullscreenAPI() {
    return "requestFullscreen" in document.documentElement;
}
