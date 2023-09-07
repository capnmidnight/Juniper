import { Object3D } from "three";
import { Objects } from "./objects";
interface BasePoint2D {
    x: number;
    y: number;
}
interface Point2DHeight extends BasePoint2D {
    height: number;
}
interface Point2DWidth extends BasePoint2D {
    width: number;
}
interface Point2DSquare extends BasePoint2D {
    scale: number;
}
type Point2D = Point2DHeight | Point2DWidth | Point2DSquare;
export declare class SpaceUI extends Object3D {
    constructor();
    addItem(child: Objects, position: Point2D): void;
}
export {};
//# sourceMappingURL=SpaceUI.d.ts.map