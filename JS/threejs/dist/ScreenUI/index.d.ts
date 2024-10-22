import { CssColorValue } from "@juniper-lib/dom";
export declare class ScreenUI {
    readonly elements: Array<HTMLElement>;
    readonly topLeft: HTMLElement;
    readonly topCenter: HTMLElement;
    readonly topRight: HTMLElement;
    readonly middleLeft: HTMLElement;
    readonly middleCenter: HTMLElement;
    readonly middleRight: HTMLElement;
    readonly bottomLeft: HTMLElement;
    readonly bottomCenter: HTMLElement;
    readonly bottomRight: HTMLElement;
    readonly cells: Array<Array<HTMLElement>>;
    constructor(buttonFillColor: CssColorValue);
    show(): void;
    hide(): void;
}
//# sourceMappingURL=index.d.ts.map