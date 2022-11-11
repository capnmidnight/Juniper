import { ElementChild, elementIsDisplayed, elementSetDisplay, ErsatzElement } from "@juniper-lib/dom/tags";
import {
    elementApply,
    elementClearChildren,
    isElementChild,
    isErsatzElement
} from "@juniper-lib/dom/tags";
import { Object3D } from "three";
import type { ErsatzObject, Objects } from "../objects";
import {
    isErsatzObject,
    isObjects,
    objectClearChildren,
    objectSetEnabled,
    objGraph
} from "../objects";

export interface IWidget<T extends HTMLElement = HTMLElement> extends ErsatzElement<T>, ErsatzObject {
    name: string;
    visible: boolean;
}

export function isWidget(obj: any): obj is IWidget {
    return isErsatzElement(obj)
        && isErsatzObject(obj);
}

export type WidgetChild = IWidget
    | ElementChild
    | Objects;

export function widgetSetEnabled(obj: IWidget, enabled: boolean) {
    if (obj.element instanceof HTMLButtonElement) {
        obj.element.disabled = !enabled;
    }

    objectSetEnabled(obj, enabled);
}

export function widgetApply(obj: IWidget, ...children: WidgetChild[]) {
    elementApply(obj, ...children.filter(isElementChild));
    objGraph(obj, ...children.filter(isObjects));
}

export function widgetClearChildren(obj: IWidget) {
    elementClearChildren(obj.element);
    objectClearChildren(obj.object);
}


export class Widget<T extends HTMLElement = HTMLElement> implements IWidget<T>, EventTarget {
    constructor(readonly element: T,
        readonly object: Object3D,
        private readonly displayType: CSSGlobalValue | CSSDisplayValue) {
    }

    get name() {
        return this.object.name;
    }

    addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.element.addEventListener(type, listener, options);
    }

    dispatchEvent(event: Event): boolean {
        return this.element.dispatchEvent(event);
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void {
        this.element.removeEventListener(type, callback, options);
    }

    click() {
        this.element.click();
    }

    get visible() {
        return elementIsDisplayed(this);
    }

    set visible(visible) {
        elementSetDisplay(this, visible, this.displayType);
        this.object.visible = visible;
    }
}
