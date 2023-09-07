import { Object3D } from "three";
import { IWidget } from "./widgets";
export declare class BasicWidget<T extends HTMLElement = HTMLElement> implements IWidget<T>, EventTarget {
    readonly element: T;
    readonly object: Object3D;
    private readonly displayType;
    constructor(element: T, object: Object3D, displayType: CssGlobalValue | CssDisplayValue);
    get name(): string;
    addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    dispatchEvent(event: Event): boolean;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void;
    click(): void;
    get visible(): boolean;
    set visible(visible: boolean);
    add(...widgets: IWidget[]): void;
}
//# sourceMappingURL=BasicWidget.d.ts.map