import { BoxGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";
export declare const cubeGeom: BoxGeometry;
export declare const invCubeGeom: BoxGeometry;
export declare class Cube extends Mesh<BoxGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial);
}
export declare function cube(name: string, sx: number, sy: number, sz: number, material: SolidMaterial): Cube;
export declare class InvCube extends Mesh<BoxGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial);
}
export declare function invCube(name: string, sx: number, sy: number, sz: number, material: SolidMaterial): InvCube;
//# sourceMappingURL=Cube.d.ts.map