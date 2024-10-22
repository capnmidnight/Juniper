import { arrayCompare, arrayReplace, compareBy, isArray, singleton } from "@juniper-lib/util";
import { DataList, ListAttr, Option, registerFactory, TypedHTMLInputElement, Value } from "@juniper-lib/dom";
import { DataAttr, fieldGetter, identityString } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
/**
 * A select box that can be databound to collections.
 **/
export class TypedInputElement extends TypedHTMLInputElement {
    static { this.observedAttributes = [
        ...super.observedAttributes,
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "value"
    ]; }
    #valueToOption = new Map();
    #optionToItem = new Map();
    #values = [];
    #dataList;
    get list() {
        if (!super.list) {
            ListAttr(this.#dataList).apply(this);
        }
        return this.#dataList;
    }
    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();
        this.#dataList = DataList();
        super.addEventListener("change", () => {
            this.#resolveItem();
            this.dispatchEvent(new TypedItemSelectedEvent(this.selectedItem));
        });
    }
    connectedCallback() {
        super.connectedCallback();
        for (const name of this.getAttributeNames()) {
            this.setAttribute(name, this.getAttribute(name));
        }
        this.#render();
    }
    disconnectedCallback() {
        if (this.list.isConnected) {
            this.list.remove();
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
            case "value":
                this.#resolveItem();
                break;
            default:
                super.attributeChangedCallback(name, oldValue, newValue);
                break;
        }
    }
    get placeholder() { return super.placeholder; }
    set placeholder(v) { super.placeholder = v || ""; }
    #valueField = identityString;
    get valueField() { return this.#valueField; }
    set valueField(v) {
        if (v !== this.valueField) {
            super.removeAttribute("valuefield");
            this.#valueField = v || identityString;
            this.#render();
        }
    }
    #labelField = identityString;
    get labelField() { return this.#labelField; }
    set labelField(v) {
        if (v !== this.labelField) {
            super.removeAttribute("labelfield");
            this.#labelField = v || identityString;
            this.#render();
        }
    }
    #sortKeyField = null;
    get sortKeyField() { return this.#sortKeyField; }
    set sortKeyField(v) {
        if (v !== this.sortKeyField) {
            this.removeAttribute("sortkeyfield");
            this.#sortKeyField = v;
            this.#render();
        }
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
    get count() {
        return this.#values && this.#values.length || 0;
    }
    /**
     * Gets the collection to which the select box was databound
     **/
    get data() {
        return this.#values;
    }
    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems) {
        newItems = newItems || null;
        if (arrayCompare(this.#values, newItems) !== -1) {
            const curValue = this.value;
            arrayReplace(this.#values, newItems);
            this.#render();
            this.value = curValue;
            this.#resolveItem();
        }
    }
    get #makeValue() { return this.valueField || this.labelField; }
    get #makeLabel() { return this.labelField || this.valueField; }
    #selectedItem = null;
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedItem() {
        if (!this.#selectedItem) {
            this.#resolveItem();
        }
        return this.#selectedItem;
    }
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedItem(item) {
        this.#selectedItem = item;
        if (this.#makeValue) {
            this.value = this.#makeValue(item) || "";
        }
    }
    #resolveItem() {
        if (this.#values.length > 0) {
            const option = this.#valueToOption.get(this.value);
            this.#selectedItem = this.#optionToItem.get(option);
            if (!this.#selectedItem && document.activeElement !== this) {
                this.value = "";
            }
        }
    }
    #render() {
        this.list.innerHTML = "";
        this.#valueToOption.clear();
        this.#optionToItem.clear();
        if (this.#makeValue && this.count > 0) {
            const items = [...this.data];
            if (this.sortKeyField) {
                items.sort(this.sortKeyField);
            }
            for (const item of items) {
                this.#mapOption(item);
            }
        }
    }
    #mapOption(item) {
        const value = this.#makeValue(item);
        const option = Option(Value(value), this.#makeLabel(item));
        this.list.append(option);
        this.#valueToOption.set(value, option);
        this.#optionToItem.set(option, item);
    }
    static install() {
        return singleton("Juniper::Widgets::TypedInputElement", () => registerFactory("typed-input", TypedInputElement, { extends: "input" }));
    }
}
export function TypedInput(dataOrFirstChild, ...rest) {
    if (isArray(dataOrFirstChild)) {
        rest.unshift(DataAttr(dataOrFirstChild));
    }
    else {
        rest.unshift(dataOrFirstChild);
    }
    return TypedInputElement.install()(...rest);
}
//# sourceMappingURL=TypedInputElement.js.map