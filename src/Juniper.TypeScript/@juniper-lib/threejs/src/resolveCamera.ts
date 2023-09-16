import { Camera, PerspectiveCamera, WebGLRenderer } from "three";

export function resolveCamera(renderer: WebGLRenderer, camera: PerspectiveCamera): Camera {
    if (renderer.xr.isPresenting) {
        return renderer.xr.getCamera();
    }
    else {
        return camera;
    }
}
