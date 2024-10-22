import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { NavTreeItemElement } from "./NavTreeItemElement";
declare class NavTreeElement extends TypedHTMLElement<{
    "itemselected": TypedItemSelectedEvent<NavTreeItemElement>;
}> {
    #private;
    constructor();
    connectedCallback(): void;
    get items(): NodeListOf<NavTreeItemElement>;
    select(type: string, value: string | number): void;
    clearSelection(): void;
    getItem(type: string, value: string | number): NavTreeItemElement;
    hasItemByLabel(type: string, label: string): boolean;
    static install(): import("@juniper-lib/dom").ElementFactory<NavTreeElement>;
}
export declare function NavTree(...rest: ElementChild<NavTreeElement>[]): NavTreeElement;
export {};
//# sourceMappingURL=NavTreeElement.d.ts.map