import { CompareFunction, arrayReplace, compareBy } from "@juniper-lib/collections/arrays";
import { CustomElement } from "@juniper-lib/dom/CustomElement";
import { Disabled, Hidden, HtmlAttr, Is, ReadOnly, Value } from "@juniper-lib/dom/attrs";
import { HtmlEvt } from "@juniper-lib/dom/evts";
import { ElementChild, HtmlTag, Option, elementClearChildren } from "@juniper-lib/dom/tags";
import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/TypedEventTarget";
import { isFunction, isNullOrUndefined, isObject, isString } from "@juniper-lib/tslib/typeChecks";
import type { makeItemCallback } from "./SelectBox";

export class SelectListItemSelectedEvent<T> extends TypedEvent<"itemselected"> {
    constructor(public item: T) {
        super("itemselected");
    }
}

type SelectListEvents<T> = {
    "input": InputEvent;
    "itemselected": SelectListItemSelectedEvent<T>;

}



export function SelectList<T>(...rest: ElementChild[]): SelectListElement<T> {
    return HtmlTag(
        "select",
        Is("select-list"),
        ...rest
    ) as SelectListElement<T>;
}

function FieldDef<T>(attrName: string, fieldName: string | makeItemCallback<T>) {
    return new HtmlAttr(
        attrName,
        fieldGetter(fieldName),
        false,
        "select"
    )
}

export function ValueField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("getValue", fieldName);
}

export function LabelField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("getLabel", fieldName);
}

export function SortKeyField<T>(fieldName: string | makeItemCallback<T>) {
    return FieldDef("getSortKey", fieldName);
}

export function Values<T>(values: T[]) {
    return new HtmlAttr("values", values, false, "select");
}

export function SelectedValue<T>(value: T) {
    return new HtmlAttr("selectedValue", value, false, "select");
}

export function onItemSelected<T>(callback: (evt: SelectListItemSelectedEvent<T>) => void, opts?: EventListenerOptions) {
    return new HtmlEvt("itemselected", callback, opts)
}

function fieldGetter<T>(fieldName: string | makeItemCallback<T>): makeItemCallback<T> {
    if (isNullOrUndefined(fieldName) || (fieldName as string).length === 0) {
        return null;
    }

    let getter: makeItemCallback<T> = null;

    if (isString(fieldName)) {
        getter = (item: T) => {
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

    return (item: T) => {
        if (isNullOrUndefined(item)) {
            return null;
        }

        return getter(item);
    }
}

function identityString(item: any): string {
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
@CustomElement("select-list", "select")
export class SelectListElement<T>
    extends HTMLSelectElement
    implements ITypedEventTarget<SelectListEvents<T>> {

    private readonly eventTarget: EventTargetMixin;
    private readonly valueToOption = new Map<string, HTMLOptionElement>();
    private readonly optionToItem = new Map<HTMLOptionElement, T>();
    private readonly _values: T[] = [];

    private _getValue: makeItemCallback<T> = identityString;
    private _getLabel: makeItemCallback<T> = identityString;
    private _getSortKey: CompareFunction<T> = null;
    private noSelection: HTMLOptionElement = null;

    /**
     * Creates a select box that can bind to collections
     */
    constructor() {
        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );

        this.addEventListener("input", () =>
            this.dispatchEvent(new SelectListItemSelectedEvent(this.selectedValue)));
    }

    connectedCallback() {
        for (const name of this.getAttributeNames()) {
            this.setAttribute(name, this.getAttribute(name));
        }
    }

    override setAttribute(name: string, value: string) {
        switch (name.toLowerCase()) {
            case "getvalue":
                this.getValue = fieldGetter(value);
                break;
            case "getlabel":
                this.getLabel = fieldGetter(value);
                break;
            case "getsortkey":
                this.getSortKey = compareBy(fieldGetter(value));
                break;
            case "placeholder":
                this.placeholder = value;
                break;
            default: super.setAttribute(name, value);
        }
    }

    override removeAttribute(name: string) {
        switch (name.toLowerCase()) {
            case "getvalue":
                this.getValue = null;
                break;
            case "getlabel":
                this.getLabel = null;
                break;
            case "getsortkey":
                this.getSortKey = null;
                break;
            case "placeholder":
                this.placeholder = null;
                break;
            default: super.removeAttribute(name);
        }
    }

    get getValue(): makeItemCallback<T> {
        return this._getValue;
    }

    set getValue(v: makeItemCallback<T>) {
        if (v !== this.getValue) {
            super.removeAttribute("getValue");
            this._getValue = v || identityString;
            this.render();
        }
    }

    get getLabel(): makeItemCallback<T> {
        return this._getLabel;
    }

    set getLabel(v: makeItemCallback<T>) {
        if (v !== this.getLabel) {
            super.removeAttribute("getLabel");
            this._getLabel = v || identityString;
            this.render();
        }
    }

    get getSortKey(): CompareFunction<T> {
        return this._getSortKey;
    }

    set getSortKey(v: CompareFunction<T>) {
        if (v !== this.getSortKey) {
            this.removeAttribute("getSortKey");
            this._getSortKey = v;
            this.render();
        }
    }

    private noSelectionText: string = null;

    get placeholder(): string {
        return this.noSelectionText;
    }

    set placeholder(v: string) {
        if (v !== this.placeholder) {
            this.noSelectionText = v;

            if (this.noSelectionText) {
                this.noSelection = Option(
                    ReadOnly(true),
                    Hidden(true),
                    Disabled(true),
                    v
                );
            }
            else {
                this.noSelection = null;
            }

            this.render();
        }
    }

    get enabled(): boolean {
        return !this.disabled;
    }

    set enabled(v: boolean) {
        this.disabled = !v;
    }

    get count() {
        return this._values && this._values.length || 0;
    }

    /**
     * Gets whether or not the select box will have a vestigial entry for "no selection" or "null" in the select box.
     **/
    private get emptySelectionEnabled(): boolean {
        return !isNullOrUndefined(this.noSelection);
    }

    /**
     * Gets the collection to which the select box was databound
     **/
    get values(): readonly T[] {
        return this._values;
    }

    /**
     * Sets the collection to which the select box will be databound
     **/
    set values(newItems: readonly T[]) {
        newItems = newItems || null;
        if (newItems !== this._values) {
            const curValue = this.selectedValue;
            arrayReplace(this._values, ...newItems);
            this.render();
            this.selectedValue = curValue;
        }
    }

    private get selectedOption(): HTMLOptionElement {
        return this.selectedOptions.item(0);
    }

    private set selectedOption(option: HTMLOptionElement) {
        for (let i = 0; i < this.options.length; ++i) {
            const here = this.options[i];
            here.selected = here === option;
        }
    }

    private get makeValue() { return this.getValue || this.getLabel; }
    private get makeLabel() { return this.getLabel || this.getValue; }

    /**
     * Gets the item at `selectedIndex` in the collection to which the select box was databound
     */
    get selectedValue(): T {
        return this.optionToItem.get(this.selectedOption);
    }

    /**
     * Gets the index of the given item in the select box's databound collection, then
     * sets that index as the `selectedIndex`.
     */
    set selectedValue(value: T) {
        if (this.makeValue) {
            this.selectedOption = this.valueToOption.get(this.makeValue(value));
        }
    }

    private render() {
        elementClearChildren(this);
        this.valueToOption.clear();
        this.optionToItem.clear();

        if (this.makeValue) {
            if (this.count === 0 || this.emptySelectionEnabled) {
                this.mapOption(null, this.noSelection);
            }

            if (this.count > 0) {
                const items = [...this.values];
                if (this.getSortKey) {
                    items.sort(this.getSortKey);
                }

                for (const item of items) {
                    const option = Option(
                        Value(this.makeValue(item)),
                        this.makeLabel(item)
                    );
                    this.mapOption(item, option);
                }
            }
        }
    }

    private mapOption(value: T, option: HTMLOptionElement): void {
        this.append(option);
        this.valueToOption.set(this.makeValue(value), option);
        this.optionToItem.set(option, value);
    }

    override addEventListener<EventTypeT extends keyof SelectListEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof SelectListEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>): void {
        this.eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<SelectListEvents<T>>): void {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<SelectListEvents<T>>): void {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof SelectListEvents<T>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<SelectListEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof SelectListEvents<T>>(type?: EventTypeT): void {
        this.eventTarget.clearEventListeners(type as string);
    }
}