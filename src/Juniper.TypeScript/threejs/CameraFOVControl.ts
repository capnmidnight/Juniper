import { clamp, truncate } from "juniper-tslib";
import type { EventSystemEvent } from "./eventSystem/EventSystemEvent";

export class CameraControl {

    fovZoomEnabled = true;
    minFOV = 15;
    maxFOV = 120;

    private dz: number = 0;

    constructor(private readonly camera: THREE.PerspectiveCamera) {

    }

    onMove(evt: EventSystemEvent<"move">) {
        if (evt.pointer.enabled
            && evt.pointer.canMoveView
            && evt.pointer.state.dz !== 0) {
            this.dz = evt.pointer.state.dz;
        }
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