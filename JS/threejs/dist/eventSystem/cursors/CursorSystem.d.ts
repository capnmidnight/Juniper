import { BaseCursor } from "./BaseCursor";
export declare class CursorSystem extends BaseCursor {
    readonly element: HTMLElement;
    private _hidden;
    constructor(element: HTMLElement);
    get style(): import("@juniper-lib/dom").CssCursorValue;
    set style(v: import("@juniper-lib/dom").CssCursorValue);
    get visible(): boolean;
    set visible(v: boolean);
    private refresh;
}
//# sourceMappingURL=CursorSystem.d.ts.map