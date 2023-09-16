import { ElementChild, ErsatzElement } from "@juniper-lib/dom/dist/tags";
import { Object3D } from "three";
import { ErsatzObject, Objects } from "../objects";
export interface IWidget<T extends HTMLElement = HTMLElement> extends ErsatzElement<T>, ErsatzObject {
    name: string;
    visible: boolean;
}
export declare function isWidget(obj: any): obj is IWidget;
export type WidgetChild = IWidget | ElementChild | Objects;
export declare function widgetSetEnabled(obj: IWidget, enabled: boolean): void;
export declare function widgetApply(obj: IWidget, ...children: WidgetChild[]): void;
export declare function widgetRemoveFromParent(obj: IWidget): void;
export declare function widgetClearChildren(obj: IWidget): void;
export declare class Widget<T extends HTMLElement = HTMLElement> implements IWidget<T>, EventTarget {
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
}
//# sourceMappingURL=widgets.d.ts.map