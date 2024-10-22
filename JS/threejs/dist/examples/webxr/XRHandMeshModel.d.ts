import { Color, Matrix4, Object3D, XRHandSpace } from "three";
import { IXRHandModel } from "./XRHandModelFactory";
export declare class XRHandMeshModel extends Object3D implements IXRHandModel {
    readonly controller: XRHandSpace;
    readonly handedness: XRHandedness;
    private readonly oculusBrowserV14Correction;
    private readonly bones;
    private root;
    readonly instanceMatrix: {
        needsUpdate: boolean;
    };
    constructor(controller: XRHandSpace, handedness: XRHandedness, color: Color);
    private addBone;
    get count(): number;
    set count(_v: number);
    getMatrixAt(n: number, M: Matrix4): void;
    setMatrixAt(n: number, M: Matrix4): void;
    updateMesh(): void;
}
//# sourceMappingURL=XRHandMeshModel.d.ts.map