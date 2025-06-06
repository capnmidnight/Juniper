import { Object3D } from "three";
import { Line2 } from "../examples/lines/Line2";
import { LineGeometry } from "../examples/lines/LineGeometry";
import { line2 } from "../materials";
import { objGraph } from "../objects";

const geom = new LineGeometry();
geom.setPositions([
    0, 0, 0,
    0, 0, -1
]);

export class Laser extends Object3D {
    line: Line2;
    private _length = 1;

    constructor(color: number, opacity: number, linewidth: number) {
        super();
            
        this.line = new Line2(geom, line2({
            color,
            transparent: opacity < 1,
            opacity,
            linewidth
        }));
        this.line.computeLineDistances();

        objGraph(this, this.line);
    }

    get length() {
        return this._length;
    }

    set length(v) {
        this._length = v;
        this.line.scale.set(1, 1, v);
    }
}