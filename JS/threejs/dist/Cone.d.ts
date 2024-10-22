import { ConeGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";
export declare const coneGeom: ConeGeometry;
export declare class Cone extends Mesh<ConeGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial);
}
export declare function cone(name: string, sx: number, sy: number, sz: number, material: SolidMaterial): Mesh<ConeGeometry, SolidMaterial>;
//# sourceMappingURL=Cone.d.ts.map