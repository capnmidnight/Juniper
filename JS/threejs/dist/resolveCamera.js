export function resolveCamera(renderer, camera) {
    if (renderer.xr.isPresenting) {
        return renderer.xr.getCamera();
    }
    else {
        return camera;
    }
}
//# sourceMappingURL=resolveCamera.js.map