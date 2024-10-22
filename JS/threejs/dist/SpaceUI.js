import { deg2rad } from "@juniper-lib/util";
import { Object3D } from "three";
import { objectResolve, objGraph } from "./objects";
function isPoint2DHeight(v) {
    return "height" in v;
}
function isPoint2DWidth(v) {
    return "width" in v;
}
const radius = 1.25;
const dAngleH = deg2rad(30);
const dAngleV = deg2rad(32);
export class SpaceUI extends Object3D {
    constructor() {
        super();
        this.name = "SpaceUI";
        this.position.y = -0.25;
    }
    addItem(child, position) {
        child = objectResolve(child);
        objGraph(this, child);
        child.rotation.set(position.y * dAngleV, -position.x * dAngleH, 0, "YXZ");
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
//# sourceMappingURL=SpaceUI.js.map