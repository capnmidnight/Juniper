import { ConeBufferGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";

export const cone = /*@__PURE__*/ new ConeBufferGeometry(1, 1, 10, 5);
cone.name = "ConeGeom";
cone.computeBoundingBox();
cone.computeBoundingSphere();

export class Cone extends Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(cone, material);
        this.scale.set(sx, sy, sz);
    }
}