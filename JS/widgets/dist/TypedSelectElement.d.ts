import { compareCallback } from "@juniper-lib/util";
import { ElementChild, HtmlProp, TypedHTMLSelectElement } from "@juniper-lib/dom";
import { ITypedElementSelector } from "./ArrayElementSelector";
import { makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
type TypedSelectEvents<T> = {
    "input": InputEvent;
    "itemselected": TypedItemSelectedEvent<T>;
};
export declare function Nullable(value: boolean): HtmlProp<"nullable", boolean, Node & Record<"nullable", boolean>>;
/**
 * A select box that can be databound to collections.
 **/
export declare class TypedSelectElement<T> extends TypedHTMLSelectElement<TypedSelectEvents<T>> implements ITypedElementSelector<T> {
    #private;
    static observedAttributes: string[];
    /**
     * Creates a select box that can bind to collections
     */
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get valueField(): makeItemCallback<T>;
    set valueField(v: makeItemCallback<T>);
    get labelField(): makeItemCallback<T>;
    set labelField(v: makeItemCallback<T>);
    get sortKeyField(): compareCallback<T>;
    set sortKeyField(v: compareCallback<T>);
    get placeholder(): string;
    set placeholder(v: string);
    get nullable(): boolean;
    set nullable(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    get count(): number;
    get data(): readonly T[];
    set data(newItems: readonly T[]);
    get readOnly(): boolean;
    set readOnly(v: boolean);
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
    set selectedItem(item: T);
    get selectedItems(): T[];
    set selectedItems(values: T[]);
    static install(): import("@juniper-lib/dom").ElementFactory<TypedSelectElement<unknown>>;
}
export declare function TypedSelect<D extends HtmlProp<"data", T>, T>(data: D, ...rest: ElementChild<TypedSelectElement<T>>[]): TypedSelectElement<T>;
export declare function TypedSelect<T>(...rest: ElementChild<TypedSelectElement<T>>[]): TypedSelectElement<T>;
export {};
//# sourceMappingURL=TypedSelectElement.d.ts.map