import { eventHandler, singleton } from "@juniper-lib/util";
import { CustomData, ElementChild, HtmlEvent, SingletonStyleBlob, TypedHTMLElement, display, em, maxWidth, registerFactory, rule } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";

export class ShownEvent extends TypedEvent<"shown", DataBrowserPaneElement> {
    constructor(eventInitDict?: EventInit) {
        super("shown", eventInitDict);
    }
}

export class DisabledEvent extends TypedEvent<"disabled", DataBrowserPaneElement> {
    #disabled: boolean;
    get disabled() { return this.#disabled; }
    constructor(disabled: boolean, eventInitDict?: EventInit) {
        super("disabled", eventInitDict);
        this.#disabled = disabled;
    }
}

export function OnShown(handler: eventHandler<ShownEvent>, options?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("shown", handler, options, false);
}

export function OnDisabled(handler: eventHandler<DisabledEvent>, options?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("disabled", handler, options, false);
}

type DataBrowserPaneEventMap = {
    "shown": ShownEvent;
    "disabled": DisabledEvent;
}

export class DataBrowserPaneElement extends TypedHTMLElement<DataBrowserPaneEventMap> {

    static observedAttributes = [
        "disabled"
    ];

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Cedrus::DataBrowserPane", () =>
            rule("data-browser-pane > fieldset",
                maxWidth(em(40))
            )
        );
    }

    #shown = new ShownEvent();
    show() {
        this.dispatchEvent(this.#shown);
    }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        if (name === "disabled") {
            this.dispatchEvent(new DisabledEvent(this.disabled));
        }
    }

    static install() {
        return singleton("Juniper::Cedrus::DataBrowserPaneElement", () => registerFactory("data-browser-pane", DataBrowserPaneElement));
    }
}

export function DataBrowserPane(name: string, ...rest: ElementChild<DataBrowserPaneElement>[]) {
    return DataBrowserPaneElement.install()(
        display("none"),
        CustomData("tab-name", name),
        ...rest
    );
}