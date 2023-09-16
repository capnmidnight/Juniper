import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { widgetApply } from "./widgets";
export class BasicWidget {
    constructor(element, object, displayType) {
        this.element = element;
        this.object = object;
        this.displayType = displayType;
    }
    get name() {
        return this.object.name;
    }
    addEventListener(type, listener, options) {
        this.element.addEventListener(type, listener, options);
    }
    dispatchEvent(event) {
        return this.element.dispatchEvent(event);
    }
    removeEventListener(type, callback, options) {
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
    add(...widgets) {
        widgetApply(this, ...widgets);
    }
}
//# sourceMappingURL=BasicWidget.js.map