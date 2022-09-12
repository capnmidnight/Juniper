import { TorusBufferGeometry, Mesh } from "three";
import type { SolidMaterial } from "./materials";

export const torus = /*@__PURE__*/ new TorusBufferGeometry(1, 0.05, 10, 36, 2 * Math.PI);
torus.name = "TorusGeom";

export class Torus extends Mesh {
    constructor(sx: number, sy: number, sz: number, material: SolidMaterial) {
        super(torus, material);
        this.scale.set(sx, sy, sz);
    }
}