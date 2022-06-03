export function resolveCamera(renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera): THREE.Camera {
    if (renderer.xr.isPresenting) {
        return renderer.xr.getCamera();
    }
    else {
        return camera;
    }
}
