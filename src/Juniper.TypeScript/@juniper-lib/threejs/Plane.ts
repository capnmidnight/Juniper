import { SolidMaterial } from "./materials";

export const plane = /*@__PURE__*/ new THREE.PlaneBufferGeometry(1, 1, 1, 1);
plane.name = "PlaneGeom";

export class Plane extends THREE.Mesh {
    constructor(sx: number, sy: number, material: SolidMaterial) {
        super(plane, material);
        this.scale.set(sx, sy, 1);
    }
}