import { Mesh, SphereGeometry } from "three";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";
export const geom = /*@__PURE__*/ new SphereGeometry(0.5);
geom.name = "SphereGeom";
geom.computeBoundingBox();
geom.computeBoundingSphere();
export const invSphere = /*@__PURE__*/ geom.clone();
invSphere.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphere);
export class Sphere extends Mesh {
    constructor(size, material) {
        super(geom, material);
        this.scale.setScalar(0.5 * size);
    }
}
export function sphere(name, size, material) {
    const s = new Sphere(size, material);
    s.name = name;
    return s;
}
export class InvSphere extends Mesh {
    constructor(size, material) {
        super(invSphere, material);
        this.scale.setScalar(0.5 * size);
    }
}
//# sourceMappingURL=Sphere.js.map