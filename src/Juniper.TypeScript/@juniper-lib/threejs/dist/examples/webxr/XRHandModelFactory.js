import { isString } from "@juniper-lib/tslib/dist/typeChecks";
import { Object3D } from "three";
import { cleanup } from "../../cleanup";
import { XRHandMeshModel } from "./XRHandMeshModel";
import { XRHandPrimitiveModel } from "./XRHandPrimitiveModel";
export const jointNames = [
    "wrist",
    "thumb-metacarpal",
    "thumb-phalanx-proximal",
    "thumb-phalanx-distal",
    "thumb-tip",
    "index-finger-metacarpal",
    "index-finger-phalanx-proximal",
    "index-finger-phalanx-intermediate",
    "index-finger-phalanx-distal",
    "index-finger-tip",
    "middle-finger-metacarpal",
    "middle-finger-phalanx-proximal",
    "middle-finger-phalanx-intermediate",
    "middle-finger-phalanx-distal",
    "middle-finger-tip",
    "ring-finger-metacarpal",
    "ring-finger-phalanx-proximal",
    "ring-finger-phalanx-intermediate",
    "ring-finger-phalanx-distal",
    "ring-finger-tip",
    "pinky-finger-metacarpal",
    "pinky-finger-phalanx-proximal",
    "pinky-finger-phalanx-intermediate",
    "pinky-finger-phalanx-distal",
    "pinky-finger-tip",
];
export class XRHandModel extends Object3D {
    get isTracking() {
        return !!this.impl;
    }
    constructor(controllerOrHandedness, color, profile) {
        super();
        this.controllerOrHandedness = controllerOrHandedness;
        this.color = color;
        this.profile = profile;
        this.impl = null;
        let controller = null;
        let handedness = null;
        if (isString(controllerOrHandedness)) {
            handedness = controllerOrHandedness;
        }
        else {
            controller = controllerOrHandedness;
        }
        const create = () => this.add(this.impl = this.createModel(controller, handedness));
        if (controller) {
            controller.addEventListener("connected", (event) => {
                const xrInputSource = event.data;
                if (xrInputSource.hand && !this.impl) {
                    handedness = xrInputSource.handedness;
                    create();
                }
            });
            controller.addEventListener("disconnected", () => {
                const old = this.impl;
                this.impl = null;
                handedness = null;
                old.removeFromParent();
                cleanup(old);
            });
        }
        else if (handedness) {
            create();
        }
    }
    createModel(controller, handedness) {
        if (this.profile === "mesh") {
            return new XRHandMeshModel(controller, handedness, this.color);
        }
        else {
            return new XRHandPrimitiveModel(controller, handedness, this.color, this.profile);
        }
    }
    get count() {
        if (this.impl) {
            return this.impl.count;
        }
        return 0;
    }
    set count(v) {
        if (this.impl) {
            this.impl.count = v;
        }
    }
    getMatrixAt(n, M) {
        if (this.impl) {
            this.impl.getMatrixAt(n, M);
        }
    }
    setMatrixAt(n, M) {
        if (this.impl) {
            this.impl.setMatrixAt(n, M);
        }
    }
    updateMesh() {
        if (this.impl) {
            this.impl.updateMesh();
        }
    }
    updateMatrixWorld(force) {
        super.updateMatrixWorld(force);
        if (this.impl) {
            this.impl.updateMesh();
        }
    }
}
export class XRHandModelFactory {
    constructor(color, profile) {
        this.color = color;
        this.profile = profile;
    }
    createHandModel(controllerOrHandedness) {
        return new XRHandModel(controllerOrHandedness, this.color, this.profile);
    }
}
//# sourceMappingURL=XRHandModelFactory.js.map