import { Mesh, SphereGeometry } from "three";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";
export const sphereGeom = /*@__PURE__*/ new SphereGeometry(0.5);
sphereGeom.name = "SphereGeom";
sphereGeom.computeBoundingBox();
sphereGeom.computeBoundingSphere();
export const invSphereGeom = /*@__PURE__*/ sphereGeom.clone();
invSphereGeom.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphereGeom);
export class Sphere extends Mesh {
    constructor(size, material) {
        super(sphereGeom, material);
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
        super(invSphereGeom, material);
        this.scale.setScalar(0.5 * size);
    }
}
//# sourceMappingURL=Sphere.js.map