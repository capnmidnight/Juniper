import { arrayClear, arrayRemove, arrayReplace, compareBy, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { Button, ClassList, Div, HtmlEvent, OnClick, SingletonStyleBlob, StyleAttr, TitleAttr, TypedHTMLElement, border, display, em, flexDirection, flexWrap, gap, justifyContent, maxHeight, overflowY, padding, registerFactory, rule } from "@juniper-lib/dom";
import { multiply } from "@juniper-lib/emoji";
import { TypedEvent } from "@juniper-lib/events";
import { fieldGetter, identityString } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
export function OnRemoving(callback, opts) {
    return new HtmlEvent("removing", callback, opts, false);
}
export class RemovingEvent extends TypedEvent {
    #item;
    get item() { return this.#item; }
    constructor(item) {
        super("removing", { bubbles: true });
        this.#item = item;
    }
}
export class ArrayViewElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "labelfield",
        "sortkeyfield"
    ]; }
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::ArrayView", () => rule("array-view", display("flex"), flexDirection("column"), border("inset 2px #ccc"), padding(em(.25)), maxHeight(em(20)), overflowY("auto"), rule(" > div", display("flex"), flexDirection("row"), flexWrap("nowrap"), justifyContent("space-between"), gap(em(.5)), rule(".selected", border("dashed 2px black"))), rule(" button", rule(".remove"))));
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name.toLowerCase()) {
            case "labelfield":
                this.labelField = newValue && fieldGetter(newValue) || identityString;
                break;
            case "sortkeyfield":
                this.sortKeyField = newValue && compareBy(fieldGetter(newValue)) || null;
                break;
            default:
                this.#render();
                break;
        }
    }
    #labelField = identityString;
    get labelField() { return this.#labelField; }
    set labelField(v) {
        this.#labelField = v || identityString;
        this.#render();
    }
    #sortKeyField = null;
    get sortKeyField() { return this.#sortKeyField; }
    set sortKeyField(v) {
        this.#sortKeyField = v;
        this.#render();
    }
    #values = [];
    get values() { return this.#values; }
    set values(v) {
        if (isNullOrUndefined(v)) {
            this.clear();
        }
        else {
            arrayReplace(this.#values, v);
            this.#render();
        }
    }
    #idvalues = [];
    get idvalues() { return this.#idvalues; }
    set idvalues(v) {
        if (isNullOrUndefined(v)) {
            this.clear();
        }
        else {
            arrayReplace(this.#idvalues, v);
            this.#render();
        }
    }
    #selectedItem = null;
    get selectedItem() { return this.#selectedItem; }
    set selectedItem(v) {
        this.#selectedItem = v;
        this.#render();
    }
    addItem(item) {
        this.#values.push(item);
        this.#idvalues.push(this.#labelField(item));
        this.#render();
    }
    removeItem(item) {
        arrayRemove(this.#values, item);
        arrayRemove(this.#idvalues, this.#labelField(item));
        this.#render();
    }
    clear() {
        arrayClear(this.#values);
        arrayClear(this.#idvalues);
        this.#render();
    }
    #itemToElement = new Map();
    #elementToItem = new Map();
    #render() {
        let viewcolor;
        if (this.#values.length === 0) {
            this.replaceChildren("No items");
        }
        else {
            if (this.#sortKeyField) {
                this.#values.sort(this.#sortKeyField);
            }
            this.#itemToElement.clear();
            this.#elementToItem.clear();
            for (const item of this.#values) {
                if (this.#idvalues.includes(this.#labelField(item))) {
                    viewcolor = "red";
                }
                else {
                    viewcolor = "--bs-body-color";
                }
                const label = this.#labelField(item);
                const element = Div(label, Button(multiply.value, ClassList("remove"), TitleAttr("Remove item"), OnClick(() => this.dispatchEvent(new RemovingEvent(item)))), OnClick(() => {
                    for (const element of this.#itemToElement.values()) {
                        element.classList.remove("selected");
                    }
                    if (item === this.#selectedItem) {
                        this.#selectedItem = null;
                    }
                    else {
                        this.#selectedItem = item;
                        element.classList.add("selected");
                    }
                    this.dispatchEvent(new TypedItemSelectedEvent(this.#selectedItem));
                }), StyleAttr({ color: viewcolor }));
                this.#itemToElement.set(item, element);
                this.#elementToItem.set(element, item);
            }
            if (this.#selectedItem) {
                if (this.#itemToElement.has(this.#selectedItem)) {
                    this.#itemToElement
                        .get(this.#selectedItem)
                        .classList
                        .add("selected");
                }
                else {
                    this.#selectedItem = null;
                }
            }
            this.replaceChildren(...this.#values.map(item => this.#itemToElement.get(item)));
        }
    }
    static install() {
        return singleton("Juniper::Widgets::ArrayViewElement", () => registerFactory("array-view", ArrayViewElement));
    }
}
export function ArrayView(...rest) {
    return ArrayViewElement.install()(...rest);
}
//# sourceMappingURL=ArrayViewElement.js.map