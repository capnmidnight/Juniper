import { ElementChild } from "@juniper-lib/dom";
type DockPanelAttrTypes = "resizable" | "rearrangeable";
declare class DockPanelAttr<T extends DockPanelAttrTypes = DockPanelAttrTypes> {
    readonly type: T;
    readonly value: boolean;
    constructor(type: T, value: boolean);
}
export declare function resizable(v: boolean): DockPanelAttr<"resizable">;
export declare function rearrangeable(v: boolean): DockPanelAttr<"rearrangeable">;
export declare function DockPanel(name: string, rearrangeable: DockPanelAttr<"rearrangeable">, resizable: DockPanelAttr<"resizable">, ...rest: ElementChild[]): HTMLElement;
export declare function DockPanel(name: string, resizable: DockPanelAttr<"resizable">, rearrangeable: DockPanelAttr<"rearrangeable">, ...rest: ElementChild[]): HTMLElement;
export declare function DockPanel(name: string, rearrangeable: DockPanelAttr<"rearrangeable">, ...rest: ElementChild[]): HTMLElement;
export declare function DockPanel(name: string, resizable: DockPanelAttr<"resizable">, ...rest: ElementChild[]): HTMLElement;
export declare function DockPanel(name: string, ...rest: ElementChild[]): HTMLElement;
export declare function DockGroupColumn(...rest: ElementChild[]): HTMLDivElement;
export declare function DockGroupRow(...rest: ElementChild[]): HTMLDivElement;
export declare function DockCell(header: string | Date | number | ParentNode, ...rest: ElementChild[]): HTMLDivElement;
export {};
//# sourceMappingURL=index.d.ts.map