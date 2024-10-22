import { ElementChild } from "@juniper-lib/dom";
export declare class NamedPanelElement extends HTMLElement {
    #private;
    static observedAttributes: string[];
    private readonly header;
    private readonly titleText;
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get open(): boolean;
    set open(v: boolean);
    static install(): import("@juniper-lib/dom").ElementFactory<NamedPanelElement>;
}
export declare function NamedPanel(...rest: ElementChild[]): NamedPanelElement;
//# sourceMappingURL=index.d.ts.map