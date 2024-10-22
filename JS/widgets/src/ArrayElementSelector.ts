import { compareCallback } from "@juniper-lib/util";
import { makeItemCallback } from "./FieldDef";

export interface TypedElementSelector<T> extends HTMLElement {
    labelField: makeItemCallback<T>;
    valueField: makeItemCallback<T>;
    sortKeyField: compareCallback<T>;
    data: readonly T[];
    placeholder: string;
    disabled: boolean;
    readOnly: boolean;
}

export interface ITypedElementSelector<T> extends TypedElementSelector<T> {
    selectedItem: T;
}

export type ArrayElementType<T> = T extends ITypedElementSelector<infer D>
    ? D
    : never;
