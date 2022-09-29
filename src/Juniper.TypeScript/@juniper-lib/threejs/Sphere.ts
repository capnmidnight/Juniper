import { Mesh, SphereGeometry } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const sphere = /*@__PURE__*/ new SphereGeometry(0.5);
sphere.name = "SphereGeom";
sphere.computeBoundingBox();
sphere.computeBoundingSphere();

export const invSphere = /*@__PURE__*/ sphere.clone() as SphereGeometry;
invSphere.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphere);

export class Sphere extends Mesh {
    constructor(size: number, material: SolidMaterial) {
        super(sphere, material);
        this.scale.setScalar(0.5 * size);
    }
}

export class InvSphere extends Mesh {
    constructor(size: number, material: SolidMaterial) {
        super(invSphere, material);
        this.scale.setScalar(0.5 * size);
    }
}