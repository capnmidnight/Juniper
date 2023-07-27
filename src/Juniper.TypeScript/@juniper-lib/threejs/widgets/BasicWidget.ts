import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { Object3D } from "three";
import { IWidget, widgetApply } from "./widgets";


export class BasicWidget<T extends HTMLElement = HTMLElement> implements IWidget<T>, EventTarget {
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

    add(...widgets: IWidget[]) {
        widgetApply(this, ...widgets);
    }
}
