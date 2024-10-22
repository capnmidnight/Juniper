import { arrayCompare, arrayReplace, compareBy, isDefined, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { HtmlProp, Option, registerFactory, TypedHTMLSelectElement, Value } from "@juniper-lib/dom";
import { fieldGetter, identityString } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
import { SelectPlaceholder } from "./widgets";
export function Nullable(value) {
    return new HtmlProp("nullable", value);
}
/**
 * A select box that can be databound to collections.
 **/
export class TypedSelectElement extends TypedHTMLSelectElement {
    static { this.observedAttributes = [
        ...super.observedAttributes,
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "placeholder",
        "nullable"
    ]; }
    #valueToOption = new Map();
    #optionToItem = new Map();
    #values = [];
    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();
        this.addEventListener("input", () => {
            this.#resolveItem();
            this.dispatchEvent(new TypedItemSelectedEvent(this.selectedItems));
        });
    }
    connectedCallback() {
        super.connectedCallback();
        this.#render();
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
            case "nullable":
                this.#refreshPlaceholder(this.#noSelectionText);
                break;
            case "placeholder":
                this.#refreshPlaceholder(newValue);
                break;
            default:
                super.attributeChangedCallback(name, oldValue, newValue);
                break;
        }
    }
    #refreshPlaceholder(value) {
        this.#noSelectionText = value;
        if (this.#noSelectionText) {
            this.#noSelection = SelectPlaceholder(value, this.nullable);
        }
        else {
            this.#noSelection = null;
        }
        this.#render();
    }
    #valueField = identityString;
    get valueField() { return this.#valueField; }
    set valueField(v) {
        this.#valueField = v || identityString;
        this.#render();
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
    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get #emptySelectionEnabled() { return isDefined(this.#noSelection); }
    #noSelection = null;
    #noSelectionText = null;
    get placeholder() { return this.#noSelectionText; }
    set placeholder(v) {
        if (isDefined(v)) {
            this.setAttribute("placeholder", v);
        }
        else {
            this.removeAttribute("placeholder");
        }
    }
    get nullable() { return this.hasAttribute("nullable"); }
    set nullable(v) { this.toggleAttribute("nullable", v); }
    get enabled() { return !this.disabled; }
    set enabled(v) { this.disabled = !v; }
    get count() { return this.#values.length; }
    get data() { return this.#values; }
    set data(newItems) {
        newItems = newItems || [];
        if (arrayCompare(this.#values, newItems) !== -1) {
            const curValue = this.value;
            arrayReplace(this.#values, newItems);
            this.#render();
            this.value = curValue;
            this.#resolveItem();
        }
    }
    #readOnly = false;
    get readOnly() {
        return this.#readOnly;
    }
    set readOnly(v) {
        this.#readOnly = v;
        for (const option of this.options) {
            option.disabled = this.#readOnly;
        }
    }
    get #selectedOption() { return this.selectedOptions.item(0); }
    set #selectedOption(option) {
        for (const option of this.options) {
            option.selected = false;
        }
        if (isNullOrUndefined(option) && this.#noSelection) {
            this.options[0].selected = true;
        }
        else {
            for (let i = 0; i < this.options.length; ++i) {
                const here = this.options[i];
                here.selected = here === option;
            }
        }
    }
    get #makeValue() { return this.valueField || this.labelField; }
    get #makeLabel() { return this.labelField || this.valueField; }
    get values() {
        return Array
            .from(this.selectedOptions)
            .map(opt => opt.value);
    }
    set values(values) {
        const v = new Set(values);
        for (const option of this.options) {
            option.selected = v.has(option.value || option.innerHTML);
        }
    }
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
            this.#resolveItem();
        }
    }
    #resolveItem() {
        if (this.#values.length > 0) {
            this.#selectedOption = this.#valueToOption.get(this.value);
            this.#selectedItem = this.#optionToItem.get(this.#selectedOption);
            if (!this.#selectedItem) {
                this.value = "";
            }
        }
    }
    get selectedItems() {
        return Array.from(this.selectedOptions)
            .map(opt => this.#optionToItem.get(opt));
    }
    set selectedItems(values) {
        this.values = values.map(v => this.#makeValue(v));
    }
    #render() {
        this.innerHTML = "";
        this.#valueToOption.clear();
        this.#optionToItem.clear();
        if (this.count === 0 || this.#emptySelectionEnabled) {
            this.#mapOption(null, this.#noSelection);
        }
        if (this.#makeValue && this.count > 0) {
            const items = [...this.data];
            if (this.sortKeyField) {
                items.sort(this.sortKeyField);
            }
            for (const item of items) {
                const option = Option(Value(this.#makeValue(item)), this.#makeLabel(item));
                this.#mapOption(item, option);
            }
        }
    }
    #mapOption(value, option) {
        this.append(option);
        this.#valueToOption.set(this.#makeValue(value), option);
        this.#optionToItem.set(option, value);
    }
    static install() {
        return singleton("Juniper::Widgets::TypedSelectElement", () => registerFactory("typed-select", TypedSelectElement, { extends: "select" }));
    }
}
export function TypedSelect(...rest) {
    return TypedSelectElement.install()(...rest);
}
//# sourceMappingURL=TypedSelectElement.js.map