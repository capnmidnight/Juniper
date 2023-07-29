import { arrayClear, arrayInsertAt, arrayRemove, arraySortNumericByKey } from "@juniper-lib/collections/arrays";
import { ClassList } from "@juniper-lib/dom/attrs";
import { Div, elementApply, ElementChild, elementClearChildren, ErsatzElement, Select } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/TypedEventBase";
import { isDefined, isFunction, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";

import "./styles.css";


/**
 * Creates a string from a list item to use as the item's ID or label in a select box.
 */
type readItemCallback<T, V> = (obj: T) => V;
export type makeItemCallback<T> = readItemCallback<T, string>;
export type makeLabelCallback<T> = readItemCallback<T, ElementChild>;

export class SelectBoxItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    constructor(public item: T) {
        super("itemselected");
    }
}

type SelectBoxEvents<T> = {
    itemselected: SelectBoxItemSelectedEvent<T>;
}

export function withDefault<T, V>(callback: readItemCallback<T, V>, defaultValue: V = null): readItemCallback<T, V> {
    return (value: T) => {
        try {
            return callback(value);
        }
        catch {
            return defaultValue;
        }
    };
}

class SelectBoxRow<T>
    extends TypedEventBase<SelectBoxEvents<T>>
    implements ErsatzElement {

    private _value: T;
    private _contents: ElementChild;
    private _sortKey: string;

    private _element: HTMLElement;
    get element() {
        return this._element;
    }

    constructor(value: T,
        public id: string,
        contents: ElementChild,
        sortKey: string) {
        super();

        this._value = value;
        this._contents = contents;
        this._sortKey = sortKey;

        this._element = Div(
            ClassList("SelectBoxRow"),
            contents);

        this.element.addEventListener("click", () => {
            if (this.enabled) {
                this.dispatchEvent(new SelectBoxItemSelectedEvent(this._value));
            }
        });
    }

    get value(): T {
        return this._value;
    }

    get sortKey(): string {
        return this._sortKey;
    }

    get contents(): ElementChild {
        return this._contents;
    }

    set contents(v: ElementChild) {
        if (v !== this.contents) {
            this._contents = v;
            elementClearChildren(this);
            elementApply(this, this._contents);
        }
    }

    get selected(): boolean {
        return this.element.classList.contains("selected");
    }

    set selected(v: boolean) {
        if (v !== this.selected) {
            this.element.classList.toggle("selected");
        }
    }

    get disabled(): boolean {
        return this.element.classList.contains("disabled");
    }

    set disabled(v: boolean) {
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
export class SelectBox<T>
    extends TypedEventBase<SelectBoxEvents<T>>
    implements ErsatzElement {

    private makeID: makeItemCallback<T>;
    private makeLabel: makeLabelCallback<T>;
    private getSortKey: makeItemCallback<T>;

    private itemToOption = new Map<string, SelectBoxRow<T>>();

    private _enabled = true;
    private _emptySelectionEnabled = false;
    private _values: readonly T[] = null;

    private _element: HTMLElement;
    get element() {
        return this._element;
    }

    private elementRows: HTMLElement;
    private noSelection: SelectBoxRow<T> = null;

    public options = new Array<SelectBoxRow<T>>();

    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(
        makeID: makeItemCallback<T>,
        makeLabel: makeLabelCallback<T>,
        getSortKey: makeItemCallback<T>,
        noSelectionText: string);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(
        makeID: makeItemCallback<T>,
        makeLabel: makeLabelCallback<T>,
        getSortKey: makeItemCallback<T>);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(
        element: HTMLSelectElement,
        makeID: makeItemCallback<T>,
        makeLabel: makeLabelCallback<T>,
        getSortKey: makeItemCallback<T>,
        noSelectionText: string);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(
        element: HTMLSelectElement,
        makeID: makeItemCallback<T>,
        makeLabel: makeLabelCallback<T>,
        getSortKey: makeItemCallback<T>);
    /**
     * Creates a select box that can bind to collections
     * @param element - the select box to wrap.
     * @param makeID - a function that evalutes a databound item to create an ID for it.
     * @param makeLabel - a function that evalutes a databound item to create a label for it.
     * @param noSelectionText - the text to display when no items are available.
     */
    constructor(
        elementOrMakeID: HTMLSelectElement | makeItemCallback<T>,
        makeIDOrMakeLabel: makeItemCallback<T>,
        makeLabelOrGetSortKey: makeLabelCallback<T> | makeItemCallback<T>,
        getSortKeyOrNoSelectionText?: makeItemCallback<T> | string,
        maybeNoSelectionText?: string) {
        super();

        let element: HTMLSelectElement;
        let makeID: makeItemCallback<T>;
        let makeLabel: makeLabelCallback<T>;
        let getSortKey: makeItemCallback<T>;
        let noSelectionText: string;

        if (isFunction(elementOrMakeID)) {
            element = Select();
            makeID = elementOrMakeID;
            makeLabel = makeIDOrMakeLabel;
            getSortKey = makeLabelOrGetSortKey as makeItemCallback<T>;
        }
        else {
            element = elementOrMakeID;
            makeID = makeIDOrMakeLabel;
            makeLabel = makeLabelOrGetSortKey as makeLabelCallback<T>;
            getSortKey = getSortKeyOrNoSelectionText as makeItemCallback<T>;
        }

        if (isString(getSortKeyOrNoSelectionText)) {
            noSelectionText = getSortKeyOrNoSelectionText;
        }
        else {
            noSelectionText = maybeNoSelectionText;
        }

        this._element = Div(ClassList("SelectBox"),
            this.elementRows = Div(ClassList("SelectBoxContent")));

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

    get enabled(): boolean {
        return this._enabled;
    }

    set enabled(v: boolean) {
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
    get emptySelectionEnabled(): boolean {
        return this._emptySelectionEnabled;
    }

    /**
     * Sets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    set emptySelectionEnabled(value: boolean) {
        if (value !== this.emptySelectionEnabled) {
            this._emptySelectionEnabled = value;
            this.render();
        }
    }

    /**
     * Gets the collection to which the select box was databound
     **/
    get values(): readonly T[] {
        return this._values || [];
    }

    /**
     * Sets the collection to which the select box will be databound
     **/
    set values(newItems: readonly T[]) {
        newItems = newItems || null;
        if (newItems !== this._values) {
            const curValue = this.selectedValue;
            this._values = newItems;
            this.render();
            this.selectedValue = curValue;
        }
    }

    private get selectedOption(): SelectBoxRow<T> {
        for (const option of this.options) {
            if (option.selected) {
                return option;
            }
        }
        return null;
    }

    private set selectedOption(option: SelectBoxRow<T>) {
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
    get selectedValue(): T {
        if (isNullOrUndefined(this.selectedOption)) {
            return null;
        }

        return this.selectedOption.value;
    }

    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value: T) {
        this.selectedOption = this.itemToOption.get(this.makeID(value));
    }

    append(item: T): void {
        this.values = [
            ...this.values,
            item
        ];
    }

    remove(item: T): boolean {
        const id = this.makeID(item);
        return this.removeByKey(id) === item;
    }

    has(id: string): boolean {
        for (const value of this.values) {
            if (this.makeID(value) === id) {
                return true;
            }
        }

        return false;
    }

    get(id: string): T {
        for (const value of this.values) {
            if (this.makeID(value) === id) {
                return value;
            }
        }

        return null;
    }

    removeByKey(id: string): T {
        let found: T = null;

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
        let lastOption: SelectBoxRow<T> = this.emptySelectionEnabled
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

    private render() {
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

    private makeOption(item: T): SelectBoxRow<T> {
        return new SelectBoxRow<T>(
            item,
            this.makeID(item),
            this.makeLabel(item),
            this.getSortKey(item));
    }

    private appendOption(option: SelectBoxRow<T>): void {
        const index = this.options.length;
        this.elementRows.append(option.element);
        this.addRow(option, index);
    }

    private addRow(option: SelectBoxRow<T>, index: number) {
        arrayInsertAt(this.options, option, index);
        this.itemToOption.set(this.makeID(option.value), option);
        option.addScopedEventListener(this, "itemselected", (evt) => {
            this.selectedValue = evt.item;
            this.dispatchEvent(new SelectBoxItemSelectedEvent(evt.item));
        });
    }

    private removeRow(option: SelectBoxRow<T>): void {
        option.removeScope(this);
        this.elementRows.removeChild(option.element);
        arrayRemove(this.options, option);
        this.itemToOption.delete(this.makeID(option.value));
    }
}