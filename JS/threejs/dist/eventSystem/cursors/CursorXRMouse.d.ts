import { Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseCursor3D } from "./BaseCursor3D";
export declare class CursorXRMouse extends BaseCursor3D {
    private readonly system;
    private xr;
    constructor(env: BaseEnvironment);
    get content3d(): import("three").Object3D<import("three").Event>;
    get side(): number;
    set side(v: number);
    get cursor(): BaseCursor3D;
    set cursor(v: BaseCursor3D);
    get style(): import("@juniper-lib/dom").CssCursorValue;
    get visible(): boolean;
    set visible(v: boolean);
    set style(v: import("@juniper-lib/dom").CssCursorValue);
    _refresh(): void;
    lookAt(p: Vector3, v: Vector3): void;
}
//# sourceMappingURL=CursorXRMouse.d.ts.map