import { arrayCompare, arrayReplace, compareBy, compareCallback, isArray } from "@juniper-lib/util";
import { DataList, ElementChild, HtmlTag, Is, List, Option, TypedHTMLInputElement, Value } from "@juniper-lib/dom";
import { DataAttr, fieldGetter, identityString, makeItemCallback } from "../FieldDef";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { IArrayElementSelector } from "../ArrayElementSelector";

type TypedInputEvents<T> = {
    "input": InputEvent;
    "itemselected": TypedItemSelectedEvent<T>;
};

export function TypedInput<T>(...rest: ElementChild[]): TypedInputElement<T>;
export function TypedInput<T>(data: T[], ...rest: ElementChild[]): TypedInputElement<T>;
export function TypedInput<T>(dataOrFirstChild: T[] | ElementChild, ...rest: ElementChild[]): TypedInputElement<T> {

    if (isArray(dataOrFirstChild)) {
        rest.unshift(DataAttr(dataOrFirstChild));
    }
    else {
        rest.unshift(dataOrFirstChild);
    }

    return HtmlTag(
        "input",
        Is("typed-input"),
        ...rest
    ) as TypedInputElement<T>;
}

/**
 * A select box that can be databound to collections.
 **/
export class TypedInputElement<T> extends TypedHTMLInputElement<TypedInputEvents<T>> implements IArrayElementSelector<T> {

    readonly #dataList: HTMLDataListElement;

    readonly #valueToOption = new Map<string, HTMLOptionElement>();
    readonly #optionToItem = new Map<HTMLOptionElement, T>();
    readonly #values: T[] = [];

    #getValue: makeItemCallback<T> = identityString;
    #getLabel: makeItemCallback<T> = identityString;
    #getSortKey: compareCallback<T> = null;

    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();

        this.#dataList = DataList();
        
        super.addEventListener("change", () => {
            this.#resolveItem();
            this.dispatchEvent(new TypedItemSelectedEvent(this.selectedItem, [this.selectedItem]));
        });
    }

    connectedCallback() {
        if (!this.#dataList.isConnected) {
            List(this.#dataList).apply(this);
        }

        for (const name of this.getAttributeNames()) {
            this.setAttribute(name, this.getAttribute(name));
        }

        this.#render();
    }

    override setAttribute(name: string, value: string) {
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = fieldGetter(value);
                break;
            case "labelfield":
                this.labelField = fieldGetter(value);
                break;
            case "sortkeyfield":
                this.sortKeyField = compareBy(fieldGetter(value));
                break;
            default: super.setAttribute(name, value);
        }
    }

    override removeAttribute(name: string) {
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = null;
                break;
            case "labelfield":
                this.labelField = null;
                break;
            case "sortkeyfield":
                this.sortKeyField = null;
                break;
            default: super.removeAttribute(name);
        }
    }

    override get placeholder() { return super.placeholder; }
    override set placeholder(v) { super.placeholder = v || ""; }

    get valueField(): makeItemCallback<T> {
        return this.#getValue;
    }

    set valueField(v: makeItemCallback<T>) {
        if (v !== this.valueField) {
            super.removeAttribute("getValue");
            this.#getValue = v || identityString;
            this.#render();
        }
    }

    get labelField(): makeItemCallback<T> {
        return this.#getLabel;
    }

    set labelField(v: makeItemCallback<T>) {
        if (v !== this.labelField) {
            super.removeAttribute("getLabel");
            this.#getLabel = v || identityString;
            this.#render();
        }
    }

    get sortKeyField(): compareCallback<T> {
        return this.#getSortKey;
    }

    set sortKeyField(v: compareCallback<T>) {
        if (v !== this.sortKeyField) {
            this.removeAttribute("getSortKey");
            this.#getSortKey = v;
            this.#render();
        }
    }

    get enabled(): boolean {
        return !this.disabled;
    }

    set enabled(v: boolean) {
        this.disabled = !v;
    }

    get count() {
        return this.#values && this.#values.length || 0;
    }

    /**
     * Gets the collection to which the select box was databound
     **/
    get data(): readonly T[] {
        return this.#values;
    }

    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems: readonly T[]) {
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
            const option = this.#valueToOption.get(this.value);
            this.#selectedItem = this.#optionToItem.get(option);
            if (!this.#selectedItem && document.activeElement !== this) {
                this.value = "";
            }
        }
    }

    #render() {
        this.#dataList.innerHTML = "";
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

    #mapOption(item: T): void {
        const value = this.#makeValue(item);
        const option = Option(
            Value(value),
            this.#makeLabel(item)
        );
        this.#dataList.append(option);
        this.#valueToOption.set(value, option);
        this.#optionToItem.set(option, item);
    }
}

customElements.define("typed-input", TypedInputElement, { extends: "input" });