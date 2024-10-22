import { ElementChild } from "@juniper-lib/dom";
import { TabPanelElement } from "@juniper-lib/widgets";
export declare class DataBrowserElement extends TabPanelElement {
    #private;
    static observedAttributes: string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    show(tabName: string, emitEvent: boolean): void;
    static install(): import("@juniper-lib/dom").ElementFactory<DataBrowserElement>;
}
export declare function DataBrowser(...rest: ElementChild<DataBrowserElement>[]): DataBrowserElement;
//# sourceMappingURL=DataBrowserElement.d.ts.map