import { ConeGeometry, Mesh } from "three";
export const coneGeom = /*@__PURE__*/ new ConeGeometry(1, 1, 10, 5);
coneGeom.name = "ConeGeom";
coneGeom.computeBoundingBox();
coneGeom.computeBoundingSphere();
export class Cone extends Mesh {
    constructor(sx, sy, sz, material) {
        super(coneGeom, material);
        this.scale.set(sx, sy, sz);
    }
}
export function cone(name, sx, sy, sz, material) {
    const c = new Cone(sx, sy, sz, material);
    c.name = name;
    return c;
}
//# sourceMappingURL=Cone.js.map