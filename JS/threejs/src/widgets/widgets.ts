import {
    CssDisplayValue, CssGlobalValue, ElementChild,
    elementClearChildren,
    elementRemoveFromParent,
    elementSetDisplay,
    elementSetEnabled,
    ErsatzNode,
    HtmlProp, HtmlRender,
    InlineStylableElement,
    isDisplayed,
    isErsatzNode,
    isNodes
} from "@juniper-lib/dom";
import { Object3D } from "three";
import {
    ErsatzObject, isErsatzObject,
    isObjects,
    objectClearChildren, Objects, objectSetEnabled,
    objectSetVisible,
    objGraph, objRemoveFromParent
} from "../objects";

export interface IWidget<DOMT extends Node = Node, THREET extends Object3D = Object3D> extends ErsatzNode<DOMT>, ErsatzObject<THREET> {
    name: string;
    visible: boolean;
}

export function isWidget<DOMT extends Node = Node, THREET extends Object3D = Object3D>(obj: any): obj is IWidget<DOMT, THREET> {
    return isErsatzNode(obj)
        && isErsatzObject(obj);
}

export type WidgetChild = IWidget
    | ElementChild
    | Objects;

export function widgetSetEnabled(widget: IWidget, enabled: boolean) {
    elementSetEnabled(widget, enabled);
    objectSetEnabled(widget, enabled);
}

export function widgetApply<DOMT extends ParentNode>(obj: IWidget<DOMT>, ...children: WidgetChild[]): void {
    HtmlRender(obj, ...children.filter(isNodes));
    objGraph(obj, ...children.filter(isObjects));
}

export function widgetRemoveFromParent<DOMT extends ChildNode>(obj: IWidget<DOMT>): void {
    elementRemoveFromParent(obj);
    objRemoveFromParent(obj);
}

export function widgetClearChildren<DOMT extends Element>(obj: IWidget<DOMT>) {
    elementClearChildren(obj)
    objectClearChildren(obj);
}

export function ObjectAttr(object: Object3D) {
    return new HtmlProp("object", object);
}


export class Widget<DOMT extends InlineStylableElement = InlineStylableElement, THREET extends Object3D = Object3D> implements IWidget<DOMT, THREET> {

    readonly #displayType: CssGlobalValue | CssDisplayValue;

    constructor(content: DOMT, content3d: THREET, displayType: CssGlobalValue | CssDisplayValue) {
        this.#content = content;
        this.#content3d = content3d;
        this.#displayType = displayType;
    }

    readonly #content: DOMT;
    get content() { return this.#content; }

    readonly #content3d: THREET;
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