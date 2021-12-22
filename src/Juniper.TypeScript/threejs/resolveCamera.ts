export function resolveCamera(renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {
    if (renderer.xr.isPresenting) {
        return renderer.xr.getCamera(camera);
    }
    else {
        return camera;
    }
}
