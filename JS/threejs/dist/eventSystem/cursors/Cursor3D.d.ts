import { Object3D } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseCursor3D } from "./BaseCursor3D";
import { CursorSystem } from "./CursorSystem";
import { CssCursorValue } from "@juniper-lib/dom";
export declare class Cursor3D extends BaseCursor3D {
    private readonly cursorSystem;
    constructor(env: BaseEnvironment, cursorSystem?: CursorSystem);
    add(name: string, obj: Object3D): void;
    get style(): CssCursorValue;
    set style(v: CssCursorValue);
    get visible(): boolean;
    set visible(v: boolean);
    clone(): Cursor3D;
}
//# sourceMappingURL=Cursor3D.d.ts.map