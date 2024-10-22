import { eventHandler } from "@juniper-lib/util";
import { ElementChild, HtmlEvent, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare class ShownEvent extends TypedEvent<"shown", DataBrowserPaneElement> {
    constructor(eventInitDict?: EventInit);
}
export declare class DisabledEvent extends TypedEvent<"disabled", DataBrowserPaneElement> {
    #private;
    get disabled(): boolean;
    constructor(disabled: boolean, eventInitDict?: EventInit);
}
export declare function OnShown(handler: eventHandler<ShownEvent>, options?: boolean | AddEventListenerOptions): HtmlEvent<"shown", ShownEvent>;
export declare function OnDisabled(handler: eventHandler<DisabledEvent>, options?: boolean | AddEventListenerOptions): HtmlEvent<"disabled", DisabledEvent>;
type DataBrowserPaneEventMap = {
    "shown": ShownEvent;
    "disabled": DisabledEvent;
};
export declare class DataBrowserPaneElement extends TypedHTMLElement<DataBrowserPaneEventMap> {
    #private;
    static observedAttributes: string[];
    constructor();
    show(): void;
    get disabled(): boolean;
    set disabled(v: boolean);
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    static install(): import("@juniper-lib/dom").ElementFactory<DataBrowserPaneElement>;
}
export declare function DataBrowserPane(name: string, ...rest: ElementChild<DataBrowserPaneElement>[]): DataBrowserPaneElement;
export {};
//# sourceMappingURL=DataBrowserPane.d.ts.map