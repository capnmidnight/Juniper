import type { ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";
import {
    elementApply,
    elementClearChildren,
    isElementChild,
    isErsatzElement
} from "@juniper-lib/dom/tags";
import type { ErsatzObject, Objects } from "../objects";
import {
    isErsatzObject,
    isObjects,
    objectClearChildren,
    objectSetEnabled,
    objGraph
} from "../objects";

export interface Widget<T extends HTMLElement = HTMLElement> extends ErsatzElement<T>, ErsatzObject {
    name: string;
    visible: boolean;
}

export function isWidget(obj: any): obj is Widget {
    return isErsatzElement(obj)
        && isErsatzObject(obj);
}

export type WidgetChild = Widget
    | ElementChild
    | Objects;

export function widgetSetEnabled(obj: Widget, enabled: boolean) {
    if (obj.element instanceof HTMLButtonElement) {
        obj.element.disabled = !enabled;
    }

    objectSetEnabled(obj, enabled);
}

export function widgetApply(obj: Widget, ...children: WidgetChild[]) {
    elementApply(obj, ...children.filter(isElementChild));
    objGraph(obj, ...children.filter(isObjects));
}

export function widgetClearChildren(obj: Widget) {
    elementClearChildren(obj.element);
    objectClearChildren(obj.object);
}

