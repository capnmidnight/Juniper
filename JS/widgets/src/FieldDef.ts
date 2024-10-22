import { compareBy, compareCallback, eventHandler, isNullOrUndefined } from "@juniper-lib/util";
import { HtmlEvt, HtmlProp } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";

export type makeItemCallback<T> = (item: T) => string;

export function fieldGetter<T>(fieldNameOrGetter: string | makeItemCallback<T>): makeItemCallback<T> {
    if (isNullOrUndefined(fieldNameOrGetter) || (fieldNameOrGetter as string).length === 0) {
        return null;
    }

    let getter: makeItemCallback<T> = null;

    if (typeof fieldNameOrGetter === "string") {
        getter = (item: T) => {
            if (typeof item === "object"
                && fieldNameOrGetter in item) {
                getter = (item) =>
                    (item as any)[fieldNameOrGetter];
            }

            if (!getter) {
                return null;
            }

            return getter(item);
        };
    }
    else {
        getter = fieldNameOrGetter;
    }

    return (item: T) => {
        if (isNullOrUndefined(item)) {
            return null;
        }

        return getter(item);
    };
}

function FieldDef<T extends string>(attrName: T, getter: Function) {
    return new HtmlProp(attrName, getter);
}

export function LabelField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("labelField", fieldGetter(fieldName));
}

export function ValueField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("valueField", fieldGetter(fieldName));
}

export function SortKeyField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("sortKeyField", compareBy(fieldGetter(fieldName)));
}

export function CompareBy<T>(comparison: compareCallback<T>) {
    return FieldDef("sortKeyField", comparison);
}

export function DataAttr<T>(values: readonly T[]) {
    return new HtmlProp("data", values);
}

export function SelectedItem<T>(value: T) {
    return new HtmlProp("selectedItem", value);
}

export function OnItemSelected<DataT, TargetT extends EventTarget = EventTarget>(callback: (evt: TypedItemSelectedEvent<DataT, TargetT>) => void, opts?: EventListenerOptions) {
    return HtmlEvt("itemselected", callback as eventHandler, opts, false);
}

export function identityString(item: any): string {
    if (isNullOrUndefined(item)) {
        return null;
    }

    if (typeof item === "string") {
        return item;
    }
    else if ("toString" in item
        && typeof item.toString === "function") {
        return item.toString();
    }

    return null;
}