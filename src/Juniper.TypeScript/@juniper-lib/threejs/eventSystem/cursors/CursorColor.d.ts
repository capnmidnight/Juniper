import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { BaseCursor3D } from "./BaseCursor3D";
export declare class CursorColor extends BaseCursor3D {
    private _currentStyle;
    private material;
    constructor(env: BaseEnvironment);
    get style(): CssCursorValue;
    set style(v: CssCursorValue);
    get visible(): boolean;
    set visible(v: boolean);
}
//# sourceMappingURL=CursorColor.d.ts.map