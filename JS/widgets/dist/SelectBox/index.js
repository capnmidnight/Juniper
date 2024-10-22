import { arrayClear, arrayInsertAt, arrayRemove, arraySortNumericByKey } from "@juniper-lib/collections/dist/arrays";
import { ClassList } from "@juniper-lib/dom/dist/attrs";
import { Div, HtmlRender, elementClearChildren, Select } from "@juniper-lib/dom/dist/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isDefined, isFunction, isNullOrUndefined, isString } from "@juniper-lib/tslib/dist/typeChecks";
import "./styles.css";
export class SelectBoxItemSelectedEvent extends TypedEvent {
    constructor(item) {
        super("itemselected");
        this.item = item;
    }
}
export function withDefault(callback, defaultValue = null) {
    return (value) => {
        try {
            return callback(value);
        }
        catch {
            return defaultValue;
        }
    };
}
class SelectBoxRow extends TypedEventTarget {
    get element() {
        return this._element;
    }
    constructor(value, id, contents, sortKey) {
        super();
        this.id = id;
        this._value = value;
        this._contents = contents;
        this._sortKey = sortKey;
        this._element = Div(ClassList("SelectBoxRow"), contents);
        this.element.addEventListener("click", () => {
            if (this.enabled) {
                this.dispatchEvent(new SelectBoxItemSelectedEvent(this._value));
            }
        });
    }
    get value() {
        return this._value;
    }
    get sortKey() {
        return this._sortKey;
    }
    get contents() {
        return this._contents;
    }
    set contents(v) {
        if (v !== this.contents) {
            this._contents = v;
            elementClearChildren(this);
            HtmlRender(this, this._contents);
        }
    }
    get selected() {
        return this.element.classList.contains("selected");
    }
    set selected(v) {
        if (v !== this.selected) {
            this.element.classList.toggle("selected");
        }
    }
    get disabled() {
        return this.element.classList.contains("disabled");
    }
    set disabled(v) {
        if (v !== this.disabled) {
            this.element.classList.toggle("disabled");
        }
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
}
/**
 * A select box that can be databound to collections.
 **/
export class SelectBox extends TypedEventTarget {
    get element() {
        return this._element;
    }
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(elementOrMakeID, makeIDOrMakeLabel, makeLabelOrGetSortKey, getSortKeyOrNoSelectionText, maybeNoSelectionText) {
        super();
        this.itemToOption = new Map();
        this._enabled = true;
        this._emptySelectionEnabled = false;
        this._values = null;
        this.noSelection = null;
        this.options = new Array();
        let element;
        let makeID;
        let makeLabel;
        let getSortKey;
        let noSelectionText;
        if (isFunction(elementOrMakeID)) {
            element = Select();
            makeID = elementOrMakeID;
            makeLabel = makeIDOrMakeLabel;
            getSortKey = makeLabelOrGetSortKey;
        }
        else {
            element = elementOrMakeID;
            makeID = makeIDOrMakeLabel;
            makeLabel = makeLabelOrGetSortKey;
            getSortKey = getSortKeyOrNoSelectionText;
        }
        if (isString(getSortKeyOrNoSelectionText)) {
            noSelectionText = getSortKeyOrNoSelectionText;
        }
        else {
            noSelectionText = maybeNoSelectionText;
        }
        this._element = Div(ClassList("SelectBox"), this.elementRows = Div(ClassList("SelectBoxContent")));
        if (isDefined(element.parentElement)) {
            element.parentElement.replaceChild(this._element, element);
            for (let i = 0; i < element.classList.length; ++i) {
                this._element.classList.add(element.classList.item(i));
            }
            Object.assign(this._element.style, element.style);
        }
        this.makeID = withDefault(makeID);
        this.makeLabel = withDefault(makeLabel, "None");
        this.getSortKey = withDefault(getSortKey);
        this.noSelection = new SelectBoxRow(null, null, noSelectionText, null);
        this.emptySelectionEnabled = isString(noSelectionText);
        Object.seal(this);
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        if (v !== this.enabled) {
            this._enabled = v;
            for (const option of this.options) {
                option.enabled = v;
            }
        }
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
        for (const option of this.options) {
            if (option.selected) {
                return option;
            }
        }
        return null;
    }
    set selectedOption(option) {
        for (const other of this.options) {
            other.selected = other === option;
        }
        let top = 0;
        if (isDefined(option)) {
            top = option.element.offsetTop - this.elementRows.offsetTop;
        }
        if (top < this._element.scrollTop
            || this._element.clientHeight + this._element.scrollTop < top) {
            this._element.scrollTo({
                behavior: "smooth",
                top
            });
        }
    }
    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedValue() {
        if (isNullOrUndefined(this.selectedOption)) {
            return null;
        }
        return this.selectedOption.value;
    }
    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value) {
        this.selectedOption = this.itemToOption.get(this.makeID(value));
    }
    append(item) {
        this.values = [
            ...this.values,
            item
        ];
    }
    remove(item) {
        const id = this.makeID(item);
        return this.removeByKey(id) === item;
    }
    has(id) {
        for (const value of this.values) {
            if (this.makeID(value) === id) {
                return true;
            }
        }
        return false;
    }
    get(id) {
        for (const value of this.values) {
            if (this.makeID(value) === id) {
                return value;
            }
        }
        return null;
    }
    removeByKey(id) {
        let found = null;
        this.values = this.values.filter((item) => {
            if (this.makeID(item) === id) {
                found = item;
                return false;
            }
            return true;
        });
        return found;
    }
    refresh() {
        const sortedItems = arraySortNumericByKey(this.values, this.getSortKey);
        const start = this.emptySelectionEnabled ? 1 : 0;
        let lastOption = this.emptySelectionEnabled
            ? this.noSelection
            : null;
        for (let i = 0; i < sortedItems.length; ++i) {
            const item = sortedItems[i];
            let option = this.itemToOption.get(this.makeID(item));
            if (option) {
                option.id = this.makeID(item);
                option.contents = this.makeLabel(item);
            }
            else {
                option = this.makeOption(item);
                if (lastOption) {
                    lastOption.element.insertAdjacentElement("afterend", option.element);
                }
                else {
                    this.elementRows.insertAdjacentElement("afterbegin", option.element);
                }
                this.addRow(option, i + start);
            }
            lastOption = option;
        }
        for (let i = this.options.length - 1; i >= start; --i) {
            const option = this.options[i];
            if (this._values.indexOf(option.value) === -1) {
                this.removeRow(option);
            }
        }
    }
    render() {
        elementClearChildren(this.elementRows);
        this.itemToOption.clear();
        arrayClear(this.options);
        if (this.count === 0 || this.emptySelectionEnabled) {
            this.appendOption(this.noSelection);
        }
        if (this.count > 0) {
            const sortedItems = arraySortNumericByKey(this.values, this.getSortKey);
            for (const item of sortedItems) {
                const option = this.makeOption(item);
                this.appendOption(option);
            }
        }
    }
    makeOption(item) {
        return new SelectBoxRow(item, this.makeID(item), this.makeLabel(item), this.getSortKey(item));
    }
    appendOption(option) {
        const index = this.options.length;
        this.elementRows.append(option.element);
        this.addRow(option, index);
    }
    addRow(option, index) {
        arrayInsertAt(this.options, option, index);
        this.itemToOption.set(this.makeID(option.value), option);
        option.addScopedEventListener(this, "itemselected", (evt) => {
            this.selectedValue = evt.item;
            this.dispatchEvent(new SelectBoxItemSelectedEvent(evt.item));
        });
    }
    removeRow(option) {
        option.removeScope(this);
        this.elementRows.removeChild(option.element);
        arrayRemove(this.options, option);
        this.itemToOption.delete(this.makeID(option.value));
    }
}
//# sourceMappingURL=index.js.map