import { ConeGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";

export const geom = /*@__PURE__*/ new ConeGeometry(1, 1, 10, 5);
geom.name = "ConeGeom";
geom.computeBoundingBox();
geom.computeBoundingSphere();

export class Cone extends Mesh<ConeGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(geom, material);
        this.scale.set(sx, sy, sz);
    }
}

export function cone(name: string, sx: number, sy: number, sz: number, material: SolidMaterial): Mesh<ConeGeometry, SolidMaterial> {
    const c = new Cone(sx, sy, sz, material);
    c.name = name;
    return c;
}