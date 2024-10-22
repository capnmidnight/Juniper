import { compareBy, isNullOrUndefined } from "@juniper-lib/util";
import { HtmlEvt, HtmlProp } from "@juniper-lib/dom";
export function fieldGetter(fieldNameOrGetter) {
    if (isNullOrUndefined(fieldNameOrGetter) || fieldNameOrGetter.length === 0) {
        return null;
    }
    let getter = null;
    if (typeof fieldNameOrGetter === "string") {
        getter = (item) => {
            if (typeof item === "object"
                && fieldNameOrGetter in item) {
                getter = (item) => item[fieldNameOrGetter];
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
    return (item) => {
        if (isNullOrUndefined(item)) {
            return null;
        }
        return getter(item);
    };
}
function FieldDef(attrName, getter) {
    return new HtmlProp(attrName, getter);
}
export function LabelField(fieldName) {
    return FieldDef("labelField", fieldGetter(fieldName));
}
export function ValueField(fieldName) {
    return FieldDef("valueField", fieldGetter(fieldName));
}
export function SortKeyField(fieldName) {
    return FieldDef("sortKeyField", compareBy(fieldGetter(fieldName)));
}
export function CompareBy(comparison) {
    return FieldDef("sortKeyField", comparison);
}
export function DataAttr(values) {
    return new HtmlProp("data", values);
}
export function SelectedItem(value) {
    return new HtmlProp("selectedItem", value);
}
export function OnItemSelected(callback, opts) {
    return HtmlEvt("itemselected", callback, opts, false);
}
export function identityString(item) {
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
//# sourceMappingURL=FieldDef.js.map