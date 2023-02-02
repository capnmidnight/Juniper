import { BoxGeometry, CylinderGeometry, Group, InstancedMesh, MeshStandardMaterial, Object3D, SphereGeometry, Texture } from 'three';

import { XRHandModel, XRHandModelHandedness } from './XRHandModelFactory';

export interface XRHandPrimitiveModelOptions {
    primitive?: 'sphere' | 'box' | 'bone' | null;
}

export class XRHandPrimitiveModel {
    isXRHandPrimitiveModel: true;
    controller: Group;
    handModel: XRHandModel;
    envMap: Texture | null;
    handMesh: InstancedMesh<SphereGeometry|BoxGeometry|CylinderGeometry, MeshStandardMaterial>;

    constructor(
        handModel: Object3D,
        controller: Group,
        handedness: XRHandModelHandedness,
        options: XRHandPrimitiveModelOptions,
    );

    updateMesh: () => void;
}
