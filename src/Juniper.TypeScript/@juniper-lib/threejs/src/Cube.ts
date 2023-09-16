import { BoxGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const cubeGeom = /*@__PURE__*/ new BoxGeometry(1, 1, 1, 1, 1, 1);
cubeGeom.name = "CubeGeom";
cubeGeom.computeBoundingBox();
cubeGeom.computeBoundingSphere();

export const invCubeGeom = /*@__PURE__*/ cubeGeom.clone() as BoxGeometry;
invCubeGeom.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCubeGeom);

export class Cube extends Mesh<BoxGeometry, SolidMaterial>{
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(cubeGeom, material);
        this.scale.set(sx, sy, sz);
    }
}

export function cube(name: string, sx: number, sy: number, sz: number, material: SolidMaterial) {
    const c = new Cube(sx, sy, sz, material);
    c.name = name;
    return c;
}

export class InvCube extends Mesh<BoxGeometry, SolidMaterial> {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(invCubeGeom, material);
        this.scale.set(sx, sy, sz);
    }
}

export function invCube(name: string, sx: number, sy: number, sz: number, material: SolidMaterial) {
    const c = new InvCube(sx, sy, sz, material);
    c.name = name;
    return c;
}