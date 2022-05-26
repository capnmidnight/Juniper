import { buttonSetEnabled } from "@juniper-lib/dom/buttonSetEnabled";
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

export interface Widget extends ErsatzElement, ErsatzObject {
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

export function widgetSetEnabled(obj: Widget, enabled: boolean, buttonType: string) {
    if (obj.element instanceof HTMLButtonElement) {
        buttonSetEnabled(obj, enabled, buttonType);
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

