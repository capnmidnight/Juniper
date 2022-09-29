import { BoxGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const cube = /*@__PURE__*/ new BoxGeometry(1, 1, 1, 1, 1, 1);
cube.name = "CubeGeom";
cube.computeBoundingBox();
cube.computeBoundingSphere();

export const invCube = /*@__PURE__*/ cube.clone() as BoxGeometry;
invCube.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCube);

export class Cube extends Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(cube, material);
        this.scale.set(sx, sy, sz);
    }
}

export class InvCube extends Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(invCube, material);
        this.scale.set(sx, sy, sz);
    }
}