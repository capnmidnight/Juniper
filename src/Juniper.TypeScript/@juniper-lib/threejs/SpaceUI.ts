import { deg2rad } from "@juniper-lib/tslib";
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
const headPos = new THREE.Vector3(0, 0, 0);

export class SpaceUI extends THREE.Object3D {
    constructor() {
        super();
        this.name = "SpaceUI";
        this.position.y = -0.25;
    }

    addItem(child: Objects, position: Point2D): void {
        child = objectResolve(child);
        objGraph(this, child);
        child.position.set(
            radius * Math.sin(position.x * dAngleH),
            radius * Math.sin(position.y * dAngleV),
            -radius * Math.cos(position.x * dAngleH));
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
        for (const child of this.children) {
            child.lookAt(headPos);
        }
    }
}