import { arraySortByKey, isNullOrUndefined, TypedEvent, TypedEventBase } from "juniper-tslib";
import { value } from "./attrs";
import { withDefault } from "./SelectBox";
import { elementClearChildren, Option } from "./tags";
export class SelectListItemSelectedEvent extends TypedEvent {
    item;
    constructor(item) {
        super("itemselected");
        this.item = item;
    }
}
/**
 * A select box that can be databound to collections.
 **/
export class SelectList extends TypedEventBase {
    element;
    makeID;
    makeLabel;
    getSortKey;
    itemToOption = new Map();
    optionToItem = new Map();
    _emptySelectionEnabled = false;
    _values = null;
    noSelection;
    constructor(element, makeID, makeLabel, getSortKey, noSelectionText) {
        super();
        this.element = element;
        this.makeID = withDefault(makeID);
        this.makeLabel = withDefault(makeLabel, "None");
        this.getSortKey = withDefault(getSortKey);
        this.noSelection = Option(noSelectionText);
        this.emptySelectionEnabled = !isNullOrUndefined(noSelectionText);
        this.element.addEventListener("input", () => this.dispatchEvent(new SelectListItemSelectedEvent(this.selectedValue)));
        Object.seal(this);
    }
    get enabled() {
        return !this.element.disabled;
    }
    set enabled(v) {
        this.element.disabled = !v;
    }
    get count() {
        return this._values && this._values.length || 0;
    }
    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    get emptySelectionEnabled() {
        return this._emptySelectionEnabled;
    }
    /**
     * Sets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    set emptySelectionEnabled(value) {
        if (value !== this.emptySelectionEnabled) {
            this._emptySelectionEnabled = value;
            this.render();
        }
    }
    /**
     * Gets the collection to which the select box was databound
     **/
    get values() {
        return this._values || [];
    }
    /**
     * Sets the collection to which the select box will be databound
     **/
    set values(newItems) {
        newItems = newItems || null;
        if (newItems !== this._values) {
            const curValue = this.selectedValue;
            this._values = newItems;
            this.render();
            this.selectedValue = curValue;
        }
    }
    get selectedOption() {
        return this.element.selectedOptions.item(0);
    }
    set selectedOption(option) {
        for (let i = 0; i < this.element.options.length; ++i) {
            const here = this.element.options[i];
            here.selected = here === option;
        }
    }
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedValue() {
        return this.optionToItem.get(this.selectedOption);
    }
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value) {
        this.selectedOption = this.itemToOption.get(this.makeID(value));
    }
    refresh() {
        this.render();
    }
    render() {
        elementClearChildren(this);
        this.itemToOption.clear();
        this.optionToItem.clear();
        if (this.count === 0 || this.emptySelectionEnabled) {
            this.append(null, this.noSelection);
        }
        if (this.count > 0) {
            const sortedItems = arraySortByKey(this.values, this.getSortKey);
            for (let item of sortedItems) {
                const option = this.makeOption(item);
                this.append(item, option);
            }
        }
    }
    makeOption(item) {
        const option = Option(value(this.makeID(item)), this.makeLabel(item));
        this.itemToOption.set(this.makeID(item), option);
        this.optionToItem.set(option, item);
        return option;
    }
    append(value, option) {
        this.element.append(option);
        this.itemToOption.set(this.makeID(value), option);
        this.optionToItem.set(option, value);
    }
}
