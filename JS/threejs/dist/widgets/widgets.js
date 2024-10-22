import { elementClearChildren, elementRemoveFromParent, elementSetDisplay, elementSetEnabled, HtmlProp, HtmlRender, isDisplayed, isErsatzNode, isNodes } from "@juniper-lib/dom";
import { isErsatzObject, isObjects, objectClearChildren, objectSetEnabled, objectSetVisible, objGraph, objRemoveFromParent } from "../objects";
export function isWidget(obj) {
    return isErsatzNode(obj)
        && isErsatzObject(obj);
}
export function widgetSetEnabled(widget, enabled) {
    elementSetEnabled(widget, enabled);
    objectSetEnabled(widget, enabled);
}
export function widgetApply(obj, ...children) {
    HtmlRender(obj, ...children.filter(isNodes));
    objGraph(obj, ...children.filter(isObjects));
}
export function widgetRemoveFromParent(obj) {
    elementRemoveFromParent(obj);
    objRemoveFromParent(obj);
}
export function widgetClearChildren(obj) {
    elementClearChildren(obj);
    objectClearChildren(obj);
}
export function ObjectAttr(object) {
    return new HtmlProp("object", object);
}
export class Widget {
    #displayType;
    constructor(content, content3d, displayType) {
        this.#content = content;
        this.#content3d = content3d;
        this.#displayType = displayType;
    }
    #content;
    get content() { return this.#content; }
    #content3d;
    get content3d() { return this.#content3d; }
    get name() {
        return this.content3d.name;
    }
    get visible() {
        return isDisplayed(this);
    }
    set visible(visible) {
        elementSetDisplay(this, visible, this.#displayType);
        objectSetVisible(this, visible);
    }
}
//# sourceMappingURL=widgets.js.map