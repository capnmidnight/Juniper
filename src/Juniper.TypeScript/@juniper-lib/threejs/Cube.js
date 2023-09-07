import { BoxGeometry, Mesh } from "three";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";
export const cubeGeom = /*@__PURE__*/ new BoxGeometry(1, 1, 1, 1, 1, 1);
cubeGeom.name = "CubeGeom";
cubeGeom.computeBoundingBox();
cubeGeom.computeBoundingSphere();
export const invCubeGeom = /*@__PURE__*/ cubeGeom.clone();
invCubeGeom.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCubeGeom);
export class Cube extends Mesh {
    constructor(sx, sy, sz, material) {
        super(cubeGeom, material);
        this.scale.set(sx, sy, sz);
    }
}
export function cube(name, sx, sy, sz, material) {
    const c = new Cube(sx, sy, sz, material);
    c.name = name;
    return c;
}
export class InvCube extends Mesh {
    constructor(sx, sy, sz, material) {
        super(invCubeGeom, material);
        this.scale.set(sx, sy, sz);
    }
}
export function invCube(name, sx, sy, sz, material) {
    const c = new InvCube(sx, sy, sz, material);
    c.name = name;
    return c;
}
//# sourceMappingURL=Cube.js.map