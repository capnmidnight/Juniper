import { elementIsDisplayed, elementRemoveFromParent, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { HtmlRender, elementClearChildren, isElementChild, isErsatzElement } from "@juniper-lib/dom/dist/tags";
import { objRemoveFromParent } from "../objects";
import { isErsatzObject, isObjects, objectClearChildren, objectSetEnabled, objGraph } from "../objects";
export function isWidget(obj) {
    return isErsatzElement(obj)
        && isErsatzObject(obj);
}
export function widgetSetEnabled(obj, enabled) {
    if (obj.element instanceof HTMLButtonElement) {
        obj.element.disabled = !enabled;
    }
    objectSetEnabled(obj, enabled);
}
export function widgetApply(obj, ...children) {
    HtmlRender(obj, ...children.filter(isElementChild));
    objGraph(obj, ...children.filter(isObjects));
}
export function widgetRemoveFromParent(obj) {
    elementRemoveFromParent(obj);
    objRemoveFromParent(obj);
}
export function widgetClearChildren(obj) {
    elementClearChildren(obj.element);
    objectClearChildren(obj.object);
}
export class Widget {
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
}
//# sourceMappingURL=widgets.js.map