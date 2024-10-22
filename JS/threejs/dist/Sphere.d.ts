import { Mesh, SphereGeometry } from "three";
import type { SolidMaterial } from "./materials";
export declare const sphereGeom: SphereGeometry;
export declare const invSphereGeom: SphereGeometry;
export declare class Sphere extends Mesh<SphereGeometry, SolidMaterial> {
    constructor(size: number, material: SolidMaterial);
}
export declare function sphere(name: string, size: number, material: SolidMaterial): Sphere;
export declare class InvSphere extends Mesh {
    constructor(size: number, material: SolidMaterial);
}
//# sourceMappingURL=Sphere.d.ts.map