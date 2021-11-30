import { TypedEvent, TypedEventBase } from "juniper-tslib";
import type { makeItemCallback } from "./SelectBox";
import type { ErsatzElement } from "./tags";
export declare class SelectListItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    item: T;
    constructor(item: T);
}
interface SelectListEvents<T> {
    itemselected: SelectListItemSelectedEvent<T>;
}
/**
 * A select box that can be databound to collections.
 **/
export declare class SelectList<T> extends TypedEventBase<SelectListEvents<T>> implements ErsatzElement {
    readonly element: HTMLSelectElement;
    private makeID;
    private makeLabel;
    private getSortKey;
    private itemToOption;
    private optionToItem;
    private _emptySelectionEnabled;
    private _values;
    private noSelection;
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
    get enabled(): boolean;
    set enabled(v: boolean);
    get count(): number;
    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get emptySelectionEnabled(): boolean;
    /**
     * Sets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    set emptySelectionEnabled(value: boolean);
    /**
     * Gets the collection to which the select box was databound
     **/
    get values(): readonly T[];
    /**
     * Sets the collection to which the select box will be databound
     **/
    set values(newItems: readonly T[]);
    private get selectedOption();
    private set selectedOption(value);
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedValue(): T;
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value: T);
    refresh(): void;
    private render;
    private makeOption;
    private append;
}
export {};
