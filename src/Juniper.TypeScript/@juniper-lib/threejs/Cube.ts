import type { SolidMaterial } from "./materials";
import { solidTransparentBlack } from "./materials";
import { setGeometryUVsForCubemaps } from "./setGeometryUVsForCubemaps";

export const cube = new THREE.BoxBufferGeometry(1, 1, 1, 1, 1, 1);
cube.name = "CubeGeom";

export const invCube = cube.clone() as THREE.BoxBufferGeometry;
invCube.name = "InvertedCubeGeom";
setGeometryUVsForCubemaps(invCube);

abstract class BaseCube extends THREE.Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial, public isCollider: boolean) {
        super(cube, material);
        this.scale.set(sx, sy, sz);
    }
}

export class Cube extends BaseCube {
    isDraggable = false;

    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(sx, sy, sz, material, false);
    }
}

export class CubeCollider extends BaseCube {
    constructor(sx: number, sy: number, sz: number) {
        super(sx, sy, sz, solidTransparentBlack(0), true);
        this.visible = false;
    }
}