import {
    always, alwaysTrue, boolCallback, colorNameToHex, compareBy, cssRgbToHex,
    distinct, FieldName,
    formatUSDate,
    identity,
    isDefined,
    isNullOrUndefined,
    isNumber, isObject, isString,
    objectSelect, stringCallback, unknownCallback
} from "@juniper-lib/util";
import {
    A, Button, Checked, ClassList, ClassName, Clear, ClearCss, CustomData, DataList,
    Details, Disabled, Div, ElementChild, FAIcon, FormAttr, HRef,
    HtmlAttr,
    HtmlRender,
    HtmlStyleAttr,
    HtmlTag,
    InnerHTML, InputCheckbox, InputColor, InputNumber, InputText, LI, ListAttr, Max, Min,
    OnClick,
    OnInput, Open, Option, PlaceHolder, RawElementChild, Scope, Select, SpanTag, Step,
    StyleAttr, TD, TH, Time, TitleAttr, UL, Value
} from "@juniper-lib/dom";
import { moneyToFinancialHTMLDOM } from "../dollarFormatting";
import { InputDateRange, InputDateRangeElement } from "../InputDateRangeElement";
import { SelectPlaceholder } from "../widgets";
import { BaseColumnDef, ColumnType } from "./types";

const NONE_OPTION_VALUE = "xxx_none_xxx";

export type ColumnDef<T> = BaseColumnDef<T> & {
    typeEnum: string;
    styleType: string | Record<number | string, string>;
    styleField: string;
    group: string;
    title: string;
    event: string;
    icon: string;
    sortable: boolean;
    filterable: boolean;
    exportable: boolean;
    hidden: boolean;
    ellipsis: boolean;
};

export class DataTableItemEvent<T> extends Event {

    constructor(event: string, item: T) {
        super(event);
        this.#item = item;
    }

    #item: T;
    get item(): T { return this.#item; }
}

export function isDataTableItemEvent<T>(evt: Event): evt is DataTableItemEvent<T> {
    return evt instanceof DataTableItemEvent;
}

const NO_COLOR = "#000001";

export function DataColumn(...rest: ElementChild[]) {
    return HtmlTag("data-column", ...rest);
}

export function DataColumnGroup(...rest: ElementChild[]) {
    return HtmlTag("data-column-group", ...rest)
}

export function NameAttr(name: string) {
    return new HtmlAttr("name", name);
}

export function FieldAttr(field: string) {
    return new HtmlAttr("field", field);
}

export function HeaderAttr(header: string) {
    return new HtmlAttr("header", header);
}

export function EventAttr(event: string) {
    return new HtmlAttr("event", event);
}

export class DataTableColumnElement<T> extends EventTarget {

    #header: HTMLElement;
    get header(): HTMLElement { return this.#header; }

    #getter: unknownCallback<T>;
    get getValue(): unknownCallback<T> { return this.#getter; }

    #sortGetter: unknownCallback<T>;


    #styleGetter: ((item: T) => (HtmlStyleAttr | ReturnType<typeof ClassName>)) = always(null);


    #filter: HTMLInputElement | HTMLSelectElement | InputDateRangeElement;
    get filter() { return this.#filter; }
    get filterActive(): boolean {
        if (!this.#filter) {
            return false;
        }

        return this.#filter.value !== "";
    }

    #validator: boolCallback<T>;

    #formatter: (item: T) => ElementChild;

    #dateFormatter: stringCallback<Date>;
    get dateFormatter(): stringCallback<Date> { return this.#dateFormatter; }
    set dateFormatter(v: stringCallback<Date>) {
        this.#dateFormatter = v;
        if (this.#filter instanceof InputDateRangeElement) {
            this.#filter.dateFormatter = v;
        }
    }

    #type: ColumnType;
    get type() { return this.#type; }

    #typeValue: any[] | Record<string, any>;
    #styleType: string | Record<string, string>;

    #field: FieldName<T>;
    get field() { return this.#field; }

    #styleField: string;

    #group: string;
    get group() { return this.#group; }

    #groupStyle: string;

    #event: string;
    get event() { return this.#event; }
    eventEnabled = true;

    #dataList: HTMLDataListElement;
    #sortable: boolean;

    #filterable: boolean;
    get filterable() { return this.#filterable; }

    #exportable: boolean;
    get exportable() { return this.#exportable; }

    #ellipsis: boolean;
    get ellipsis() { return this.#ellipsis; }

    #hidden: boolean;
    get hidden() { return this.#hidden; }

    #longName: string;
    get longName() { return this.#longName; }

    #shortName: string;
    get shortName() { return this.#shortName; }

    constructor(columnDef: ColumnDef<T>,
        groupStyle: string,
        form: HTMLFormElement,
        enumerations: Record<string, Record<string | number, any>>,
        validators: Record<string, boolCallback<T>>) {
        super();

        this.#dateFormatter = formatUSDate;

        const {
            type,
            typeEnum,
            styleType: styleTypeStr,
            field,
            styleField,
            group,
            title,
            header,
            event,
            icon,
            hidden,
            sortable,
            filterable,
            exportable,
            ellipsis
        } = columnDef;

        this.#type = type;
        this.#field = field;
        this.#styleField = styleField;
        this.#group = group;
        this.#groupStyle = groupStyle;
        this.#event = event;
        this.#hidden = hidden;
        this.#sortable = sortable;
        this.#filterable = filterable;
        this.#exportable = exportable;
        this.#ellipsis = ellipsis;

        this.#validator = alwaysTrue;
        this.#longName = title ?? header;
        this.#shortName = header ?? title;

        this.#header = SpanTag(this.#shortName);
        if (this.#longName && this.#longName !== this.#shortName) {
            this.#header.title = this.#longName;
        }

        if (this.#type === "enum"
            && typeEnum
            && typeof typeEnum === "string"
            && enumerations) {
            this.#typeValue = enumerations[typeEnum];
        }

        this.#styleType = styleTypeStr
            && typeof styleTypeStr === "string"
            && enumerations
            && enumerations[styleTypeStr]
            || styleTypeStr;

        if (this.#styleField) {
            this.#styleGetter = (item: T) => {
                const styleValue = item
                    && this.#styleField
                    && (item as any)[this.#styleField]
                    || null;

                const style = styleValue
                    && this.#styleType
                    && (this.#styleType as any)[styleValue];

                if (isNullOrUndefined(style)) {
                    return null;
                }

                if (typeof style === "string") {
                    return ClassName(style);
                }

                return StyleAttr(style);
            };
        }

        if (this.#field) {
            if (this.#sortable) {
                const sortEvt = new Event("sort");
                this.#header.removeAttribute("title");
                this.#header = A(
                    ClassList("table-sort-link", groupStyle),
                    CustomData("field-name", this.#field),
                    TitleAttr(`Sort by ${this.#longName} (ascending)`),
                    OnClick(() => {
                        this.#incrementSort();
                        return this.dispatchEvent(sortEvt);
                    }),
                    this.#header,
                    FAIcon("sort")
                );
            }

            this.#getter = (item: T) => objectSelect(item, this.#field);

            this.#sortGetter = this.#getter;
            this.#formatter = (item: T) => this.#getter(item) as RawElementChild || "";
            if (this.#field in validators) {
                this.#validator = validators[this.#field];
            }


            if (this.#type === "date") {
                this.#formatter = (item: T) => {
                    let value = this.#getter(item) as Date | string | number;
                    if (isString(value) || isNumber(value)) {
                        value = new Date(value);
                    }

                    try {
                        return Time(value, this.#dateFormatter(value));
                    }
                    catch (err) {
                        console.log("Formatting", value, item, err);
                        return null;
                    }
                };
            }
            else if (this.#type === "html") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as string;
                    return InnerHTML(value);
                };
            }
            else if (this.#type === "link") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as string;
                    return A(HRef(value), value);
                }
            }
            else if (this.#type === "number") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item);
                    return value?.toString();
                };
            }
            else if (this.#type === "dollars") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as (string | number);
                    return moneyToFinancialHTMLDOM(value);
                };
            }
            else if (this.#type === "percent") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as number;
                    if (Number.isNaN(value)) {
                        return "--";
                    }
                    else {
                        return Math.round(100 * value) + "%";
                    }
                };
            }
            else if (this.#type === "integer") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as number;
                    return value?.toFixed();
                };
            }
            else if (this.#type === "boolean") {
                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as boolean;
                    return Div(
                        ClassList("form-check", "form-switch"),
                        InputCheckbox(
                            ClassList("form-check-input"),
                            Checked(value),
                            Disabled(true),
                            TitleAttr(value ? "Archived" : "Live")
                        )
                    );
                };
            }
            else if (this.#type === "color") {
                const getter = this.#getter;
                this.#getter = (item: T) => {
                    const color = getter(item);
                    return colorNameToHex(getter(item) as string)
                        || color;
                };

                this.#formatter = (item: T) => {
                    const value = this.#getter(item) as string;
                    return Div(
                        ClassList("color-cell"),
                        StyleAttr({
                            "background-color": value
                        })
                    );
                };
            }
            else if (this.#type === "array") {
                this.#formatter = (item: T) => {
                    const values = this.#getter(item) as Array<ElementChild>;
                    return Details(
                        Open(this.filterActive),
                        UL(
                            ...values.map(v => {
                                if (this.filterActive) {
                                    const index = v?.toString()?.indexOf(this.filter.value) ?? -1;
                                    const textDecoration = index > -1 ? "underline" : "none";
                                    return LI(
                                        StyleAttr({ "text-decoration": textDecoration }),
                                        v
                                    );
                                }
                                else {
                                    return LI(v);
                                }
                            })
                        )
                    );
                };

                this.#sortGetter = item => {
                    const values = this.#getter(item) as Array<ElementChild>;
                    return values.map(v => v?.toString())?.join(", ");
                };
            }
            else if (this.#type === "csv") {
                this.#formatter = (item: T) => {
                    const values = this.#getter(item) as Array<ElementChild>;
                    const tag = SpanTag();
                    for (let i = 0; i < values.length; ++i) {
                        if (i > 0) {
                            tag.append(", ");
                        }
                        const v = values[i];
                        if (this.filterActive) {
                            const index = v?.toString()?.indexOf(this.filter.value) ?? -1;
                            const textDecoration = index > -1 ? "underline" : "none";
                            tag.append(SpanTag(
                                StyleAttr({ "text-decoration": textDecoration }),
                                v
                            ));
                        }
                        else {
                            tag.append(SpanTag(v));
                        }
                    }
                    return tag;
                };

                this.#sortGetter = item => {
                    const values = this.#getter(item) as Array<ElementChild>;
                    return values.map(v => v?.toString())?.join(", ");
                };
            }

            if (this.#filterable) {
                const filterEvt = new Event("filter");
                const onFilter = () =>
                    this.dispatchEvent(filterEvt);

                if (this.#type === "enum" && this.#typeValue instanceof Array) {
                    this.#filter = Select(
                        FormAttr(form),
                        ClassList("form-select", "form-select-sm"),
                        CustomData("field-name", this.#field),
                        this.#shortName && SelectPlaceholder(this.#shortName) || null,
                        this.#longName && TitleAttr(`Filter ${this.#longName}`) || null,
                        OnInput(onFilter),
                        Option(Value(NONE_OPTION_VALUE), "None"),
                        ...this.#typeValue.map(v => Option(v))
                    );
                }
                else if (this.#type === "date") {
                    this.#filter = InputDateRange(
                        FormAttr(form),
                        ClassList("form-control", "form-control-sm"),
                        CustomData("field-name", this.#field),
                        PlaceHolder(`--${this.#shortName}--`),
                        TitleAttr("Filter " + this.#longName),
                        OnInput(onFilter)
                    );
                }
                else if (this.#type === "number"
                    || this.#type === "integer"
                    || this.#type === "dollars") {
                    this.#filter = InputNumber(
                        FormAttr(form),
                        ClassList("form-control", "form-control-sm"),
                        CustomData("field-name", this.#field),
                        PlaceHolder(`--${this.#shortName}--`),
                        TitleAttr("Filter " + this.#longName),
                        OnInput(onFilter),
                        ListAttr(this.#dataList = DataList())
                    );
                }
                else if (this.#type === "percent") {
                    this.#filter = InputNumber(
                        FormAttr(form),
                        ClassList("form-control", "form-control-sm"),
                        CustomData("field-name", this.#field),
                        PlaceHolder(`--${this.#shortName}--`),
                        TitleAttr("Filter " + this.#longName),
                        Min(0),
                        Max(100),
                        Step(1),
                        OnInput(onFilter)
                    );
                }
                else if (this.#type === "boolean") {
                    this.#filter = Select(
                        FormAttr(form),
                        ClassList("form-select", "form-select-sm"),
                        CustomData("field-name", this.#field),
                        this.#longName && TitleAttr(`Filter ${this.#longName}`) || null,
                        OnInput(onFilter),
                        Option(""),
                        Option("YES", Value("true")),
                        Option("NO", Value("false"))
                    );
                }
                else if (this.#type === "color") {
                    this.#filter = InputColor(
                        FormAttr(form),
                        ClassList("form-control", "form-control-sm"),
                        CustomData("filed-name", this.#field),
                        PlaceHolder(`--${this.#shortName}--`),
                        TitleAttr("Filter " + this.#longName),
                        ListAttr(this.#dataList = DataList()),
                        Value(NO_COLOR),
                        OnInput(onFilter)
                    );

                    form.addEventListener("reset", () =>
                        this.#filter.value = NO_COLOR);
                }
                else {
                    this.#filter = InputText(
                        FormAttr(form),
                        ClassList("form-control", "form-control-sm"),
                        CustomData("field-name", this.#field),
                        PlaceHolder(`--${this.#shortName}--`),
                        TitleAttr("Filter " + this.#longName),
                        OnInput(onFilter),
                        ListAttr(this.#dataList = DataList())
                    );
                }
            }
        }
        else if (this.#type === "button") {
            this.#formatter = (item: T) =>
                Button(
                    ClassList("btn", "borderless"),
                    FAIcon(icon),
                    TitleAttr(this.#longName),
                    Disabled(!this.eventEnabled),
                    OnClick(() => {
                        if (this.eventEnabled) {
                            this.dispatchEvent(new DataTableItemEvent(event, item));
                        }
                    })
                );
        }

        this.#getter = this.#addErrorHandler("getter", this.#getter);
        this.#sortGetter = this.#addErrorHandler("sortGetter", this.#sortGetter);
        this.#styleGetter = this.#addErrorHandler("styleGetter", this.#styleGetter);
        this.#validator = this.#addErrorHandler("validator", this.#validator);
        this.#formatter = this.#addErrorHandler("formatter", this.#formatter);
        this.#dateFormatter = this.#addErrorHandler("dateFormatter", this.#dateFormatter);
    }

    #addErrorHandler<U, V>(name: string, handler: (obj: U) => V): (obj: U) => V {
        if (!handler) {
            return null;
        }

        return value => {
            try {
                return handler(value);
            }
            catch (error) {
                console.error({
                    name,
                    type: this.#type,
                    field: this.#field,
                    value,
                    error,
                    column: this
                });
                throw error;
            }
        };
    }

    #validateCell(cell: HTMLTableCellElement, item: T) {
        cell.classList.toggle(
            "warning",
            !this.#validator(item)
        );
    }

    get #canSort() {
        return this.#sortable
            && isDefined(this.#field);
    }

    get #sortIcon() {
        const icon = this.#header.querySelector(".fa")
            || this.#header.querySelector(".svg-inline--fa");
        return icon;
    }

    #incrementSort() {
        if (this.#canSort) {
            if (this.#sortIcon.classList.contains("fa-sort-down")) {
                this.#sortIcon.classList.remove("fa-sort-up", "fa-sort-down");
                this.#sortIcon.classList.add("fa-sort");
                this.#header.title = `Sort by ${this.#longName} (ascending)`;
            }
            else if (this.#sortIcon.classList.contains("fa-sort-up")) {
                this.#sortIcon.classList.remove("fa-sort", "fa-sort-up");
                this.#sortIcon.classList.add("fa-sort-down");
                this.#header.title = `Clear sort by ${this.#longName}`;
            }
            else {
                this.#sortIcon.classList.remove("fa-sort", "fa-sort-down");
                this.#sortIcon.classList.add("fa-sort-up");
                this.#header.title = `Sort by ${this.#longName} (descending)`;
            }
        }
    }

    updateFilters(data: T[]) {
        if (this.#dataList) {
            let values: Array<ElementChild> = [];
            if (this.#type === "array") {
                values = data.flatMap(v => this.#getter(v) as ElementChild);
            }
            else {
                values = data.map(v => this.#getter(v) as ElementChild);
            }

            values = distinct(values)
                .filter(identity)
                .sort(compareBy(identity));

            HtmlRender(
                this.#dataList,
                Clear(),
                this.#type === "color" && Option(NO_COLOR) || null,
                Option(Value(NONE_OPTION_VALUE), "None"),
                ...values.map(v => Option(v))
            );
        }
    }

    updateCell(cell: HTMLTableCellElement, item: T) {
        let content = this.#formatter(item);

        if (this.ellipsis) {
            const wrapper = Div(content);
            wrapper.classList.add("data-table-ellipsis-cell");
            const value = this.#getter(item);
            if (isString(value)) {
                wrapper.title = value;
            }
            else if (isObject(value) && "toString" in value) {
                wrapper.title = value.toString();
            }

            content = wrapper;
        }

        HtmlRender(
            cell,
            Clear(),
            ClearCss(),
            this.#styleGetter(item),
            content
        );

        if (this.#type === "dollars"
            || this.#type === "percent") {
            cell.style.textAlign = "right";
        }
        else if (this.#type === "button") {
            cell.style.textAlign = "center";
        }

        this.#validateCell(cell, item);
    }

    resetSort() {
        if (this.#canSort) {
            this.#sortIcon.classList.remove("fa-sort-up", "fa-sort-down");
            this.#sortIcon.classList.add("fa-sort");
            this.#header.title = `Sort by ${this.#longName} (ascending)`;
        }
    }

    get sorter() {
        if (!this.#canSort) {
            return null;
        }

        const icon = this.#header.querySelector(".fa")
            || this.#header.querySelector(".svg-inline--fa");

        if (icon.classList.contains("fa-sort")) {
            return null;
        }

        return compareBy(icon.classList.contains("fa-sort-up"), this.#sortGetter);
    }

    makeSorterCell() {
        const headerCell = TH(
            Scope("col"),
            ClassList(this.#groupStyle),
            this.#header
        );

        if (this.#type === "dollars"
            || this.#type === "percent") {
            headerCell.style.textAlign = "right";
        }

        return headerCell;
    }

    makeFilterCell() {
        return TH(
            ClassList(this.#groupStyle),
            this.#filter
        );
    }

    setFilterValue(value: string) {
        if (this.#filter) {
            this.#filter.value = value;
        }
    }

    get filterer() {
        if (!(this.#filter instanceof HTMLInputElement
            || this.#filter instanceof HTMLSelectElement
            || this.#filter instanceof InputDateRangeElement)
            || this.#filter.value.trim().length === 0) {
            return null;
        }

        if (this.#filter instanceof InputDateRangeElement) {
            let [min, max] = this.#filter.valuesAsDates;
            if (isNullOrUndefined(min)) {
                min = new Date(0);
            }
            if (isNullOrUndefined(max)) {
                max = new Date(8.64e15);
            }

            const filter: boolCallback<T> = item => {
                const formatted = this.#formatter(item) as HTMLTimeElement;
                const date = new Date(formatted.dateTime);
                return min <= date && date <= max;
            };

            return filter;
        }
        else {
            const filterValue = this.#filter.value.toLowerCase();
            if (this.#type === "color") {
                const filter: boolCallback<T> = item => {
                    if (!filterValue || filterValue === NO_COLOR) {
                        return true;
                    }
                    const formatted = this.#formatter(item) as HTMLElement;
                    return filterValue === cssRgbToHex(formatted.style.backgroundColor);
                };

                return filter;
            }
            else if (this.#type === "boolean") {
                if (filterValue === "") {
                    return alwaysTrue;
                }
                else {
                    const boolFilterValue = filterValue === "true";
                    const filter: boolCallback<T> = item => {
                        const boolValue = this.#getter(item);
                        return boolValue === boolFilterValue;
                    };

                    return filter;
                }
            }
            else {
                const filter: boolCallback<T> = item => {
                    const formatted = this.#formatter(item);
                    try {
                        const text = (formatted instanceof HTMLElement
                            ? formatted.textContent
                            : formatted as string)
                            ?.toString()
                            ?.trim()
                            ?.toLowerCase();

                        return filterValue === NONE_OPTION_VALUE && text.length === 0
                            || filterValue !== NONE_OPTION_VALUE && text.indexOf(filterValue) >= 0;
                    }
                    catch (err) {
                        console.error(formatted, err);
                        return false;
                    }
                };

                return filter;
            }
        }
    }

    searchMatch(item: T, value: any) {
        if (!this.filterable) {
            return false;
        }

        let isMatch = false;
        const oldValue = this.#filter.value;
        this.#filter.value = value;
        const filterer = this.filterer;
        if (filterer) {
            isMatch = filterer(item);
        }

        if (!isMatch && value.length > 0) {
            this.#filter.value = oldValue;
        }

        return isMatch;
    }

    makeItemCell(item: T) {
        const cell = TD(ClassList(this.#groupStyle));
        this.updateCell(cell, item);
        return cell;
    }
}