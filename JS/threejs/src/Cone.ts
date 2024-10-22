import { ConeGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";

export const coneGeom = /*@__PURE__*/ new ConeGeometry(1, 1, 10, 5);
coneGeom.name = "ConeGeom";
coneGeom.computeBoundingBox();
coneGeom.computeBoundingSphere();

export class Cone extends Mesh<ConeGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(coneGeom, material);
        this.scale.set(sx, sy, sz);
    }
}

export function cone(name: string, sx: number, sy: number, sz: number, material: SolidMaterial): Mesh<ConeGeometry, SolidMaterial> {
    const c = new Cone(sx, sy, sz, material);
    c.name = name;
    return c;
}