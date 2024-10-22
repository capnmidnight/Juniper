import { arrayReplace, compareBy, compareCallback, first, singleton } from "@juniper-lib/util";
import { Button, ClassList, Disabled, ElementChild, FAIcon, Multiple, OnClick, Option, Query, QueryAll, Select, SingletonStyleBlob, SlotAttr, TemplateInstance, TitleAttr, TypedHTMLElement, Value, display, gap, gridArea, gridTemplateAreas, gridTemplateColumns, gridTemplateRows, registerFactory, rule } from "@juniper-lib/dom";
import { fieldGetter, identityString, makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";

function findStyle(name: string) {
    return first(
        (QueryAll("link[rel=stylesheet]") as HTMLLinkElement[])
            .filter(s =>
                s.href.indexOf(name) >= 0
            )
    );
}

type InclusionListEvents<T> = {
    "itemselected": TypedItemSelectedEvent<T>;
};

export class InclusionListElement<T> extends TypedHTMLElement<InclusionListEvents<T>> {

    #availableList: HTMLSelectElement;
    #includedList: HTMLSelectElement;
    #addButton: HTMLButtonElement;
    #removeButton: HTMLButtonElement;
    #availToInc = new Map<HTMLOptionElement, HTMLOptionElement>();
    #incToAvail = new Map<HTMLOptionElement, HTMLOptionElement>();

    readonly #optionToItem = new Map<HTMLOptionElement, T>();
    readonly #values: T[] = [];

    #getValue: makeItemCallback<T> = identityString;
    #getLabel: makeItemCallback<T> = identityString;
    #getSortKey: compareCallback<T> = null;

    constructor() {
        super();

        const root = this.attachShadow({ mode: "closed" });

        const style = SingletonStyleBlob("Juniper::Widgets::InclusionList", () =>
            rule(":host",
                display("grid"),
                gridTemplateColumns("1fr", "auto", "1fr"),
                gridTemplateRows("1fr", "auto", "auto", "1fr"),
                gridTemplateAreas(
                    "AVL X INC",
                    "AVL Y INC",
                    "AVL N INC",
                    "AVL Z INC"
                ),
                gap("1rem"),

                rule(" [slot=availableList]", gridArea("AVL")),
                rule(" [slot=addButton]", gridArea("Y")),
                rule(" [slot=removeButton]", gridArea("N")),
                rule(" [slot=includedList]", gridArea("INC"))
            )
        );

        const bootstrap = singleton("Juniper::InclusionList::Bootstrap", () => findStyle("bootstrap"));
        const fontAwesome = singleton("Juniper::InclusionList::FontAwesome", () => findStyle("fontawesome") || findStyle("all"));

        root.append(
            bootstrap?.cloneNode(true),
            fontAwesome?.cloneNode(true),
            style.cloneNode(true),
            TemplateInstance("Juniper::Widgets::InclusionList", () => [
                Select(
                    Multiple(true),
                    SlotAttr("availableList"),
                    ClassList("form-select"),
                    TitleAttr("Items available for selection")
                ),
                Button(
                    SlotAttr("addButton"),
                    ClassList("btn", "btn-primary"),
                    TitleAttr("Add selected items to selection"),
                    Disabled(true),
                    FAIcon("angle-right")
                ),
                Button(
                    SlotAttr("removeButton"),
                    ClassList("btn", "btn-primary"),
                    TitleAttr("Remove selected items from selection"),
                    Disabled(true),
                    FAIcon("angle-left")
                ),
                Select(
                    Multiple(true),
                    SlotAttr("includedList"),
                    ClassList("form-select"),
                    TitleAttr("Items included in selection")
                )
            ])
        );

        this.#availableList = Select(Query(root, "[slot=availableList]"));
        this.#includedList = Select(Query(root, "[slot=includedList]"));

        const move = (options: HTMLCollectionOf<HTMLOptionElement>, list: HTMLSelectElement) => {
            const map = list === this.#availableList
                ? this.#incToAvail
                : this.#availToInc;

            for (const option of Array.from(options)) {
                const alternate = map.get(option);
                alternate.hidden = false;
                alternate.selected = true;
                option.hidden = true;
                option.selected = false;
            }

            this.dispatchEvent(new TypedItemSelectedEvent(this.selectedItems));
        }

        this.#addButton = Button(
            Query(root, "[slot=addButton]"),
            OnClick(() => {
                move(this.#availableList.selectedOptions, this.#includedList);
                this.#refresh();
            })
        );

        this.#removeButton = Button(
            Query(root, "[slot=removeButton]"),
            OnClick(() => {
                move(this.#includedList.selectedOptions, this.#availableList);
                this.#refresh();
            })
        );


        const refresh = this.#refresh.bind(this);
        this.#availableList.addEventListener("input", refresh);
        this.#availableList.addEventListener("change", refresh);
        this.#includedList.addEventListener("input", refresh);
        this.#includedList.addEventListener("change", refresh);

        this.disabled = this.hasAttribute("disabled");
        this.name = this.getAttribute("name");
        this.required = this.hasAttribute("required");
        this.size = parseFloat(this.getAttribute("size"));
    }

    connectedCallback() {
        for (const name of this.getAttributeNames()) {
            this.setAttribute(name, this.getAttribute(name));
        }

        const options = this.querySelectorAll<HTMLOptionElement>("option");
        if (options.length > 0) {
            this.setAttribute("valuefield", "value");
            this.setAttribute("labelfield", "innerHTML");
            this.data = Array.from(options) as any;
        }
    }

    override setAttribute(name: string, value: string) {
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
            default: super.setAttribute(name, value);
        }
    }

    override removeAttribute(name: string) {
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
            default: super.removeAttribute(name);
        }
    }

    get valueField(): makeItemCallback<T> {
        return this.#getValue;
    }

    set valueField(v: makeItemCallback<T>) {
        if (v !== this.valueField) {
            super.removeAttribute("getValue");
            this.#getValue = v || identityString;
            this.#render();
        }
    }

    get labelField(): makeItemCallback<T> {
        return this.#getLabel;
    }

    set labelField(v: makeItemCallback<T>) {
        if (v !== this.labelField) {
            super.removeAttribute("getLabel");
            this.#getLabel = v || identityString;
            this.#render();
        }
    }

    get sortKeyField(): compareCallback<T> {
        return this.#getSortKey;
    }

    set sortKeyField(v: compareCallback<T>) {
        if (v !== this.sortKeyField) {
            this.removeAttribute("getSortKey");
            this.#getSortKey = v;
            this.#render();
        }
    }

    /**
     * Gets the collection to which the select box was databound
     **/
    get data(): readonly T[] {
        return this.#values;
    }

    /**
     * Sets the collection to which the select box will be databound
     **/
    set data(newItems: readonly T[]) {
        newItems = newItems || null;
        if (newItems !== this.#values) {
            const curItems = this.selectedItems;
            arrayReplace(this.#values, newItems);
            this.#render();
            this.selectedItems = curItems;
        }
    }

    get selectedItems(): T[] { return Array.from(this.#getSelectedItems()); }
    *#getSelectedItems() {
        for (const opt of this.#includedList.options) {
            if (!opt.hidden) {
                yield this.#optionToItem.get(opt);
            }
        }
    }

    set selectedItems(values: readonly T[]) {
        const v = new Set(values);
        for (const option of this.options) {
            const value = this.#optionToItem.get(option);
            const alternate = this.#availToInc.get(option);
            option.hidden = v.has(value);
            alternate.hidden = !option.hidden;
        }
    }

    get #makeValue() { return this.valueField || this.labelField; }
    get #makeLabel() { return this.labelField || this.valueField; }

    #render() {
        this.#availableList.innerHTML = "";
        this.#includedList.innerHTML = "";
        this.#optionToItem.clear();
        this.#availToInc.clear();
        this.#incToAvail.clear();

        if (this.#makeValue && this.#values.length > 0) {
            const items = Array.from(this.#values);
            if (this.sortKeyField) {
                items.sort(this.sortKeyField);
            }

            for (const item of items) {
                const option = Option(
                    Value(this.#makeValue(item)),
                    this.#makeLabel(item)
                );
                const alternate = option.cloneNode(true) as HTMLOptionElement;
                alternate.hidden = true;
                this.#availableList.append(option);
                this.#includedList.append(alternate);
                this.#optionToItem.set(option, item);
                this.#optionToItem.set(alternate, item);
                this.#availToInc.set(option, alternate);
                this.#incToAvail.set(alternate, option);
            }
        }

        this.#refresh();
    }

    #refresh(evt?: Event) {
        this.#addButton.disabled = this.#disabled || this.#availableList.selectedOptions.length === 0;
        this.#removeButton.disabled = this.#disabled || this.#includedList.selectedOptions.length === 0;

        for (const opt of this.#availableList.options) {
            const alternate = this.#availToInc.get(opt);
            opt.disabled = this.#disabled;
            opt.hidden = this.#filter.length > 0 && opt.innerHTML.toLocaleLowerCase().indexOf(this.#filter) === -1
                || !alternate.hidden;
        }

        for (const opt of this.#includedList.options) {
            opt.disabled = this.#disabled;
        }

        if (evt) {
            if (evt.type === "input") {
                evt.stopPropagation();
            }

            evt = new Event(evt.type, {
                bubbles: evt.bubbles,
                cancelable: evt.cancelable,
                composed: evt.composed
            });
            this.dispatchEvent(evt);
        }
    }

    #filter = "";
    get filter() { return this.#filter; }
    set filter(v) {
        this.#filter = (v || "").toLocaleLowerCase();
        this.#refresh();
    }

    #disabled = false;
    get disabled() { return this.#disabled; }
    set disabled(v) {
        this.#disabled = v;
        this.#refresh();
    }

    get name() { return this.#includedList.name; }
    set name(v) { this.#includedList.name = v; }

    get required() { return this.#includedList.required; }
    set required(v) { this.#includedList.required = v; }

    get size() { return this.#includedList.size; }
    set size(v) { this.#includedList.size = v; }

    get value() { return this.#includedList.value; }
    set value(v) { this.#includedList.value = v; }

    get form() { return this.#includedList.form }
    get length() { return this.#includedList.length; }
    get multiple() { return true; }
    get options() { return this.#availableList.options; }
    get type() { return this.#includedList.type; }
    get validationMessage() { return this.#includedList.validationMessage; }
    get validity() { return this.#includedList.validity; }
    get willValidate() { return this.#includedList.willValidate; }

    static install() {
        return singleton("Juniper::Widgets::InclusionListElement", () => registerFactory("inclusion-list", InclusionListElement));
    }
}

export function InclusionList<T>(...rest: ElementChild<InclusionListElement<T>>[]): InclusionListElement<T> {
    return (InclusionListElement.install() as any)(...rest) as InclusionListElement<T>;
}
