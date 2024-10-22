import { compareCallback } from "@juniper-lib/util";
import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
type InclusionListEvents<T> = {
    "itemselected": TypedItemSelectedEvent<T>;
};
export declare class InclusionListElement<T> extends TypedHTMLElement<InclusionListEvents<T>> {
    #private;
    constructor();
    connectedCallback(): void;
    setAttribute(name: string, value: string): void;
    removeAttribute(name: string): void;
    get valueField(): makeItemCallback<T>;
    set valueField(v: makeItemCallback<T>);
    get labelField(): makeItemCallback<T>;
    set labelField(v: makeItemCallback<T>);
    get sortKeyField(): compareCallback<T>;
    set sortKeyField(v: compareCallback<T>);
    /**
     * Gets the collection to which the select box was databound
     **/
    get data(): readonly T[];
    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems: readonly T[]);
    get selectedItems(): T[];
    set selectedItems(values: readonly T[]);
    get filter(): string;
    set filter(v: string);
    get disabled(): boolean;
    set disabled(v: boolean);
    get name(): string;
    set name(v: string);
    get required(): boolean;
    set required(v: boolean);
    get size(): number;
    set size(v: number);
    get value(): string;
    set value(v: string);
    get form(): HTMLFormElement;
    get length(): number;
    get multiple(): boolean;
    get options(): HTMLOptionsCollection;
    get type(): "select-one" | "select-multiple";
    get validationMessage(): string;
    get validity(): ValidityState;
    get willValidate(): boolean;
    static install(): import("@juniper-lib/dom").ElementFactory<InclusionListElement<unknown>>;
}
export declare function InclusionList<T>(...rest: ElementChild<InclusionListElement<T>>[]): InclusionListElement<T>;
export {};
//# sourceMappingURL=InclusionListElement.d.ts.map