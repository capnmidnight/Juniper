import { ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import "./styles.css";
/**
 * Creates a string from a list item to use as the item's ID or label in a select box.
 */
type readItemCallback<T, V> = (obj: T) => V;
export type makeItemCallback<T> = readItemCallback<T, string>;
export type makeLabelCallback<T> = readItemCallback<T, ElementChild>;
export declare class SelectBoxItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    item: T;
    constructor(item: T);
}
type SelectBoxEvents<T> = {
    itemselected: SelectBoxItemSelectedEvent<T>;
};
export declare function withDefault<T, V>(callback: readItemCallback<T, V>, defaultValue?: V): readItemCallback<T, V>;
declare class SelectBoxRow<T> extends TypedEventTarget<SelectBoxEvents<T>> implements ErsatzElement {
    id: string;
    private _value;
    private _contents;
    private _sortKey;
    private _element;
    get element(): HTMLElement;
    constructor(value: T, id: string, contents: ElementChild, sortKey: string);
    get value(): T;
    get sortKey(): string;
    get contents(): ElementChild;
    set contents(v: ElementChild);
    get selected(): boolean;
    set selected(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
}
/**
 * A select box that can be databound to collections.
 **/
export declare class SelectBox<T> extends TypedEventTarget<SelectBoxEvents<T>> implements ErsatzElement {
    private makeID;
    private makeLabel;
    private getSortKey;
    private itemToOption;
    private _enabled;
    private _emptySelectionEnabled;
    private _values;
    private _element;
    get element(): HTMLElement;
    private elementRows;
    private noSelection;
    options: SelectBoxRow<T>[];
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(makeID: makeItemCallback<T>, makeLabel: makeLabelCallback<T>, getSortKey: makeItemCallback<T>, noSelectionText: string);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(makeID: makeItemCallback<T>, makeLabel: makeLabelCallback<T>, getSortKey: makeItemCallback<T>);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(element: HTMLSelectElement, makeID: makeItemCallback<T>, makeLabel: makeLabelCallback<T>, getSortKey: makeItemCallback<T>, noSelectionText: string);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(element: HTMLSelectElement, makeID: makeItemCallback<T>, makeLabel: makeLabelCallback<T>, getSortKey: makeItemCallback<T>);
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
    append(item: T): void;
    remove(item: T): boolean;
    has(id: string): boolean;
    get(id: string): T;
    removeByKey(id: string): T;
    refresh(): void;
    private render;
    private makeOption;
    private appendOption;
    private addRow;
    private removeRow;
}
export {};
//# sourceMappingURL=index.d.ts.map