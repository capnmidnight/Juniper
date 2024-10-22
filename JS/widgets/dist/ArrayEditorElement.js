import { arrayReplace, compareBy, isDefined, singleton } from "@juniper-lib/util";
import { ClassList, Div, Name, P, Query, SingletonStyleBlob, SlotTag, TemplateInstance, alignItems, display, flexDirection, registerFactory, rule, visibility } from "@juniper-lib/dom";
import { ArrayView, OnRemoving } from "./ArrayViewElement";
import { fieldGetter, identityString } from "./FieldDef";
import { IKEA } from "./IKEA";
export class ArrayEditorElement extends HTMLElement {
    static { this.observedAttributes = [
        "disabled",
        "readonly",
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "placeholder",
        "allowduplicates"
    ]; }
    #template;
    #array;
    #observer;
    #onItemSelected;
    #saveLabel;
    #selector;
    get selector() { return this.#selector; }
    #data = [];
    get data() { return this.#data; }
    set data(v) {
        arrayReplace(this.#data, v);
        this.#render();
    }
    #initvalues = [];
    get values() { return this.#array.values; }
    set values(v) {
        this.#array.clear();
        this.#array.values = v;
        this.#initvalues = v;
        this.#render();
    }
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::ArrayEditor", () => [
            rule("array-editor", display("flex"), flexDirection("column")),
            rule("array-editor > div.controls", display("flex"), flexDirection("column"), alignItems("stretch")),
            rule("array-editor button"),
            rule("array-editor button.add"),
            rule("array-editor button.remove")
        ]);
        this.#template = TemplateInstance("Foxgolve::Widgets::ArrayEditor", () => [
            Div(ClassList("controls", "input-group"), SlotTag(Name("selector"))),
            ArrayView(),
            P("* Unsaved Changes *", visibility("hidden")),
        ]);
        this.#saveLabel = P(Query(this.#template, "p"));
        this.#array = ArrayView(Query(this.#template, "array-view"), OnRemoving(evt => this.#removeItem(evt.item)));
        this.#onItemSelected = () => {
            if (this.#selector?.selectedItem) {
                this.#addItem(this.#selector.selectedItem);
                this.#selector.selectedItem = null;
            }
        };
        this.#observer = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                for (const removed of mutation.removedNodes) {
                    this.#remove(removed);
                }
                for (const added of mutation.addedNodes) {
                    this.#add(added);
                }
            }
        });
        this.#start();
    }
    connectedCallback() {
        if (!this.#template.isConnected) {
            this.append(this.#template);
            this.#add(this.querySelector("[slot='selector']"));
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = newValue && fieldGetter(newValue) || identityString;
                break;
            case "labelfield":
                this.labelField = newValue && fieldGetter(newValue) || identityString;
                break;
            case "sortkeyfield":
                this.sortKeyField = newValue && compareBy(fieldGetter(newValue)) || null;
                break;
            case "disabled":
                if (this.#selector) {
                    this.#selector.disabled = this.disabled;
                }
                for (const button of this.querySelectorAll("button")) {
                    button.disabled = this.disabled;
                }
                break;
            case "readonly":
                if (this.#selector) {
                    this.#selector.readOnly = this.readOnly;
                }
                break;
            case "placeholder":
                if (this.#selector) {
                    this.#selector.placeholder = this.placeholder;
                }
                break;
            default:
                this.#render();
                break;
        }
    }
    #start() {
        this.#observer.observe(this, { childList: true });
    }
    #stop() {
        this.#observer.disconnect();
    }
    #add(added) {
        if (added instanceof Element && added.slot === "selector") {
            this.#stop();
            this.#selector = added;
            IKEA(this);
            this.#selector.addEventListener("input", this.#onItemSelected);
            if (this.data.length === 0 && this.#selector.data.length > 0) {
                arrayReplace(this.#data, this.#selector.data);
            }
            if (!this.placeholder && this.#selector.placeholder) {
                this.setAttribute("placeholder", this.#selector.placeholder);
            }
            if (this.#labelField === identityString) {
                this.#labelField = this.#selector.labelField;
            }
            if (this.#valueField === identityString) {
                this.#valueField = this.#selector.valueField;
            }
            this.#sortKeyField ??= this.#selector.sortKeyField;
            this.#selector.labelField = this.#labelField;
            this.#selector.valueField = this.#valueField;
            this.#selector.sortKeyField = this.#sortKeyField;
            this.#selector.placeholder = this.placeholder;
            this.#array.labelField = this.#labelField;
            this.#array.sortKeyField = this.#sortKeyField;
            this.#render();
            this.#start();
        }
    }
    #remove(removed) {
        if (this.#selector && removed === this.#selector) {
            this.#selector.removeEventListener("input", this.#onItemSelected);
            this.#selector = null;
        }
    }
    get placeholder() { return this.getAttribute("placeholder"); }
    set placeholder(v) {
        if (isDefined) {
            this.setAttribute("placeholder", v);
        }
        else {
            this.removeAttribute("placeholder");
        }
    }
    #labelField = identityString;
    get labelField() { return this.#labelField; }
    set labelField(v) {
        this.#labelField = v || identityString;
        if (this.#selector) {
            this.#selector.labelField = this.#labelField;
        }
        this.#render();
    }
    #valueField = identityString;
    get valueField() { return this.#valueField; }
    set valueField(v) {
        this.#valueField = v || identityString;
        if (this.#selector) {
            this.#selector.valueField = this.#valueField;
        }
        this.#render();
    }
    #sortKeyField = null;
    get sortKeyField() { return this.#sortKeyField; }
    set sortKeyField(v) {
        this.#sortKeyField = v;
        if (this.#selector) {
            this.#selector.sortKeyField = this.#sortKeyField;
        }
        this.#render();
    }
    get allowDuplicates() { return this.hasAttribute("allowduplicates"); }
    set allowDuplicates(v) { this.toggleAttribute("allowduplicates", v); }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }
    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(v) { this.toggleAttribute("readonly", v); }
    get selectedItem() { return this.#array.selectedItem; }
    #addItem(item) {
        if (this.values.indexOf(item) === -1 || this.allowDuplicates) {
            this.#array.addItem(item);
        }
        this.#render();
    }
    #removeItem(item) {
        this.#array.removeItem(item);
        this.#render();
    }
    clear() {
        this.#array.clear();
        this.#render();
    }
    #render() {
        if (this.#selector && this.#array && this.#array.values) {
            if (this.allowDuplicates) {
                this.#selector.data = this.#data;
            }
            else {
                const valueIDs = this.#array.values.map(v => this.#valueField(v));
                this.#selector.data = this.#data.filter(v => valueIDs.indexOf(this.#valueField(v)) === -1);
                if (JSON.stringify(this.#initvalues) !== JSON.stringify(this.#array.values)) {
                    this.#saveLabel.style.visibility = "visible";
                }
                else {
                    this.#saveLabel.style.visibility = "hidden";
                }
            }
        }
    }
    static install() {
        return singleton("Juniper::Widgets::ArrayEditorElement", () => registerFactory("array-editor", ArrayEditorElement));
    }
}
export function ArrayEditor(...rest) {
    return ArrayEditorElement.install()(...rest);
}
//# sourceMappingURL=ArrayEditorElement.js.map