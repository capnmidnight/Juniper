import { SolidMaterial, solidTransparentBlack } from "./materials";

export const plane = new THREE.PlaneBufferGeometry(1, 1, 1, 1);
plane.name = "PlaneGeom";

abstract class BasePlane extends THREE.Mesh {
    constructor(sx: number, sy: number, material: SolidMaterial, public isCollider: boolean) {
        super(plane, material);
        this.scale.set(sx, sy, 1);
    }
}

export class Plane extends BasePlane {
    isDraggable = false;

    constructor(sx: number, sy: number, material: SolidMaterial) {
        super(sx, sy, material, false);
    }
}

export class PlaneCollider extends BasePlane {
    constructor(sx: number, sy: number) {
        super(sx, sy, solidTransparentBlack(0), true);
        this.visible = false;
    }
}

