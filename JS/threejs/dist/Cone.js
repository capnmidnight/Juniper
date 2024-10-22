import { ConeGeometry, Mesh } from "three";
export const geom = /*@__PURE__*/ new ConeGeometry(1, 1, 10, 5);
geom.name = "ConeGeom";
geom.computeBoundingBox();
geom.computeBoundingSphere();
export class Cone extends Mesh {
    constructor(sx, sy, sz, material) {
        super(geom, material);
        this.scale.set(sx, sy, sz);
    }
}
export function cone(name, sx, sy, sz, material) {
    const c = new Cone(sx, sy, sz, material);
    c.name = name;
    return c;
}
//# sourceMappingURL=Cone.js.map