import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const cube = /*@__PURE__*/ new THREE.BoxBufferGeometry(1, 1, 1, 1, 1, 1);
cube.name = "CubeGeom";

export const invCube = /*@__PURE__*/ cube.clone() as THREE.BoxBufferGeometry;
invCube.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCube);

export class Cube extends THREE.Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(cube, material);
        this.scale.set(sx, sy, sz);
    }
}

export class InvCube extends THREE.Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(invCube, material);
        this.scale.set(sx, sy, sz);
    }
}