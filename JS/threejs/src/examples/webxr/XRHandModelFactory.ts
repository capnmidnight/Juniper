import { isString } from "@juniper-lib/util";
import { Color, Matrix4, Object3D, XRHandSpace } from "three";
import { cleanup } from "../../cleanup";
import { XRHandMeshModel } from "./XRHandMeshModel";
import { XRHandPrimitiveModel } from "./XRHandPrimitiveModel";

export type XRHandModelPrimitiveProfileType =
    | "spheres"
    | "boxes"
    | "bones";

export type XRHandModelProfileType =
    | XRHandModelPrimitiveProfileType
    | "mesh";

export const jointNames: ReadonlyArray<XRHandJoint> = [
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

export interface IXRHandModel extends Object3D {
    count: number;
    getMatrixAt(n: number, M: Matrix4): void;
    setMatrixAt(n: number, M: Matrix4): void;
    updateMesh(): void;
}

export class XRHandModel
    extends Object3D
    implements IXRHandModel {

    private impl: IXRHandModel = null;
    get isTracking() {
        return !!this.impl;
    }

    constructor(
        public readonly controllerOrHandedness: XRHandSpace | XRHandedness,
        private readonly color: Color,
        private readonly profile: XRHandModelProfileType) {
        super();

        let controller: XRHandSpace = null;
        let handedness: XRHandedness = null;
        if (isString(controllerOrHandedness)) {
            handedness = controllerOrHandedness;
        }
        else {
            controller = controllerOrHandedness;
        }

        const create = () => this.add(this.impl = this.createModel(controller, handedness));

        if (controller) {
            controller.addEventListener("connected", (event) => {
                const xrInputSource = event.data as XRInputSource;
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

    private createModel(controller: XRHandSpace, handedness: XRHandedness): IXRHandModel {
        if (this.profile === "mesh") {
            return new XRHandMeshModel(controller, handedness, this.color);
        } else {
            return new XRHandPrimitiveModel(controller, handedness, this.color, this.profile);
        }
    }

    get count(): number {
        if (this.impl) {
            return this.impl.count;
        }

        return 0;
    }

    set count(v: number) {
        if (this.impl) {
            this.impl.count = v;
        }
    }

    getMatrixAt(n: number, M: Matrix4): void {
        if (this.impl) {
            this.impl.getMatrixAt(n, M);
        }
    }

    setMatrixAt(n: number, M: Matrix4): void {
        if (this.impl) {
            this.impl.setMatrixAt(n, M);
        }
    }

    updateMesh(): void {
        if (this.impl) {
            this.impl.updateMesh();
        }
    }

    override updateMatrixWorld(force: boolean) {
        super.updateMatrixWorld(force);
        if (this.impl) {
            this.impl.updateMesh();
        }
    }
}

export class XRHandModelFactory {

    constructor(private readonly color: Color, private readonly profile: XRHandModelProfileType) {

    }

    createHandModel(handedness: XRHandedness): XRHandModel;
    createHandModel(controller: XRHandSpace): XRHandModel;
    createHandModel(controllerOrHandedness: XRHandSpace | XRHandedness): XRHandModel {
        return new XRHandModel(controllerOrHandedness, this.color, this.profile);
    }
}
