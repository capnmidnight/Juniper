import { Color, Matrix4, Object3D, XRHandSpace } from "three";
export type XRHandModelPrimitiveProfileType = "spheres" | "boxes" | "bones";
export type XRHandModelProfileType = XRHandModelPrimitiveProfileType | "mesh";
export declare const jointNames: ReadonlyArray<XRHandJoint>;
export interface IXRHandModel extends Object3D {
    count: number;
    getMatrixAt(n: number, M: Matrix4): void;
    setMatrixAt(n: number, M: Matrix4): void;
    updateMesh(): void;
}
export declare class XRHandModel extends Object3D implements IXRHandModel {
    readonly controllerOrHandedness: XRHandSpace | XRHandedness;
    private readonly color;
    private readonly profile;
    private impl;
    get isTracking(): boolean;
    constructor(controllerOrHandedness: XRHandSpace | XRHandedness, color: Color, profile: XRHandModelProfileType);
    private createModel;
    get count(): number;
    set count(v: number);
    getMatrixAt(n: number, M: Matrix4): void;
    setMatrixAt(n: number, M: Matrix4): void;
    updateMesh(): void;
    updateMatrixWorld(force: boolean): void;
}
export declare class XRHandModelFactory {
    private readonly color;
    private readonly profile;
    constructor(color: Color, profile: XRHandModelProfileType);
    createHandModel(handedness: XRHandedness): XRHandModel;
    createHandModel(controller: XRHandSpace): XRHandModel;
}
//# sourceMappingURL=XRHandModelFactory.d.ts.map