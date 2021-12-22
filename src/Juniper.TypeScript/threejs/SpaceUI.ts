import { deg2rad } from "juniper-tslib";
import { objectResolve, Objects, objGraph } from "./objects";

interface Point2D {
    x: number;
    y: number;
    scale: number;
}

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
        this.add(child);
        child.position.set(
            radius * Math.sin(position.x * dAngleH),
            radius * Math.sin(position.y * dAngleV),
            -radius * Math.cos(position.x * dAngleH));
        child.scale.set(
            position.scale,
            position.scale,
            1);
        for (const child of this.children) {
            child.lookAt(headPos);
        }
    }
}