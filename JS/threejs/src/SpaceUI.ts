import { deg2rad } from "@juniper-lib/util";
import { Object3D } from "three";
import { objectResolve, Objects, objGraph } from "./objects";

interface BasePoint2D {
    x: number,
    y: number
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

function isPoint2DHeight(v: BasePoint2D): v is Point2DHeight {
    return "height" in v;
}

function isPoint2DWidth(v: BasePoint2D): v is Point2DWidth {
    return "width" in v;
}

type Point2D = Point2DHeight | Point2DWidth | Point2DSquare;

const radius = 1.25;
const dAngleH = deg2rad(30);
const dAngleV = deg2rad(32);

export class SpaceUI extends Object3D {
    constructor() {
        super();
        this.name = "SpaceUI";
        this.position.y = -0.25;
    }

    addItem(child: Objects, position: Point2D): void {
        child = objectResolve(child);
        objGraph(this, child);

        child.rotation.set(
            position.y * dAngleV,
            -position.x * dAngleH,
            0,
            "YXZ"
        );

        child.position
            .set(0, 0, -radius)
            .applyEuler(child.rotation);

        if (isPoint2DHeight(position) && isPoint2DWidth(position)) {
            child.scale.set(position.width, position.height, 1);
        }
        else if (isPoint2DHeight(position)) {
            child.scale.multiplyScalar(position.height / child.scale.y);
        }
        else if (isPoint2DWidth(position)) {
            child.scale.multiplyScalar(position.width / child.scale.x);
        }
        else {
            child.scale.setScalar(position.scale);
        }

        child.scale.z = 1;
    }
}