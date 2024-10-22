import { always, arrayGetRunLengths, arrayInsert, arrayRemove, arrayReplace, binarySearch, debounce, filterJoin, formatUSDate, isDefined, isNullOrUndefined, last, singleton, stringRandom } from "@juniper-lib/util";
import { alignItems, backgroundColor, border, Button, ButtonReset, ClassList, ClassName, Clear, color, ColSpan, cursor, display, Div, elementSetDisplay, FAIcon, filter, fontSize, FormAttr, FormTag, gridTemplateColumns, gridTemplateRows, height, hide, HtmlProp, HtmlRender, ID, isModifierless, justifyContent, marginBottom, marginLeft, OnClick, OnReset, OnScroll, overflow, padding, px, QueryAll, registerFactory, rule, show, SingletonStyleBlob, StyleAttr, Table, TBody, TD, textAlign, textDecoration, textOverflow, textWrap, TFoot, TH, THead, TitleAttr, TR, TypedHTMLElement, visibility, width } from "@juniper-lib/dom";
import { Paginator, PaginatorView } from "../Paginator";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { DataTableColumnElement, DataTableItemEvent, isDataTableItemEvent } from "./DataTableColumn";
const PaginationValues = ["on", "off", "auto"];
function isPaginationValue(value) {
    return PaginationValues.indexOf(value) >= 0;
}
export function DefaultSort(defaultSort) {
    return new HtmlProp("defaultSort", defaultSort);
}
export function Enumerations(enumerations) {
    return new HtmlProp("enumerations", enumerations);
}
export function Validators(validators) {
    return new HtmlProp("validators", validators);
}
export class DataTableElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "paginated"
    ]; }
    #rootData = [];
    get data() { return this.#rootData; }
    set data(items) {
        if (items !== this.#rootData) {
            arrayReplace(this.#rootData, items);
            this.#renderData();
        }
    }
    #selectedValue;
    get selectedValue() { return this.#selectedValue; }
    set selectedValue(v) {
        if (v !== this.#selectedValue) {
            const oldSelectedRow = this.#itemsToRows.get(this.#selectedValue);
            if (oldSelectedRow) {
                oldSelectedRow.classList.remove("selected");
            }
            this.#selectedValue = v;
            this.#paginator.select(v);
            this.#renderBody();
        }
        // This needs to be outside of the IF-statement
        // so that clicking a currently-selected row still
        // fires the "selected" event and our UI can respond.
        this.dispatchEvent(new TypedItemSelectedEvent(v));
    }
    #groups = [];
    #headerGroups;
    #lastGroupStyle;
    #columns = [];
    #visibleColumns = [];
    #paginator;
    get paginated() {
        const v = this.getAttribute("paginated");
        if (isPaginationValue(v)) {
            return v;
        }
        else {
            return "on";
        }
    }
    set paginated(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("paginated");
        }
        else {
            if (!isPaginationValue(v)) {
                v = "on";
            }
            this.setAttribute("paginated", v);
        }
    }
    get autoPaginated() { return this.#paginator.autoSized; }
    set autoPaginated(v) { this.#paginator.autoSized = v; }
    get filteredData() { return this.#paginator.data; }
    get resultsPerPage() { return this.#paginator.resultsPerPage; }
    set resultsPerPage(v) { this.#paginator.resultsPerPage = v; }
    #form;
    #footTable;
    #headTable;
    #headerGroupRow;
    #headerLabelRow;
    #headerFilterRow;
    #mainTable;
    #resultsBody;
    #noResultsBody;
    #scrollOuter;
    #scrollInner;
    #itemsToRows = new Map();
    #resizeHeader;
    #resizePage;
    #resetFiltersButton1;
    #resetFiltersButton2;
    #resetSortButton;
    #curSort;
    #defaultSort;
    get defaultSort() { return this.#defaultSort; }
    set defaultSort(v) {
        if (v !== this.#defaultSort) {
            this.#curSort
                = this.#defaultSort
                    = v;
            this.#paginator.sort(this.#curSort);
        }
    }
    #dateFormatter;
    get dateFormatter() { return this.#dateFormatter; }
    set dateFormatter(v) {
        if (v !== this.#dateFormatter) {
            this.#dateFormatter = v;
            for (const column of this.#columns) {
                column.dateFormatter = v;
            }
            this.#renderData();
        }
    }
    #enumerations = {};
    get enumerations() { return this.#enumerations; }
    set enumerations(v) {
        if (v !== this.#enumerations) {
            this.#enumerations = v;
            this.#renderHeader();
        }
    }
    #validators = {};
    get validators() { return this.#validators; }
    set validators(v) {
        if (v !== this.#validators) {
            this.#validators = v;
            this.#renderBody();
        }
    }
    constructor() {
        SingletonStyleBlob("Juniper:Widgets:DataTable", () => [
            rule("data-column-group, data-column, data-table > form", display("none")),
            rule(".data-table-ellipsis-cell", width("25em"), overflow("hidden"), textOverflow("ellipsis"), textWrap("nowrap")),
            rule("data-table", display("grid"), gridTemplateRows("1fr", "auto"), overflow("hidden"), rule(" table,  table.table", marginBottom(0)), rule(" .header-container", overflow("hidden"), rule(">table", rule(" th", textAlign("center")), rule(" th:last-child", padding(0)))), rule(" .pagination", justifyContent("center"), color("inherit")), rule(" .scroll-outer", display("grid"), gridTemplateRows("auto", "1fr"), overflow("hidden")), rule(" .scroll-inner", overflow("auto"), rule(" thead", visibility("collapse"))), rule(" .no-results", fontSize("24pt"), textAlign("center")), rule(" .table-sort-link", display("inline-grid"), gridTemplateColumns("1fr", "auto"), alignItems("center"), textDecoration("none"), cursor("pointer")), rule(" .table-sort-link", rule("> .svg-inline--fa, > .fa", marginLeft(".5rem"))), rule(" .selected", border("dashed 2px white")), rule(" .warning", backgroundColor("#ee6c4d99 !important")), rule(" th.alt-group", filter("contrast(.8) brightness(1.2)")), rule(" td:not(.alt-group)", filter("contrast(1.4) brightness(.95)")), rule(" td button.btn", color("currentcolor")), rule(" .color-cell", width("5em"), height("5em")))
        ]);
        super();
        this.#dateFormatter = formatUSDate;
        this.#resizeHeader = debounce(this.#_resizeHeader.bind(this));
        this.#resizePage = debounce(this.#_resizePage.bind(this));
        const paginatorView = PaginatorView();
        this.#paginator = new Paginator([paginatorView]);
        this.#paginator.addEventListener("paginated", () => this.#renderBody());
        this.defaultSort = always(0);
        this.#form = FormTag(ID(stringRandom(12)), OnReset(debounce(() => this.filter())));
        this.#scrollInner = Div(ClassList("scroll-inner"), OnScroll(() => this.#scrollHeader()), this.#mainTable = Table(
        // Clone the table header so the sizing works right, because the filter
        // input elements have minimum sizes that restrict the width of the
        // columns more than the column content itself.
        THead(), this.#resultsBody = TBody(StyleAttr({ display: "none" })), this.#noResultsBody = TBody(ClassList("no-results"), TR(TD("No results ", this.#resetFiltersButton2 = ButtonReset(ClassList("btn", "btn-sm", "btn-secondary"), FormAttr(this.#form), "Reset filters"))))));
        this.#scrollOuter = Div(ClassList("scroll-outer"), Div(ClassList("header-container"), this.#headTable = Table(THead(this.#headerGroupRow = TR(), this.#headerLabelRow = TR(), this.#headerFilterRow = TR()))), this.#scrollInner);
        this.#resetSortButton = Button(StyleAttr({ display: "none" }), FAIcon("x"), TitleAttr("Reset sort"), ClassList("btn", "btn-sm", "btn-secondary"), OnClick(() => this.#compareBy(null)));
        this.#resetFiltersButton1 = ButtonReset(StyleAttr({ display: "none" }), FAIcon("x"), FormAttr(this.#form), TitleAttr("Reset filters"), ClassList("btn", "btn-sm", "btn-secondary"));
        this.#footTable = Table(ClassName(this.#mainTable.className), TFoot(TR(TD(paginatorView))));
    }
    get pageSize() { return parseFloat(this.getAttribute("pagesize")) || 25; }
    set pageSize(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("pagesize");
        }
        else {
            if (v <= 0) {
                v = 25;
            }
            this.setAttribute("pagesize", v.toFixed());
        }
    }
    #checkAttribute(name) {
        this.attributeChangedCallback(name, null, this.getAttribute(name));
    }
    connectedCallback() {
        this.#paginator.resultsPerPage = this.pageSize;
        this.#checkAttribute("paginated");
        const tableClasses = [...this.classList]
            .filter(c => c.indexOf("table") === 0);
        this.#mainTable.className
            = this.#headTable.className
                = this.#footTable.className
                    = tableClasses.join(" ");
        this.classList.remove(...tableClasses.filter(c => c !== "table-primary"));
        this.append(this.#form, this.#scrollOuter, this.#footTable);
        const { resizer, intersector } = singleton("Juniper:Widgets:DataTable:connectedCallback", () => {
            function doResize(entries) {
                for (const entry of entries) {
                    if (entry.target instanceof DataTableElement) {
                        entry.target.resize();
                    }
                }
            }
            const resizer = new ResizeObserver(doResize);
            const intersector = new IntersectionObserver(doResize, { root: document.documentElement });
            return { resizer, intersector };
        });
        resizer.observe(this);
        intersector.observe(this);
        const mutater = new MutationObserver((mutations => {
            for (const mutation of mutations) {
                if (mutation.attributeName === "style") {
                    this.#onResize();
                }
                if (Array.from(mutation.addedNodes).filter(n => n instanceof HTMLElement &&
                    n.tagName === "DATA-COLUMN").length > 0) {
                    this.#renderHeader();
                }
            }
        }));
        mutater.observe(this, {
            attributes: true,
            childList: true
        });
        this.#renderHeader();
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name) {
            case "paginated":
                this.#paginator.disabled = newValue === "off";
                this.#paginator.autoSized = newValue === "auto";
                break;
        }
    }
    #renderHeader() {
        /////////////////////////////////
        // Build the column groups
        /////////////////////////////////
        const columnDefs = QueryAll(this, "data-column")
            .map(col => {
            const type = col.getAttribute("type");
            const typeEnum = col.getAttribute("typeenum");
            const styleType = col.getAttribute("styles");
            const field = col.getAttribute("field");
            const styleField = col.getAttribute("stylefield");
            const title = col.getAttribute("title");
            const header = col.getAttribute("header");
            const event = col.getAttribute("event");
            const icon = col.getAttribute("icon");
            const isDataField = type !== "none" && type !== "button";
            const checkBool = (key, onByDefault) => (onByDefault || col.hasAttribute(key))
                && col.getAttribute(key) !== "off";
            const hidden = checkBool("hidden", false);
            const sortable = !hidden && isDataField && checkBool("sort", true);
            const filterable = isDataField && checkBool("filter", true);
            const exportable = isDataField && checkBool("export", true);
            const ellipsis = isDataField && checkBool("ellipsis", false);
            const group = col.parentElement
                && col.parentElement.tagName === "DATA-COLUMN-GROUP"
                && col.parentElement.getAttribute("name")
                || "";
            return {
                type,
                typeEnum,
                styleType,
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
            };
        });
        this.#headerGroups = arrayGetRunLengths(columnDefs
            .filter(v => !v.hidden)
            .map(v => v.group));
        arrayReplace(this.#groups, this.#headerGroups.map(v => v[0]));
        this.#lastGroupStyle = this.#getGroupStyle(last(this.#groups));
        const allColumns = columnDefs.map(columnDef => {
            const column = new DataTableColumnElement(columnDef, this.#getGroupStyle(columnDef.group), this.#form, this.#enumerations, this.#validators);
            column.dateFormatter = this.#dateFormatter;
            column.addEventListener("filter", () => this.filter());
            column.addEventListener("sort", () => this.#compareBy(column));
            column.addEventListener(column.event, evt => {
                if (isDataTableItemEvent(evt)) {
                    this.selectedValue = evt.item;
                    this.dispatchEvent(new DataTableItemEvent(evt.type, evt.item));
                }
            });
            return column;
        });
        arrayReplace(this.#columns, allColumns);
        arrayReplace(this.#visibleColumns, allColumns.filter(v => !v.hidden));
        /////////////////////////////////
        // Build the columns
        /////////////////////////////////
        const hasHeaderGroups = this.#headerGroups.length > 1
            || (this.#headerGroups.length === 1
                && this.#headerGroups[0][0].length > 0);
        HtmlRender(this.#headerGroupRow, Clear(), StyleAttr({
            color: "",
            display: hasHeaderGroups ? "" : "none"
        }), ...this.#headerGroups.map(([group, size]) => TH(ClassList(this.#getGroupStyle(group)), ColSpan(size), group)), 
        // A spacer for the filter/sort clear buttons below this row
        TH(ClassList(this.#lastGroupStyle)), 
        // need to make space for the scrollbar
        TH(ClassList(this.#lastGroupStyle)));
        HtmlRender(this.#headerLabelRow, Clear(), ...this.#visibleColumns.map(column => column.makeSorterCell()), TH(ClassList(this.#lastGroupStyle), this.#resetSortButton), 
        // need to make space for the scrollbar
        TH(ClassList(this.#lastGroupStyle)));
        const canFilter = this.#visibleColumns.filter(c => c.filterable).length > 0;
        HtmlRender(this.#headerFilterRow, Clear(), StyleAttr({
            display: canFilter ? "" : "none"
        }), ...this.#visibleColumns.map(column => column.makeFilterCell()), TH(ClassList(this.#lastGroupStyle), this.#resetFiltersButton1), 
        // need to make space for the scrollbar
        TH(ClassList(this.#lastGroupStyle)));
        this.#noResultsBody.querySelector("td").colSpan = this.#visibleColumns.length + 1;
        this.#mainTable.tHead.replaceWith(this.#headTable.tHead.cloneNode(true));
        for (const row of this.#mainTable.tHead.rows) {
            const scrollSpacerCell = last(row.cells);
            scrollSpacerCell.remove();
            const resetButtonCell = last(row.cells);
            resetButtonCell.style.width = "45px";
        }
        this.#renderData();
    }
    #renderData() {
        for (const column of this.#columns) {
            column.updateFilters(this.#rootData);
        }
        this.filter();
    }
    #renderBody() {
        this.#itemsToRows.clear();
        const pageData = this.#paginator.pageData;
        HtmlRender(this.#resultsBody, Clear(), ...pageData
            .map(value => this.#renderRow(value)));
        if (pageData.indexOf(this.selectedValue) >= 0) {
            const selectedRow = this.#itemsToRows.get(this.selectedValue);
            selectedRow.scrollIntoView({
                behavior: "smooth",
                block: "center",
                inline: "center"
            });
        }
        this.#resizeHeader();
    }
    #listeners = new Map();
    on(type, handler) {
        const listener = (evt) => {
            if (evt instanceof DataTableItemEvent && evt.type === type) {
                handler(evt.item);
            }
        };
        this.#listeners.set(handler, listener);
        /*
         May have to check this had it as `this.addEventListener(type, listener);` but was running into some issues
        */
        this.addEventListener(type, listener);
    }
    off(type, handler) {
        const listener = this.#listeners.get(handler);
        if (listener) {
            this.removeEventListener(type, listener);
            this.#listeners.delete(handler);
        }
    }
    enableEvent(eventName, enabled) {
        for (const column of this.#columns) {
            if (column.type === "button" && column.event === eventName) {
                column.eventEnabled = enabled;
            }
        }
        this.#renderBody();
    }
    #renderRow(item) {
        const row = TR(item === this.#selectedValue && ClassList("selected") || null, ...this.#visibleColumns.map(column => column.makeItemCell(item)), 
        // needed to make space for the sort/filter clear buttons in the header
        TD(ClassList(this.#lastGroupStyle)), OnClick((evt) => {
            if (isModifierless(evt)) {
                this.selectedValue = item;
            }
            else if (evt.ctrlKey) {
                this.selectedValue = null;
            }
        }));
        this.#itemsToRows.set(item, row);
        return row;
    }
    #scrollHeader() {
        const mainStyle = getComputedStyle(this.#mainTable);
        // I have no idea why this needs to be divided by 3. Seems like it should be 2.
        const diff = (this.#scrollInner.offsetWidth - this.#scrollInner.clientWidth) / 3;
        this.#headTable.style.marginLeft
            = `calc(${mainStyle.marginLeft} - ${this.#scrollInner.scrollLeft + diff}px)`;
    }
    #_resizeHeader() {
        this.#headTable.style.width
            = px(this.#mainTable.clientWidth);
        const sourceRow = this.#mainTable.tHead.rows[1];
        const destRow = this.#headTable.tHead.rows[1];
        if (sourceRow
            && destRow
            && sourceRow.cells
            && destRow.cells) {
            for (let i = 0; i < sourceRow.cells.length && i < destRow.cells.length; ++i) {
                const srcHeader = sourceRow.cells[i];
                const destHeader = destRow.cells[i];
                const srcStyle = getComputedStyle(srcHeader);
                destHeader.style.width = srcStyle.width;
            }
            this.#scrollHeader();
            this.#resizePage();
        }
    }
    #_resizePage() {
        const pageData = this.#paginator.pageData;
        if (this.#paginator.autoSized
            && pageData.length > 0
            && this.#mainTable.scrollHeight > 0) {
            const avgRowHeight = this.#mainTable.scrollHeight / pageData.length;
            const numVisible = Math.floor(this.#scrollInner.clientHeight / avgRowHeight);
            this.#paginator.resultsPerPage = numVisible;
        }
    }
    #onResize() {
        this.#resizeHeader();
        this.#scrollHeader();
        this.#resizePage();
    }
    resize() {
        this.#onResize();
    }
    #getGroupStyle(group) {
        return (this.#groups.indexOf(group) % 2) === 1
            ? "alt-group"
            : null;
    }
    #compareBy(column) {
        this.#columns
            .filter(c => c !== column)
            .forEach(c => c.resetSort());
        this.#curSort = column?.sorter || this.#defaultSort;
        if (this.#curSort) {
            this.#paginator.sort(this.#curSort);
            this.#paginator.select(this.selectedValue);
        }
        else {
            this.filter();
        }
        if (this.#curSort === this.#defaultSort) {
            hide(this.#resetSortButton);
        }
        else {
            show(this.#resetSortButton);
        }
    }
    filter() {
        const filters = this.#columns
            .map(column => column.filterer)
            .filter(isDefined);
        const expr = [
            isDefined,
            ...filters
        ].reduce(filterJoin);
        this.#paginator.data = this.#rootData.filter(expr);
        elementSetDisplay(this.#resultsBody, this.#paginator.data.length > 0, "table-row-group");
        elementSetDisplay(this.#noResultsBody, this.#paginator.data.length === 0, "table-row-group");
        elementSetDisplay(this.#resetFiltersButton1, filters.length > 0 && this.#rootData.length > 0);
        elementSetDisplay(this.#resetFiltersButton2, filters.length > 0 && this.#rootData.length > 0);
        this.#paginator.select(this.selectedValue);
    }
    getFilterValue(columnName) {
        for (const column of this.#columns) {
            if (column.field === columnName) {
                return column.filter.value;
            }
        }
        return null;
    }
    setFilterValue(columnName, filterValue) {
        for (const column of this.#columns) {
            if (column.field === columnName) {
                column.setFilterValue(filterValue);
                break;
            }
        }
        this.filter();
    }
    clearFilters() {
        this.#form.reset();
    }
    /**
     * Search the given fields (or all filterable fields if no fields are given)
     * of all of the data table items and set the filters to make matches.
     */
    #search(value, fields, breakOnFirstMatch) {
        if (fields.length === 0) {
            fields = this.#columns
                .map(c => c.field)
                .filter(isDefined);
        }
        const fieldSet = new Set(fields);
        const columns = this.#columns
            .filter(c => c.filterable
            && fieldSet.has(c.field));
        for (const column of columns) {
            let found = false;
            for (const item of this.data) {
                if (column.searchMatch(item, value)) {
                    found = true;
                    column.setFilterValue(value);
                    break;
                }
            }
            if (found && breakOnFirstMatch) {
                break;
            }
        }
        this.filter();
    }
    /**
     * Search the given fields (or all filterable fields if no fields are given)
     * of all of the data table items and set the filters to make matches.
     */
    searchAll(value, ...fields) {
        this.#search(value, fields, false);
    }
    /**
     * Search the given fields (or all filterable fields if no fields are given)
     * of all of the data table items and set the filters to make matches.
     */
    searchFirstMatch(value, ...fields) {
        this.#search(value, fields, true);
    }
    update(item) {
        for (const column of this.#columns) {
            column.updateFilters(this.#rootData);
        }
        const row = this.#itemsToRows.get(item);
        if (row) {
            for (let i = 0; i < this.#visibleColumns.length; ++i) {
                const column = this.#visibleColumns[i];
                const cell = row.cells[i];
                column.updateCell(cell, item);
            }
        }
        this.#paginator.update(item);
    }
    addItem(item) {
        const index = this.#curSort
            ? binarySearch(this.#rootData, item, this.#curSort, "prepend")
            : this.#rootData.length;
        arrayInsert(this.#rootData, item, index);
        for (const column of this.#columns) {
            column.updateFilters(this.#rootData);
        }
        this.selectedValue = item;
        this.filter();
    }
    removeItem(item) {
        const row = this.#itemsToRows.get(item);
        if (row) {
            if (row.isConnected) {
                row.remove();
            }
            this.#itemsToRows.delete(item);
            arrayRemove(this.#rootData, item);
            for (const column of this.#columns) {
                column.updateFilters(this.#rootData);
            }
            if (this.selectedValue === item) {
                this.selectedValue = null;
            }
            this.filter();
        }
    }
    #getExportableColumns() {
        return this.#columns
            .filter(column => column.exportable)
            .map(c => ({
            field: c.field,
            type: c.type,
            header: c.shortName
        }));
    }
    #getExportData(allData) {
        const cols = this.#columns
            .filter(column => column.exportable);
        const data = allData
            ? this.#rootData
            : this.#paginator.data;
        return data.map(r => cols.map(c => c.getValue(r)));
    }
    getSheetDef(sheetName, allData) {
        return {
            name: sheetName,
            columns: this.#getExportableColumns(),
            data: this.#getExportData(allData)
        };
    }
    #alterColumn(columnIndex, act) {
        const columnDef = this.querySelector(`data-column:nth-child(${columnIndex + 1})`);
        act(columnDef);
    }
    #setColumnDefField(columnIndex, columnFieldName, columnFieldValue) {
        this.#alterColumn(columnIndex, columnDef => {
            if (isNullOrUndefined(columnFieldValue)) {
                columnDef.removeAttribute(columnFieldName);
            }
            else {
                columnDef.setAttribute(columnFieldName, columnFieldValue);
            }
        });
    }
    #columnAlterer = {
        setVisible: (columnIndex, visible) => {
            this.#alterColumn(columnIndex, columnDef => columnDef.toggleAttribute("hidden", !visible));
        },
        setHeader: (columnIndex, header) => {
            this.#setColumnDefField(columnIndex, "header", header);
        },
        setField: (columnIndex, field) => {
            this.#setColumnDefField(columnIndex, "field", field);
        },
        setType: (columnIndex, type) => {
            this.#setColumnDefField(columnIndex, "type", type);
        }
    };
    alterColumns(action) {
        action(this.#columnAlterer);
        this.#renderHeader();
        this.#renderData();
        this.#renderBody();
    }
    static install() {
        return singleton("Juniper::Widgets::DataTableElement", () => registerFactory("data-table", DataTableElement));
    }
}
export function DataTable(...rest) {
    return DataTableElement.install()(...rest);
}
//# sourceMappingURL=DataTable.js.map