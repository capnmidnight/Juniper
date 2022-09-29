import { BoxGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const geom = /*@__PURE__*/ new BoxGeometry(1, 1, 1, 1, 1, 1);
geom.name = "CubeGeom";
geom.computeBoundingBox();
geom.computeBoundingSphere();

export const invCube = /*@__PURE__*/ geom.clone() as BoxGeometry;
invCube.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCube);

export class Cube extends Mesh<BoxGeometry, SolidMaterial>{
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(geom, material);
        this.scale.set(sx, sy, sz);
    }
}

export function cube(name: string, sx: number, sy: number, sz: number, material: SolidMaterial) {
    const c = new Cube(sx, sy, sz, material);
    c.name = name;
    return c;
}

export class InvCube extends Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(invCube, material);
        this.scale.set(sx, sy, sz);
    }
}