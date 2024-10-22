import { BufferGeometry, Color, InstancedMesh, MeshPhongMaterial, XRHandSpace } from "three";
import { XRHandModelPrimitiveProfileType } from "./XRHandModelFactory";
export declare class XRHandPrimitiveModel extends InstancedMesh<BufferGeometry, MeshPhongMaterial> {
    readonly controller: XRHandSpace;
    private readonly oculusBrowserV14Correction;
    constructor(controller: XRHandSpace, handedness: XRHandedness, color: Color, primitive: XRHandModelPrimitiveProfileType);
    updateMesh(): void;
}
//# sourceMappingURL=XRHandPrimitiveModel.d.ts.map