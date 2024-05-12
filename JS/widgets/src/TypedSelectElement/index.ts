import { arrayCompare, arrayReplace, compareBy, compareCallback, hasValue, isNullOrUndefined } from "@juniper-lib/util";
import { ElementChild, HtmlAttr, HtmlTag, Is, Option, TypedHTMLSelectElement, Value } from "@juniper-lib/dom";
import { IArrayElementSelector } from "../ArrayElementSelector";
import { fieldGetter, identityString, makeItemCallback } from "../FieldDef";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { SelectPlaceholder } from "../widgets";

type TypedSelectEvents<T> = {
    "input": InputEvent;
    "itemselected": TypedItemSelectedEvent<T>;
};

export function Nullable(value: boolean) {
    return new HtmlAttr("nullable", value, false, "typed-select");
}

export function TypedSelect<D extends HtmlAttr<"data", T>, T>(data: D, ...rest: ElementChild[]): TypedSelectElement<T>;
export function TypedSelect<T>(...rest: ElementChild[]): TypedSelectElement<T>;
export function TypedSelect<T>(...rest: ElementChild[]): TypedSelectElement<T> {
    return HtmlTag("select", Is("typed-select"), ...rest) as TypedSelectElement<T>;
}

/**
 * A select box that can be databound to collections.
 **/
export class TypedSelectElement<T> extends TypedHTMLSelectElement<TypedSelectEvents<T>> implements IArrayElementSelector<T> {

    static observedAttributes = [
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "placeholder",
        "nullable"
    ];

    readonly #valueToOption = new Map<string, HTMLOptionElement>();
    readonly #optionToItem = new Map<HTMLOptionElement, T>();
    readonly #values: T[] = [];

    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();

        this.addEventListener("input", () => {
            this.#resolveItem();
            this.dispatchEvent(new TypedItemSelectedEvent(this.selectedItem, this.selectedItems));
        });
    }

    connectedCallback() {
        this.#render();
    }

    attributeChangedCallback(name: string) {
        const value = this.getAttribute(name);
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = value && fieldGetter(value) || identityString;
                break;
            case "labelfield":
                this.labelField = value && fieldGetter(value) || identityString;
                break;
            case "sortkeyfield":
                this.sortKeyField = value && compareBy(fieldGetter(value)) || null;
                break;
            case "nullable":
                this.#refreshPlaceholder(this.#noSelectionText);
                break;
            case "placeholder":
                this.#refreshPlaceholder(value);
                break;
        }
    }

    #refreshPlaceholder(value: string) {
        this.#noSelectionText = value;
        if (this.#noSelectionText) {
            this.#noSelection = SelectPlaceholder(value, this.nullable);
        }
        else {
            this.#noSelection = null;
        }
        this.#render();
    }

    #getValue: makeItemCallback<T> = identityString;
    get valueField(): makeItemCallback<T> { return this.#getValue; }
    set valueField(v: makeItemCallback<T>) {
        this.#getValue = v || identityString;
        this.#render();
    }

    #getLabel: makeItemCallback<T> = identityString;
    get labelField(): makeItemCallback<T> { return this.#getLabel; }
    set labelField(v: makeItemCallback<T>) {
        this.#getLabel = v || identityString;
        this.#render();
    }

    #getSortKey: compareCallback<T> = null;
    get sortKeyField(): compareCallback<T> { return this.#getSortKey; }
    set sortKeyField(v: compareCallback<T>) {
        this.#getSortKey = v;
        this.#render();
    }

    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get #emptySelectionEnabled(): boolean { return hasValue(this.#noSelection); }
    #noSelection: HTMLOptionElement = null;
    #noSelectionText: string = null;
    get placeholder(): string { return this.#noSelectionText; }
    set placeholder(v: string) {
        if (v) {
            this.setAttribute("placeholder", v);
        }
        else {
            this.removeAttribute("placeholder");
        }
    }

    get nullable() { return this.hasAttribute("nullable"); }
    set nullable(v) {
        if (v) {
            this.setAttribute("nullable", "");
        }
        else {
            this.removeAttribute("nullable");
        }
    }

    get enabled(): boolean { return !this.disabled; }
    set enabled(v: boolean) { this.disabled = !v; }

    get count() { return this.#values.length; }

    get data(): readonly T[] { return this.#values; }
    set data(newItems: readonly T[]) {
        newItems = newItems || [];
        if (arrayCompare(this.#values, newItems) !== -1) {
            const curValue = this.value;
            arrayReplace(this.#values, newItems);
            this.#render();
            this.value = curValue;
            this.#resolveItem();
        }
    }

    get #selectedOption(): HTMLOptionElement { return this.selectedOptions.item(0); }
    set #selectedOption(option: HTMLOptionElement) {
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

    set values(values: string[]) {
        const v = new Set(values);
        for (const option of this.options) {
            option.selected = v.has(option.value || option.innerHTML);
        }
    }

    #selectedItem: T = null;

    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedItem(): T {
        if (!this.#selectedItem) {
            this.#resolveItem();
        }
        return this.#selectedItem;
    }

    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedItem(item: T) {
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

    get selectedItems(): T[] {
        return Array.from(this.selectedOptions)
            .map(opt => this.#optionToItem.get(opt));
    }

    set selectedItems(values: T[]) {
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
                const option = Option(
                    Value(this.#makeValue(item)),
                    this.#makeLabel(item)
                );
                this.#mapOption(item, option);
            }
        }
    }

    #mapOption(value: T, option: HTMLOptionElement): void {
        this.append(option);
        this.#valueToOption.set(this.#makeValue(value), option);
        this.#optionToItem.set(option, value);
    }
}

customElements.define("typed-select", TypedSelectElement, { extends: "select" })