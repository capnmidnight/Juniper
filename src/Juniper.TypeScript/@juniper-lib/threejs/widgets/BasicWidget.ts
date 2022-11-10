import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { Object3D } from "three";
import { Widget } from "./widgets";


export class BasicWidget<T extends HTMLElement> implements Widget<T>, EventTarget {
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
