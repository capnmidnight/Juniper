import { ElementChild } from "@juniper-lib/dom";
export declare class SideDrawerElement extends HTMLElement {
    #private;
    constructor();
    connectedCallback(): void;
    get open(): boolean;
    set open(v: boolean);
    append(...nodes: (string | Node)[]): void;
    appendChild<T extends Node>(node: T): T;
    insertAdjacentElement(where: InsertPosition, element: Element): Element;
    insertAdjacentHTML(position: InsertPosition, text: string): void;
    insertAdjacentText(where: InsertPosition, data: string): void;
    replaceChildren(...nodes: (string | Node)[]): void;
    replaceChild<T extends Node>(node: Node, child: T): T;
    insertBefore<T extends Node>(node: T, child: Node): T;
    static install(): import("@juniper-lib/dom").ElementFactory<SideDrawerElement>;
}
export declare function SideDrawer(...rest: ElementChild<SideDrawerElement>[]): SideDrawerElement;
//# sourceMappingURL=SideDrawer.d.ts.map