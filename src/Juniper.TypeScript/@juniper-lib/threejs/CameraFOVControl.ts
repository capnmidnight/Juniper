import { clamp, truncate } from "@juniper-lib/tslib";

export class CameraControl {

    fovZoomEnabled = true;
    minFOV = 15;
    maxFOV = 120;

    private dz: number = 0;

    constructor(private readonly camera: THREE.PerspectiveCamera) {

    }

    zoom(dz: number) {
        this.dz = dz;
    }

    get fov() {
        return this.camera.fov;
    }

    set fov(v) {
        if (v !== this.fov) {
            this.camera.fov = v;
            this.camera.updateProjectionMatrix();
        }
    }

    update(dt: number): void {
        if (this.fovZoomEnabled
            && Math.abs(this.dz) > 0) {
            const factor = Math.pow(0.95, 5 * dt);
            this.dz = truncate(factor * this.dz);
            this.fov = clamp(this.camera.fov - this.dz, this.minFOV, this.maxFOV);
        }
    }
}