import { Mesh, SphereGeometry } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const sphereGeom = /*@__PURE__*/ new SphereGeometry(0.5);
sphereGeom.name = "SphereGeom";
sphereGeom.computeBoundingBox();
sphereGeom.computeBoundingSphere();

export const invSphereGeom = /*@__PURE__*/ sphereGeom.clone() as SphereGeometry;
invSphereGeom.name = "InvertedSphereGeom";
setGeometryUVsForCubemaps(invSphereGeom);

export class Sphere extends Mesh<SphereGeometry, SolidMaterial> {
    constructor(size: number, material: SolidMaterial) {
        super(sphereGeom, material);
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
        super(invSphereGeom, material);
        this.scale.setScalar(0.5 * size);
    }
}