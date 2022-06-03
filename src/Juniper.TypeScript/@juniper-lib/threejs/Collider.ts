import { solidTransparentBlack } from "./materials";

export class Collider extends THREE.Mesh {
    readonly isCollider = true;

    constructor(geometry: THREE.BufferGeometry) {
        super(geometry, solidTransparentBlack(0));
        this.visible = false;
    }
}