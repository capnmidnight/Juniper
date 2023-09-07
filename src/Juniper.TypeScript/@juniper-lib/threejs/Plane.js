import { Mesh, PlaneGeometry } from "three";
export const plane = /*@__PURE__*/ new PlaneGeometry(1, 1, 1, 1);
plane.name = "PlaneGeom";
export class Plane extends Mesh {
    constructor(sx, sy, material) {
        super(plane, material);
        this.scale.set(sx, sy, 1);
    }
}
//# sourceMappingURL=Plane.js.map