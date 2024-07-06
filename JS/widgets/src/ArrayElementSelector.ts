import { compareCallback } from "@juniper-lib/util";
import { makeItemCallback } from "./FieldDef";

export interface TypedElementSelector<T> extends HTMLElement {
    data: readonly T[];
    labelField: makeItemCallback<T>;
    valueField: makeItemCallback<T>;
    sortKeyField: compareCallback<T>;
    placeholder: string;
}

export interface IArrayElementSelector<T> extends TypedElementSelector<T> {
    selectedItem: T;
}

export type ArrayElementType<T> = T extends IArrayElementSelector<infer D> ? D : never;
