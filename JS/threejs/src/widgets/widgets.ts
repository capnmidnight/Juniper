import { ElementChild, elementIsDisplayed, elementRemoveFromParent, elementSetDisplay, ErsatzElement } from "@juniper-lib/dom/dist/tags";
import {
    HtmlRender,
    elementClearChildren,
    isElementChild,
    isErsatzElement
} from "@juniper-lib/dom/dist/tags";
import { Object3D } from "three";
import { ErsatzObject, Objects, objRemoveFromParent } from "../objects";
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

export function widgetApply(obj: IWidget, ...children: WidgetChild[]): void {
    HtmlRender(obj, ...children.filter(isElementChild));
    objGraph(obj, ...children.filter(isObjects));
}

export function widgetRemoveFromParent(obj: IWidget): void {
    elementRemoveFromParent(obj);
    objRemoveFromParent(obj);
}

export function widgetClearChildren(obj: IWidget) {
    elementClearChildren(obj.element);
    objectClearChildren(obj.object);
}


export class Widget<T extends HTMLElement = HTMLElement> implements IWidget<T>, EventTarget {
    constructor(readonly element: T,
        readonly object: Object3D,
        private readonly displayType: CssGlobalValue | CssDisplayValue) {
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
