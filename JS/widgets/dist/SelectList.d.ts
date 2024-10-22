import { CompareFunction } from "@juniper-lib/collections/dist/arrays";
import { HtmlAttr } from "@juniper-lib/dom/dist/attrs";
import { HtmlEvt } from "@juniper-lib/dom/dist/evts";
import { ElementChild } from "@juniper-lib/dom/dist/tags";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/dist/TypedEventTarget";
import type { makeItemCallback } from "./SelectBox";
export declare class SelectListItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    readonly item: T;
    readonly items: T[];
    constructor(item: T, items: T[]);
}
type SelectListEvents<T> = {
    "input": InputEvent;
    "itemselected": SelectListItemSelectedEvent<T>;
};
export declare function SelectList<T>(...rest: ElementChild[]): SelectListElement<T>;
export declare function ValueField<T>(fieldName: string | makeItemCallback<T>): HtmlAttr<string, makeItemCallback<T>>;
export declare function LabelField<T>(fieldName: string | makeItemCallback<T>): HtmlAttr<string, makeItemCallback<T>>;
export declare function SortKeyField<T>(fieldName: string | makeItemCallback<T>): HtmlAttr<string, makeItemCallback<T>>;
export declare function DataAttr<T>(values: T[]): HtmlAttr<"data", T[]>;
export declare function SelectedItem<T>(value: T): HtmlAttr<"selectedItem", T>;
export declare function onItemSelected<T>(callback: (evt: SelectListItemSelectedEvent<T>) => void, opts?: EventListenerOptions): HtmlEvt<SelectListItemSelectedEvent<T>>;
/**
 * A select box that can be databound to collections.
 **/
export declare class SelectListElement<T> extends HTMLSelectElement implements ITypedEventTarget<SelectListEvents<T>> {
    private readonly eventTarget;
    private readonly valueToOption;
    private readonly optionToItem;
    private readonly _values;
    private _getValue;
    private _getLabel;
    private _getSortKey;
    private noSelection;
    /**
     * Creates a select box that can bind to collections
     */
    constructor();
    connectedCallback(): void;
    setAttribute(name: string, value: string): void;
    removeAttribute(name: string): void;
    get valueField(): makeItemCallback<T>;
    set valueField(v: makeItemCallback<T>);
    get labelField(): makeItemCallback<T>;
    set labelField(v: makeItemCallback<T>);
    get sortKeyField(): CompareFunction<T>;
    set sortKeyField(v: CompareFunction<T>);
    private noSelectionText;
    get placeholder(): string;
    set placeholder(v: string);
    get enabled(): boolean;
    set enabled(v: boolean);
    get count(): number;
    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    private get emptySelectionEnabled();
    /**
     * Gets the collection to which the select box was databound
     **/
    get data(): readonly T[];
    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems: readonly T[]);
    private get selectedOption();
    private set selectedOption(value);
    private get makeValue();
    private get makeLabel();
    get values(): string[];
    set values(values: string[]);
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedItem(): T;
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedItem(value: T);
    get selectedItems(): T[];
    set selectedItems(values: T[]);
    private render;
    private mapOption;
    addEventListener<EventTypeT extends keyof SelectListEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof SelectListEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: ITypedEventTarget<SelectListEvents<T>>): void;
    removeBubbler(bubbler: ITypedEventTarget<SelectListEvents<T>>): void;
    addScopedEventListener<EventTypeT extends keyof SelectListEvents<T>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners<EventTypeT extends keyof SelectListEvents<T>>(type?: EventTypeT): void;
}
export {};
//# sourceMappingURL=SelectList.d.ts.map