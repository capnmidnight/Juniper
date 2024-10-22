import { CssDisplayValue, CssGlobalValue, ElementChild, ErsatzNode, HtmlProp, InlineStylableElement } from "@juniper-lib/dom";
import { Object3D } from "three";
import { ErsatzObject, Objects } from "../objects";
export interface IWidget<DOMT extends Node = Node, THREET extends Object3D = Object3D> extends ErsatzNode<DOMT>, ErsatzObject<THREET> {
    name: string;
    visible: boolean;
}
export declare function isWidget<DOMT extends Node = Node, THREET extends Object3D = Object3D>(obj: any): obj is IWidget<DOMT, THREET>;
export type WidgetChild = IWidget | ElementChild | Objects;
export declare function widgetSetEnabled(widget: IWidget, enabled: boolean): void;
export declare function widgetApply<DOMT extends ParentNode>(obj: IWidget<DOMT>, ...children: WidgetChild[]): void;
export declare function widgetRemoveFromParent<DOMT extends ChildNode>(obj: IWidget<DOMT>): void;
export declare function widgetClearChildren<DOMT extends Element>(obj: IWidget<DOMT>): void;
export declare function ObjectAttr(object: Object3D): HtmlProp<"object", Object3D<import("three").Event>, Node & Record<"object", Object3D<import("three").Event>>>;
export declare class Widget<DOMT extends InlineStylableElement = InlineStylableElement, THREET extends Object3D = Object3D> implements IWidget<DOMT, THREET> {
    #private;
    constructor(content: DOMT, content3d: THREET, displayType: CssGlobalValue | CssDisplayValue);
    get content(): DOMT;
    get content3d(): THREET;
    get name(): string;
    get visible(): boolean;
    set visible(visible: boolean);
}
//# sourceMappingURL=widgets.d.ts.map