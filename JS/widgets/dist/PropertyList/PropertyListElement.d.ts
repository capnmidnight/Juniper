import { ElementChild } from "@juniper-lib/dom";
export type PropertyElement = string | ElementChild | [string, ElementChild];
export declare class PropertyListElement extends HTMLElement {
    #private;
    static observedAttributes: string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    setGroupVisible(id: string, v: boolean): void;
    getGroupVisible(id: string): boolean;
    static install(): import("@juniper-lib/dom").ElementFactory<PropertyListElement>;
}
export declare function PropertyList(...rest: ElementChild<PropertyListElement>[]): PropertyListElement;
//# sourceMappingURL=PropertyListElement.d.ts.map