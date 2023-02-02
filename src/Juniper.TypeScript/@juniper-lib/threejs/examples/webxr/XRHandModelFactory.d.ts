import { Group, Object3D, XRHandSpace } from 'three';

import { XRHandPrimitiveModel, XRHandPrimitiveModelOptions } from './XRHandPrimitiveModel';
import { XRHandMeshModel } from './XRHandMeshModel';

export type XRHandModelHandedness = 'left' | 'right';

export class XRHandModel<T extends XRHandPrimitiveModel | XRHandMeshModel = XRHandPrimitiveModel | XRHandMeshModel>  extends Object3D {
    constructor(controller: XRHandSpace);

    motionController: T;
}

export type XRHandModelProfileType =
    | "spheres"
    | "boxes"
    | "bones"
    | "mesh";

interface XRHandModelTypes extends Record<XRHandModelProfileType, any> {
    "spheres": XRHandPrimitiveModel;
    "boxes": XRHandPrimitiveModel;
    "bones": XRHandPrimitiveModel;
    "mesh": XRHandMeshModel;
}

export class XRHandModelFactory {
    constructor();
    path: string;

    setPath(path: string): XRHandModelFactory;

    createHandModel<T extends keyof XRHandModelTypes>(
        controller: XRHandSpace,
        profile?: T,
        options?: XRHandPrimitiveModelOptions,
    ): XRHandModel<XRHandModelTypes[T]>;
}
