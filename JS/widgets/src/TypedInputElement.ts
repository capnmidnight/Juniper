import { arrayCompare, arrayReplace, compareBy, compareCallback, isArray, singleton } from "@juniper-lib/util";
import { DataList, ElementChild, ListAttr, Option, registerFactory, TypedHTMLInputElement, Value } from "@juniper-lib/dom";
import { ITypedElementSelector } from "./ArrayElementSelector";
import { DataAttr, fieldGetter, identityString, makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";

type TypedInputEvents<T> = {
    "input": InputEvent;
    "itemselected": TypedItemSelectedEvent<T>;
};

/**
 * A select box that can be databound to collections.
 **/
export class TypedInputElement<T> extends TypedHTMLInputElement<TypedInputEvents<T>> implements ITypedElementSelector<T> {

    static override observedAttributes = [
        ...super.observedAttributes,
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "value"
    ];

    readonly #valueToOption = new Map<string, HTMLOptionElement>();
    readonly #optionToItem = new Map<HTMLOptionElement, T>();
    readonly #values: T[] = [];
    readonly #dataList: HTMLDataListElement;
    override get list() {
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

    override connectedCallback() {
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



    override attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

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

    override get placeholder() { return super.placeholder; }
    override set placeholder(v) { super.placeholder = v || ""; }


    #valueField: makeItemCallback<T> = identityString;
    get valueField(): makeItemCallback<T> { return this.#valueField; }
    set valueField(v: makeItemCallback<T>) {
        if (v !== this.valueField) {
            super.removeAttribute("valuefield");
            this.#valueField = v || identityString;
            this.#render();
        }
    }

    #labelField: makeItemCallback<T> = identityString;
    get labelField(): makeItemCallback<T> { return this.#labelField; }
    set labelField(v: makeItemCallback<T>) {
        if (v !== this.labelField) {
            super.removeAttribute("labelfield");
            this.#labelField = v || identityString;
            this.#render();
        }
    }

    #sortKeyField: compareCallback<T> = null;
    get sortKeyField(): compareCallback<T> { return this.#sortKeyField; }
    set sortKeyField(v: compareCallback<T>) {
        if (v !== this.sortKeyField) {
            this.removeAttribute("sortkeyfield");
            this.#sortKeyField = v;
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

    #mapOption(item: T): void {
        const value = this.#makeValue(item);
        const option = Option(
            Value(value),
            this.#makeLabel(item)
        );
        this.list.append(option);
        this.#valueToOption.set(value, option);
        this.#optionToItem.set(option, item);
    }

    static install() {
        return singleton("Juniper::Widgets::TypedInputElement", () => registerFactory("typed-input", TypedInputElement, { extends: "input" }));
    }
}

export function TypedInput<T>(...rest: ElementChild<TypedInputElement<T>>[]): TypedInputElement<T>;
export function TypedInput<T>(data: T[], ...rest: ElementChild<TypedInputElement<T>>[]): TypedInputElement<T>;
export function TypedInput<T>(dataOrFirstChild: T[] | ElementChild<TypedInputElement<T>>, ...rest: ElementChild<TypedInputElement<T>>[]): TypedInputElement<T> {
    if (isArray(dataOrFirstChild)) {
        rest.unshift(DataAttr(dataOrFirstChild));
    }
    else {
        rest.unshift(dataOrFirstChild);
    }

    return (TypedInputElement.install() as any)(...rest) as TypedInputElement<T>;
}
