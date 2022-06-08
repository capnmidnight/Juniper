import { value } from "@juniper-lib/dom/attrs";
import type { ErsatzElement } from "@juniper-lib/dom/tags";
import { elementClearChildren, Option } from "@juniper-lib/dom/tags";
import { arraySortByKey, isNullOrUndefined, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import type { makeItemCallback } from "./SelectBox";
import { withDefault } from "./SelectBox";

export class SelectListItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    constructor(public item: T) {
        super("itemselected");
    }
}

interface SelectListEvents<T> {
    itemselected: SelectListItemSelectedEvent<T>;
}

/**
 * A select box that can be databound to collections.
 **/
export class SelectList<T>
    extends TypedEventBase<SelectListEvents<T>>
    implements ErsatzElement {

    private makeID: makeItemCallback<T>;
    private makeLabel: makeItemCallback<T>;
    private getSortKey: makeItemCallback<T>;

    private itemToOption = new Map<string, HTMLOptionElement>();
    private optionToItem = new Map<HTMLOptionElement, T>();

    private _emptySelectionEnabled = false;
    private _values: readonly T[] = null;

    private noSelection: HTMLOptionElement;

    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     */
    constructor(element: HTMLSelectElement, makeID: makeItemCallback<T>, makeLabel: makeItemCallback<T>, getSortKey: makeItemCallback<T>);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(element: HTMLSelectElement, makeID: makeItemCallback<T>, makeLabel: makeItemCallback<T>, getSortKey: makeItemCallback<T>, noSelectionText: string);
    constructor(public readonly element: HTMLSelectElement, makeID: makeItemCallback<T>, makeLabel: makeItemCallback<T>, getSortKey: makeItemCallback<T>, noSelectionText?: string) {
        super();

        this.makeID = withDefault(makeID);
        this.makeLabel = withDefault(makeLabel, "None");
        this.getSortKey = withDefault(getSortKey);

        this.noSelection = Option(noSelectionText);

        this.emptySelectionEnabled = !isNullOrUndefined(noSelectionText);

        this.element.addEventListener("input", () =>
            this.dispatchEvent(new SelectListItemSelectedEvent(this.selectedValue)));

        Object.seal(this);
    }

    get enabled(): boolean {
        return !this.element.disabled;
    }

    set enabled(v: boolean) {
        this.element.disabled = !v;
    }

    get count() {
        return this._values && this._values.length || 0;
    }

    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get emptySelectionEnabled(): boolean {
        return this._emptySelectionEnabled;
    }

    /**
     * Sets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    set emptySelectionEnabled(value: boolean) {
        if (value !== this.emptySelectionEnabled) {
            this._emptySelectionEnabled = value;
            this.render();
        }
    }

    /**
     * Gets the collection to which the select box was databound
     **/
    get values(): readonly T[] {
        return this._values || [];
    }

    /**
     * Sets the collection to which the select box will be databound
     **/
    set values(newItems: readonly T[]) {
        newItems = newItems || null;
        if (newItems !== this._values) {
            const curValue = this.selectedValue;
            this._values = newItems;
            this.render();
            this.selectedValue = curValue;
        }
    }

    private get selectedOption(): HTMLOptionElement {
        return this.element.selectedOptions.item(0);
    }

    private set selectedOption(option: HTMLOptionElement) {
        for (let i = 0; i < this.element.options.length; ++i) {
            const here = this.element.options[i];
            here.selected = here === option;
        }
    }

    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedValue(): T {
        return this.optionToItem.get(this.selectedOption);
    }

    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value: T) {
        this.selectedOption = this.itemToOption.get(this.makeID(value));
    }

    refresh() {
        this.render();
    }


    private render() {
        elementClearChildren(this);
        this.itemToOption.clear();
        this.optionToItem.clear();

        if (this.count === 0 || this.emptySelectionEnabled) {
            this.append(null, this.noSelection);
        }

        if (this.count > 0) {
            const sortedItems = arraySortByKey(this.values, this.getSortKey);
            for (let item of sortedItems) {
                const option = this.makeOption(item);
                this.append(item, option);
            }
        }
    }

    private makeOption(item: T): HTMLOptionElement {
        const option = Option(
            value(this.makeID(item)),
            this.makeLabel(item));
        this.itemToOption.set(this.makeID(item), option);
        this.optionToItem.set(option, item);
        return option;
    }

    private append(value: T, option: HTMLOptionElement): void {
        this.element.append(option);
        this.itemToOption.set(this.makeID(value), option);
        this.optionToItem.set(option, value);
    }
}