import { singleton } from "@juniper-lib/util";
import { CustomData, HtmlEvent, SingletonStyleBlob, TypedHTMLElement, display, em, maxWidth, registerFactory, rule } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export class ShownEvent extends TypedEvent {
    constructor(eventInitDict) {
        super("shown", eventInitDict);
    }
}
export class DisabledEvent extends TypedEvent {
    #disabled;
    get disabled() { return this.#disabled; }
    constructor(disabled, eventInitDict) {
        super("disabled", eventInitDict);
        this.#disabled = disabled;
    }
}
export function OnShown(handler, options) {
    return new HtmlEvent("shown", handler, options, false);
}
export function OnDisabled(handler, options) {
    return new HtmlEvent("disabled", handler, options, false);
}
export class DataBrowserPaneElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "disabled"
    ]; }
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Cedrus::DataBrowserPane", () => rule("data-browser-pane > fieldset", maxWidth(em(40))));
    }
    #shown = new ShownEvent();
    show() {
        this.dispatchEvent(this.#shown);
    }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        if (name === "disabled") {
            this.dispatchEvent(new DisabledEvent(this.disabled));
        }
    }
    static install() {
        return singleton("Juniper::Cedrus::DataBrowserPaneElement", () => registerFactory("data-browser-pane", DataBrowserPaneElement));
    }
}
export function DataBrowserPane(name, ...rest) {
    return DataBrowserPaneElement.install()(display("none"), CustomData("tab-name", name), ...rest);
}
//# sourceMappingURL=DataBrowserPane.js.map