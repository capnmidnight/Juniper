import { Matrix4, Object3D, XRHandSpace } from 'three';
import { cleanup } from '../../cleanup';
import { XRHandMeshModel } from './XRHandMeshModel';
import { XRHandPrimitiveModel } from './XRHandPrimitiveModel';

export type XRHandModelPrimitiveProfileType =
    | "spheres"
    | "boxes"
    | "bones";

export type XRHandModelProfileType =
    | XRHandModelPrimitiveProfileType
    | "mesh";

export interface IXRHandModel extends Object3D {
    count: number;
    getMatrixAt(n: number, M: Matrix4): void;
    setMatrixAt(n: number, M: Matrix4): void;
    updateMatrixWorld(force?: boolean): void;
    voodoo(): void;
}

export class XRHandModel extends Object3D {

    motionController: IXRHandModel = null;
    xrInputSource: XRInputSource = null;

    constructor(public readonly controller: XRHandSpace) {
        super();
    }

    override updateMatrixWorld(force: boolean) {
        super.updateMatrixWorld(force);
        if (this.motionController) {
            this.motionController.updateMatrixWorld(force);
        }
    }
}

export class XRHandModelFactory {

    createModel(controller: XRHandSpace, handedness: XRHandedness, profile: XRHandModelProfileType): IXRHandModel {
        if (profile === 'mesh') {
            return new XRHandMeshModel(controller, handedness);
        } else {
            return new XRHandPrimitiveModel(controller, handedness, profile);
        }
    }

    createHandModel(controller: XRHandSpace, profile: XRHandModelProfileType): XRHandModel {

        const handModel = new XRHandModel(controller);

        controller.addEventListener('connected', (event) => {
            const xrInputSource = event.data as XRInputSource;
            if (xrInputSource.hand && !handModel.motionController) {
                handModel.xrInputSource = xrInputSource;
                handModel.motionController = this.createModel(controller, xrInputSource.handedness, profile);
                handModel.add(handModel.motionController);
            }
        });

        controller.addEventListener('disconnected', () => {
            const old = handModel.motionController;
            handModel.motionController = null;
            handModel.xrInputSource = null;
            old.removeFromParent();
            cleanup(old);
        });

        return handModel;

    }

}
