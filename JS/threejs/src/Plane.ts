import { Mesh, PlaneGeometry } from "three";
import { SolidMaterial } from "./materials";

export const plane = /*@__PURE__*/ new PlaneGeometry(1, 1, 1, 1);
plane.name = "PlaneGeom";

export class Plane extends Mesh {
    constructor(sx: number, sy: number, material: SolidMaterial) {
        super(plane, material);
        this.scale.set(sx, sy, 1);
    }
}