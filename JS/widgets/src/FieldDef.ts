import { compareBy, compareCallback, eventHandler, hasValue } from "@juniper-lib/util";
import { HtmlAttr, HtmlEvt } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";

export type makeItemCallback<T> = (item: T) => string;

export function fieldGetter<T>(fieldName: string | makeItemCallback<T>): makeItemCallback<T> {
    if (!hasValue(fieldName) || (fieldName as string).length === 0) {
        return null;
    }

    let getter: makeItemCallback<T> = null;

    if (typeof fieldName === "string") {
        getter = (item: T) => {
            if (typeof item === "object"
                && fieldName in item) {
                getter = (item) =>
                    (item as any)[fieldName];
            }

            if (!getter) {
                return null;
            }

            return getter(item);
        };
    }
    else {
        getter = fieldName;
    }

    return (item: T) => {
        if (!hasValue(item)) {
            return null;
        }

        return getter(item);
    }
}

function FieldDef(attrName: string, getter: Function) {
    return new HtmlAttr(
        attrName,
        getter,
        false,
        "typed-select",
        "typed-input",
        "inclusion-list",
        "array-editor"
    );
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

export function DataAttr<T>(values: T[]) {
    return new HtmlAttr("data", values, false, "typed-select", "typed-input", "inclusion-list", "array-editor");
}

export function SelectedItem<T>(value: T) {
    return new HtmlAttr("selectedItem", value, false, "typed-select", "typed-input");
}

export function OnItemSelected<T>(callback: (evt: TypedItemSelectedEvent<T>) => void, opts?: EventListenerOptions) {
    return HtmlEvt("itemselected", callback as eventHandler, opts);
}

export function identityString(item: any): string {
    if (!hasValue(item)) {
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