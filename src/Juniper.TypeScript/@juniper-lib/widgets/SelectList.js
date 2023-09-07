import { __decorate, __metadata } from "tslib";
import { arrayReplace, compareBy } from "@juniper-lib/collections/arrays";
import { CustomElement } from "@juniper-lib/dom/CustomElement";
import { Disabled, Hidden, HtmlAttr, Is, Value } from "@juniper-lib/dom/attrs";
import { HtmlEvt } from "@juniper-lib/dom/evts";
import { HtmlTag, Option, elementClearChildren } from "@juniper-lib/dom/tags";
import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { isFunction, isNullOrUndefined, isObject, isString } from "@juniper-lib/tslib/typeChecks";
export class SelectListItemSelectedEvent extends TypedEvent {
    constructor(item, items) {
        super("itemselected");
        this.item = item;
        this.items = items;
    }
}
export function SelectList(...rest) {
    return HtmlTag("select", Is("select-list"), ...rest);
}
function FieldDef(attrName, fieldName) {
    return new HtmlAttr(attrName, fieldGetter(fieldName), false, "select");
}
export function ValueField(fieldName) {
    return FieldDef("valueField", fieldName);
}
export function LabelField(fieldName) {
    return FieldDef("labelField", fieldName);
}
export function SortKeyField(fieldName) {
    return FieldDef("sortKeyField", fieldName);
}
export function DataAttr(values) {
    return new HtmlAttr("data", values, false, "select");
}
export function SelectedItem(value) {
    return new HtmlAttr("selectedItem", value, false, "select");
}
export function onItemSelected(callback, opts) {
    return new HtmlEvt("itemselected", callback, opts);
}
function fieldGetter(fieldName) {
    if (isNullOrUndefined(fieldName) || fieldName.length === 0) {
        return null;
    }
    let getter = null;
    if (isString(fieldName)) {
        getter = (item) => {
            if (isObject(item)
                && fieldName in item) {
                getter = (item) => 
                //@ts-ignore
                item[fieldName];
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
    return (item) => {
        if (isNullOrUndefined(item)) {
            return null;
        }
        return getter(item);
    };
}
function identityString(item) {
    if (isNullOrUndefined(item)) {
        return null;
    }
    if (isString(item)) {
        return item;
    }
    else if ("toString" in item
        && isFunction(item.toString)) {
        return item.toString();
    }
    return null;
}
/**
 * A select box that can be databound to collections.
 **/
let SelectListElement = class SelectListElement extends HTMLSelectElement {
    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();
        this.valueToOption = new Map();
        this.optionToItem = new Map();
        this._values = [];
        this._getValue = identityString;
        this._getLabel = identityString;
        this._getSortKey = null;
        this.noSelection = null;
        this.noSelectionText = null;
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
        this.addEventListener("input", () => this.dispatchEvent(new SelectListItemSelectedEvent(this.selectedItem, this.selectedItems)));
    }
    connectedCallback() {
        for (const name of this.getAttributeNames()) {
            this.setAttribute(name, this.getAttribute(name));
        }
    }
    setAttribute(name, value) {
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = fieldGetter(value);
                break;
            case "labelfield":
                this.labelField = fieldGetter(value);
                break;
            case "sortkeyfield":
                this.sortKeyField = compareBy(fieldGetter(value));
                break;
            case "placeholder":
                this.placeholder = value;
                break;
            default: super.setAttribute(name, value);
        }
    }
    removeAttribute(name) {
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = null;
                break;
            case "labelfield":
                this.labelField = null;
                break;
            case "sortkeyfield":
                this.sortKeyField = null;
                break;
            case "placeholder":
                this.placeholder = null;
                break;
            default: super.removeAttribute(name);
        }
    }
    get valueField() {
        return this._getValue;
    }
    set valueField(v) {
        if (v !== this.valueField) {
            super.removeAttribute("getValue");
            this._getValue = v || identityString;
            this.render();
        }
    }
    get labelField() {
        return this._getLabel;
    }
    set labelField(v) {
        if (v !== this.labelField) {
            super.removeAttribute("getLabel");
            this._getLabel = v || identityString;
            this.render();
        }
    }
    get sortKeyField() {
        return this._getSortKey;
    }
    set sortKeyField(v) {
        if (v !== this.sortKeyField) {
            this.removeAttribute("getSortKey");
            this._getSortKey = v;
            this.render();
        }
    }
    get placeholder() {
        return this.noSelectionText;
    }
    set placeholder(v) {
        if (v !== this.placeholder) {
            this.noSelectionText = v;
            if (this.noSelectionText) {
                this.noSelection = Option(Hidden(true), Disabled(true), v);
            }
            else {
                this.noSelection = null;
            }
            this.render();
        }
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
    get count() {
        return this._values && this._values.length || 0;
    }
    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get emptySelectionEnabled() {
        return !isNullOrUndefined(this.noSelection);
    }
    /**
     * Gets the collection to which the select box was databound
     **/
    get data() {
        return this._values;
    }
    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems) {
        newItems = newItems || null;
        if (newItems !== this._values) {
            const curValue = this.selectedItem;
            arrayReplace(this._values, ...newItems);
            this.render();
            this.selectedItem = curValue;
        }
    }
    get selectedOption() {
        return this.selectedOptions.item(0);
    }
    set selectedOption(option) {
        for (let i = 0; i < this.options.length; ++i) {
            const here = this.options[i];
            here.selected = here === option;
        }
    }
    get makeValue() { return this.valueField || this.labelField; }
    get makeLabel() { return this.labelField || this.valueField; }
    get values() {
        return Array
            .from(this.selectedOptions)
            .map(opt => opt.value);
    }
    set values(values) {
        const v = new Set(values);
        for (const option of this.options) {
            option.selected = v.has(option.value || option.innerHTML);
        }
    }
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedItem() {
        return this.optionToItem.get(this.selectedOption);
    }
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedItem(value) {
        if (this.makeValue) {
            this.selectedOption = this.valueToOption.get(this.makeValue(value));
        }
    }
    get selectedItems() {
        return Array.from(this.selectedOptions)
            .map(opt => this.optionToItem.get(opt));
    }
    set selectedItems(values) {
        this.values = values.map(v => this.makeValue(v));
    }
    render() {
        elementClearChildren(this);
        this.valueToOption.clear();
        this.optionToItem.clear();
        if (this.makeValue) {
            if (this.count === 0 || this.emptySelectionEnabled) {
                this.mapOption(null, this.noSelection);
            }
            if (this.count > 0) {
                const items = [...this.data];
                if (this.sortKeyField) {
                    items.sort(this.sortKeyField);
                }
                for (const item of items) {
                    const option = Option(Value(this.makeValue(item)), this.makeLabel(item));
                    this.mapOption(item, option);
                }
            }
        }
    }
    mapOption(value, option) {
        this.append(option);
        this.valueToOption.set(this.makeValue(value), option);
        this.optionToItem.set(option, value);
    }
    addEventListener(type, callback, options) {
        this.eventTarget.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        this.eventTarget.removeEventListener(type, callback);
    }
    dispatchEvent(evt) {
        return this.eventTarget.dispatchEvent(evt);
    }
    addBubbler(bubbler) {
        this.eventTarget.addBubbler(bubbler);
    }
    removeBubbler(bubbler) {
        this.eventTarget.removeBubbler(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        this.eventTarget.addScopedEventListener(scope, type, callback, options);
    }
    removeScope(scope) {
        this.eventTarget.removeScope(scope);
    }
    clearEventListeners(type) {
        this.eventTarget.clearEventListeners(type);
    }
};
SelectListElement = __decorate([
    CustomElement("select-list", "select"),
    __metadata("design:paramtypes", [])
], SelectListElement);
export { SelectListElement };
//# sourceMappingURL=SelectList.js.map