import { compareCallback } from "@juniper-lib/util";
import { ElementChild, TypedHTMLInputElement } from "@juniper-lib/dom";
import { ITypedElementSelector } from "./ArrayElementSelector";
import { makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
type TypedInputEvents<T> = {
    "input": InputEvent;
    "itemselected": TypedItemSelectedEvent<T>;
};
/**
 * A select box that can be databound to collections.
 **/
export declare class TypedInputElement<T> extends TypedHTMLInputElement<TypedInputEvents<T>> implements ITypedElementSelector<T> {
    #private;
    static observedAttributes: string[];
    get list(): HTMLDataListElement;
    /**
     * Creates a select box that can bind to collections
     */
    constructor();
    connectedCallback(): void;
    disconnectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get placeholder(): string;
    set placeholder(v: string);
    get valueField(): makeItemCallback<T>;
    set valueField(v: makeItemCallback<T>);
    get labelField(): makeItemCallback<T>;
    set labelField(v: makeItemCallback<T>);
    get sortKeyField(): compareCallback<T>;
    set sortKeyField(v: compareCallback<T>);
    get enabled(): boolean;
    set enabled(v: boolean);
    get count(): number;
    /**
     * Gets the collection to which the select box was databound
     **/
    get data(): readonly T[];
    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems: readonly T[]);
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedItem(): T;
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedItem(item: T);
    static install(): import("@juniper-lib/dom").ElementFactory<TypedInputElement<unknown>>;
}
export declare function TypedInput<T>(...rest: ElementChild<TypedInputElement<T>>[]): TypedInputElement<T>;
export declare function TypedInput<T>(data: T[], ...rest: ElementChild<TypedInputElement<T>>[]): TypedInputElement<T>;
export {};
//# sourceMappingURL=TypedInputElement.d.ts.map