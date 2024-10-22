import { Object3D, Texture, XRTargetRaySpace } from "three";
import { GLTFLoader } from "../loaders/GLTFLoader";
import { MotionController } from "./motion-controllers.module";
export declare class XRControllerModel extends Object3D {
    envMap: Texture;
    motionController: MotionController;
    setEnvironmentMap(envMap: Texture): this;
    /**
     * Polls data from the XRInputSource and updates the model's components to match
     * the real world data
     */
    updateMatrixWorld(force?: boolean): void;
}
export declare class XRControllerModelFactory {
    private readonly gltfLoader;
    private readonly path;
    private readonly _assetCache;
    constructor(gltfLoader?: GLTFLoader);
    createControllerModel(controller: XRTargetRaySpace, profileName?: string): XRControllerModel;
}
//# sourceMappingURL=XRControllerModelFactory.d.ts.map