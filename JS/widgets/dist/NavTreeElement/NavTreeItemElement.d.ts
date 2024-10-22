import { ElementChild, HtmlAttr, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
export declare function Selectable(selectable: boolean): HtmlAttr<boolean, NavTreeItemElement>;
export declare class NavTreeItemElement extends TypedHTMLElement<{
    "itemselected": TypedItemSelectedEvent<NavTreeItemElement>;
}> {
    #private;
    static observedAttributes: string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    _refresh(): void;
    get type(): string;
    set type(v: string);
    get value(): string;
    set value(v: string);
    get label(): string;
    get selectable(): boolean;
    set selectable(v: boolean);
    get selected(): boolean;
    set selected(v: boolean);
    get open(): boolean;
    set open(v: boolean);
    static install(): import("@juniper-lib/dom").ElementFactory<NavTreeItemElement>;
}
export declare function NavTreeItem(...rest: ElementChild<NavTreeItemElement>[]): NavTreeItemElement;
//# sourceMappingURL=NavTreeItemElement.d.ts.map