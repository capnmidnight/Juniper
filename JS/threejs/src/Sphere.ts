import { Mesh, SphereGeometry } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const geom = /*@__PURE__*/ new SphereGeometry(0.5);
geom.name = "SphereGeom";
geom.computeBoundingBox();
geom.computeBoundingSphere();

export const invSphere = /*@__PURE__*/ geom.clone() as SphereGeometry;
invSphere.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphere);

export class Sphere extends Mesh<SphereGeometry, SolidMaterial> {
    constructor(size: number, material: SolidMaterial) {
        super(geom, material);
        this.scale.setScalar(0.5 * size);
    }
}

export function sphere(name: string, size: number, material: SolidMaterial) {
    const s = new Sphere(size, material);
    s.name = name;
    return s;
}


export class InvSphere extends Mesh {
    constructor(size: number, material: SolidMaterial) {
        super(invSphere, material);
        this.scale.setScalar(0.5 * size);
    }
}