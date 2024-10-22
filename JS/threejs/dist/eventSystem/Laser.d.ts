import { Object3D } from "three";
import { Line2 } from "../examples/lines/Line2";
export declare class Laser extends Object3D {
    line: Line2;
    private _length;
    constructor(color: number, opacity: number, linewidth: number);
    get length(): number;
    set length(v: number);
}
//# sourceMappingURL=Laser.d.ts.map