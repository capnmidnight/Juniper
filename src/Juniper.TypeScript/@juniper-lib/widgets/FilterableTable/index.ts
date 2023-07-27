import { arrayClear, arrayReplace, arraySortedInsert } from "@juniper-lib/collections/arrays";
import { ClassList, ColSpan, CustomData, QueryAll } from "@juniper-lib/dom/attrs";
import { padding, px } from "@juniper-lib/dom/css";
import { getColumnIndex } from "@juniper-lib/dom/getColumnIndex";
import { ButtonReset, ButtonSecondaryOutlineSmall, ElementChild, ErsatzElement, Label, Span, TBody, TD, TFoot, TH, THead, TR, Table, buttonSetEnabled, elementApply, elementClearChildren, elementGetText, elementSetClass, elementSetText } from "@juniper-lib/dom/tags";
import { debounce } from "@juniper-lib/events/debounce";
import { identity } from "@juniper-lib/tslib/identity";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { isDate, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";

import "./style.css";

export type HTMLValuedElement = HTMLInputElement | HTMLSelectElement;

function makeDate(value: string): Date {
    if (/^\d{4}-\d\d?-\d\d?$/.test(value)) {
        const parts = value.split("-");
        const year = parts.shift();
        parts.push(year);
        value = parts.join("/");
    }
    return new Date(value);
}

function isRangeStart(filterElement: HTMLValuedElement) {
    return filterElement.classList.contains("range-start");
}

function isRangeEnd(filterElement: HTMLValuedElement): boolean {
    return filterElement.classList.contains("range-end");
}

function parseValue(input: HTMLValuedElement, value: string): string | number | Date {
    if (input instanceof HTMLInputElement && (input.type.startsWith("date") || input.type.startsWith("time") || input.type.startsWith("month") || input.type.startsWith("week"))) {
        return value && makeDate(value) || null;
    }
    else if (input instanceof HTMLInputElement && (input.type === "number" || input.type === "range")) {
        return value && parseFloat(value) || null;
    }
    else {
        return value && value.toLocaleLowerCase() || null;
    }
}

function getCellValue(cell: HTMLTableCellElement, filterElement: HTMLValuedElement) {
    const inputs = Array.from(cell.querySelectorAll<HTMLInputElement>("input"));
    if (inputs.length === 0) {
        const text = cell.textContent.trim();
        return [parseValue(filterElement, text)];
    }
    else {
        return inputs.map(input => parseValue(input, input.value));
    }
}

export interface FilterableTableColumn<T> {
    header?: ElementChild;
    headerColSpan?: number;
    filter?: HTMLValuedElement;
    filterColSpan?: number;
    getCellValue?: (value: T, row?: HTMLTableRowElement) => ElementChild;
}

export interface FilterableTableOptions<T> {
    resourceName: string;
    pageSizes?: number[];
    columns: FilterableTableColumn<T>[];
}

const DEFAULT_PAGE_SIZES = [
    10,
    25,
    50,
    100
];

export class FilterableTable<T extends any> implements ErsatzElement<HTMLTableElement> {

    static create<T>(options: FilterableTableOptions<T>) {
        const headerRow = TR();
        const filterRow = TR();
        let burnHeader = 0;
        let burnFilter = 0;
        for (const column of options.columns) {
            let filterId = null;
            if (burnHeader > 0) {
                --burnHeader;
            }
            else {
                const header = Label(column.header);
                if (column.filter) {
                    filterId = stringRandom(10);
                    header.htmlFor = filterId;
                }
                const headerCell = TH(header);
                if (isDefined(column.headerColSpan)) {
                    headerCell.colSpan = column.headerColSpan;
                    burnHeader = column.headerColSpan - 1;
                }
                headerRow.appendChild(headerCell);
            }

            if (burnFilter > 0) {
                --burnFilter;
            }
            else if (column.filter) {
                const filter = column.filter;
                if (filterId) {
                    filter.id = filterId;
                }
                if (column.header && filter instanceof HTMLInputElement) {
                    filter.placeholder = "Filter by " + column.header;
                }
                const filterCell = TH(filter);
                if (isDefined(column.filterColSpan)) {
                    filterCell.colSpan = column.filterColSpan;
                    burnFilter = column.filterColSpan - 1;
                }
                filterRow.appendChild(filterCell);
            }
        }

        let lastColumn = getColumnIndex(headerRow.lastElementChild) - 1;
        while (getColumnIndex(filterRow.lastElementChild) < lastColumn) {
            filterRow.appendChild(TH());
        }

        filterRow.appendChild(TH(ButtonReset(ClassList("btn", "btn-secondary"), "Reset")));

        lastColumn = getColumnIndex(filterRow.lastElementChild);
        while (lastColumn > getColumnIndex(headerRow.lastElementChild)) {
            headerRow.appendChild(TH());
        }

        const table = new FilterableTable<T>(Table(
            ClassList("table", "table-responsive", "table-hover", "table-striped", "summary"),
            CustomData("resourcename", options.resourceName),
            THead(
                headerRow,
                filterRow
            ),
            TBody()
        ));

        options.columns.forEach((c, i) => table.setCellMapper(i, c.getCellValue));
        table.setPageSizes(options.pageSizes || DEFAULT_PAGE_SIZES);

        return table;
    }

    private readonly update: (updatedElement?: HTMLValuedElement) => void;

    private readonly resourceName: string;
    private readonly rows: Array<HTMLTableRowElement>;
    private readonly ranges = new Map<number, [HTMLValuedElement, HTMLValuedElement]>();
    private readonly valueCache = new Map<HTMLValuedElement, string | number | Date>();
    private readonly filterElements: HTMLValuedElement[];
    private readonly colCount: number;
    private readonly noContentMessageElement: HTMLTableRowElement;
    private readonly paginator: HTMLTableCellElement;
    private readonly columnIndices: Map<HTMLValuedElement, number>;
    private readonly resetButton: HTMLButtonElement = null;
    private readonly cellMappers = new Map<number, (value: T, row?: HTMLTableRowElement) => ElementChild>();
    private readonly pageSizes = [0, ...DEFAULT_PAGE_SIZES];
    private readonly pageIndexKey: string;
    private readonly pageSizeKey: string;

    private _pageIndex: number;
    get pageIndex() {
        return this._pageIndex;
    }

    set pageIndex(v) {
        this._pageIndex = v;
        localStorage.setItem(this.pageIndexKey, v.toString());
    }

    private _pageSize: number;
    get pageSize() {
        return this._pageSize;
    }

    set pageSize(v) {
        this._pageSize = v;
        localStorage.setItem(this.pageSizeKey, v.toString());
    }

    constructor(public readonly element: HTMLTableElement) {
        this.update = debounce(this._update.bind(this));

        this.resourceName = this.element.dataset.resourcename;
        this.pageIndexKey = `${this.resourceName}-page-index`;
        this.pageSizeKey = `${this.resourceName}-page-size`;

        this.rows = QueryAll(this.element, "tbody > tr");

        this.ranges = new Map<number, [HTMLValuedElement, HTMLValuedElement]>();

        if (this.element.tHead) {
            this.filterElements = QueryAll(this.element.tHead, "input,select");
            this.resetButton = this.element.tHead.querySelector<HTMLButtonElement>("button[type=reset]");
        }
        else {
            this.filterElements = [];
        }

        this.colCount = Math.max(
            ...QueryAll(this.element, "tr")
                .map(r => r.children.length)
        );

        this.noContentMessageElement = TR(TD(ColSpan(this.colCount), "No content"));
        this.paginator = TD(ColSpan(this.colCount), ClassList("multi"));
        this.columnIndices = new Map<HTMLValuedElement, number>();

        if (this.resetButton) {
            this.resetButton.addEventListener("click", () => {
                this.valueCache.clear();
                for (const element of this.filterElements) {
                    this.deleteValue(element);
                    element.value = "";
                }
                this.update();
            });
        }

        if (!this.element.tFoot) {
            this.element.tFoot = TFoot();
        }

        elementApply(this.element.tFoot,
            this.noContentMessageElement,
            ...this.element.tFoot.children,
            TR(this.paginator));

        for (const f of this.filterElements) {
            const idx = getColumnIndex(f);
            this.columnIndices.set(f, idx);
            f.addEventListener("input", this.update.bind(null, f));
            this.restoreValue(f);
            if (isRangeStart(f)
                || isRangeEnd(f)) {
                if (!this.ranges.has(idx)) {
                    this.ranges.set(idx, [null, null]);
                }

                const range = this.ranges.get(idx);
                const part = isRangeStart(f) ? 0 : 1;
                range[part] = f;
            }
        }

        for (const f of this.element.querySelectorAll("input")) {
            f.classList.add("form-control", "form-control-sm");
        }

        for (const f of this.element.querySelectorAll("select")) {
            f.classList.add("custom-select", "custom-select-sm");
        }

        this.pageIndex = parseFloat(localStorage.getItem(this.pageIndexKey) || "0");
        this.pageSize = parseFloat(localStorage.getItem(this.pageSizeKey) || "10");

        if (isDefined(this.element.dataset.pagesizes)
            && this.element.dataset.pagesizes.length > 0) {
            const sizes = this.element.dataset.pagesizes
                .split(",")
                .map(v => parseFloat(v.trim()))
                .filter(identity);
            this.setPageSizes(sizes);
        }
        else {
            this.update();
        }
    }

    setPageSizes(pageSizes: number[]): void {
        if (isNullOrUndefined(pageSizes) || pageSizes.length === 0) {
            throw new Error("Need at least one page size");
        }
        arrayReplace(this.pageSizes, 0, ...pageSizes);
        this.pageSize = this.pageSizes[1];
        this.update();
    }

    setCellMapper(columnIndex: number, mapper: (value: T, row?: HTMLTableRowElement) => ElementChild): void {
        this.cellMappers.set(columnIndex, mapper);
    }

    clear(): void {
        arrayClear(this.rows);
        for (const body of this.element.tBodies) {
            elementClearChildren(body);
        }
        this._update();
    }

    setValues(...values: T[]): void {
        const newRows = values.map(value => {
            const row = TR();
            for (let i = 0; i < this.colCount; ++i) {
                const cell = TD();
                if (this.cellMappers.has(i)) {
                    elementApply(cell, this.cellMappers.get(i)(value, row));
                }
                row.appendChild(cell);
            }
            this.rows.push(row);
            return row;
        });

        arrayReplace(this.rows, ...newRows);

        for (const body of this.element.tBodies) {
            elementClearChildren(body);
        }

        elementApply(this.contentRoot,
            ...this.rows
        );

        this._update();
    }

    select(sel: HTMLTableRowElement) {
        let selectedIndex: number = null;
        for (let i = 0; i < this.rows.length; ++i) {
            const row = this.rows[i];
            const selected = row === sel;
            elementSetClass(row, selected, "selected");
            if (selected) {
                selectedIndex = i;
            }
        }

        if (selectedIndex !== null) {
            this.pageIndex = Math.floor(selectedIndex / this.pageSize) * this.pageSize;
            this.update();
        }
    }

    get noContentMessage(): string {
        return elementGetText(this.noContentMessageElement);
    }

    set noContentMessage(v: string) {
        elementSetText(this.noContentMessageElement, v);
    }

    private get contentRoot() { return this.element.tBodies[0]; }

    private makeKey(filterElement: HTMLValuedElement): string {
        return `yarrow:${this.resourceName}:${filterElement.id}`;
    }

    private saveValue(filterElement: HTMLValuedElement, value: string | number | Date): void {
        const key = this.makeKey(filterElement);
        if (value) {
            localStorage.setItem(key, value.toString());
        }
        else {
            localStorage.removeItem(key);
        }
    }

    private deleteValue(filterElement: HTMLValuedElement): void {
        const key = this.makeKey(filterElement);
        localStorage.removeItem(key);
    }

    private restoreValue(input: HTMLValuedElement): void {
        const key = this.makeKey(input);
        const value = localStorage.getItem(key);

        if (!isDefined(value)) {
            input.value = "";
        }
        else if (input instanceof HTMLSelectElement) {
            input.value = value;
            this.valueCache.set(input, input.value);
        }
        else if (input.type.startsWith("date")) {
            const date = makeDate(value);
            input.valueAsDate = date;
            this.valueCache.set(input, date);
        }
        else if (input.type === "number"
            || input.type === "range") {
            const number = parseFloat(value);
            input.valueAsNumber = number;
            this.valueCache.set(input, number);
        }
        else {
            input.value = value;
            this.valueCache.set(input, input.value);
        }
    }

    private isRange(filterElement: HTMLValuedElement) {
        const idx = getColumnIndex(filterElement);
        return this.ranges.has(idx);
    }

    private _update(updatedElement: HTMLValuedElement = null) {
        if (isDefined(updatedElement)) {
            const value = parseValue(updatedElement, updatedElement.value);
            this.saveValue(updatedElement, value);
            if (value) {
                this.valueCache.set(updatedElement, value);
            }
            else {
                this.valueCache.delete(updatedElement);
            }
        }

        if (this.resetButton) {
            buttonSetEnabled(this.resetButton, this.valueCache.size > 0, "Reset", "Clear out filters");
            elementSetClass(this.resetButton, this.valueCache.size > 0, "btn-secondary");
            elementSetClass(this.resetButton, this.valueCache.size === 0, "btn-outline-secondary");
        }

        let showCount = 0;
        for (let r = 0; r < this.rows.length; ++r) {
            const row = this.rows[r] as HTMLTableRowElement;
            let showCell = true;

            for (let f = 0; f < this.filterElements.length && showCell; ++f) {
                const element = this.filterElements[f];
                const columnIndex = this.columnIndices.get(element);
                if (this.isRange(element)) {
                    const [minFilterElement, maxFilterElement] = this.ranges.get(columnIndex);
                    const minFilterValue = this.valueCache.get(minFilterElement) as Date;
                    const maxFilterValue = this.valueCache.get(maxFilterElement) as Date;
                    const cell = row.children[columnIndex] as HTMLTableCellElement;
                    const cellValues = getCellValue(cell, minFilterElement);

                    let matches: boolean[] = null;
                    if (isDefined(minFilterValue)
                        && isDefined(maxFilterValue)) {
                        matches = cellValues.map(cellValue =>
                            minFilterValue <= cellValue && cellValue <= maxFilterValue);

                    }
                    else if (isDefined(minFilterValue)) {
                        matches = cellValues.map(cellValue =>
                            minFilterValue <= cellValue);
                    }
                    else if (isDefined(maxFilterValue)) {
                        matches = cellValues.map(cellValue =>
                            cellValue <= maxFilterValue);
                    }

                    if (matches) {
                        const match = matches.reduce((a, b) => a || b, false);
                        showCell &&= match;
                    }
                }
                else {
                    const filterValue = this.valueCache.get(element);
                    if (filterValue) {
                        const cell = row.children[columnIndex] as HTMLTableCellElement;
                        const cellValues = getCellValue(cell, element);
                        const matches = cellValues.map(cellValue =>
                            isNullOrUndefined(cellValue)
                                ? true
                                : isDate(cellValue)
                                    ? (filterValue as Date).getTime() === cellValue.getTime()
                                    : isNumber(cellValue)
                                        ? cellValue === filterValue
                                        : filterValue === "XXX_NONE_XXX"
                                            ? cellValue.length === 0 || cellValue === "NONE"
                                            : element.classList.contains("exact")
                                                ? cellValue.toLocaleLowerCase() === filterValue
                                                : cellValue.toLocaleLowerCase().indexOf(filterValue as string) > -1);
                        const match = matches.reduce((a, b) => a || b, false);
                        showCell &&= match;
                    }
                }
            }

            if (showCell) {
                ++showCount;
            }

            row.style.display = showCell
                ? ""
                : "none";
        }

        const numPages = Math.ceil(showCount / this.pageSize);
        const lastPage = Math.max(0, numPages - 1);
        const curPage = Math.min(lastPage, Math.floor(this.pageIndex / this.pageSize));
        this.pageIndex = curPage * this.pageSize;

        const nextPage = Math.min(lastPage, curPage + 1);
        const prevPage = Math.max(0, curPage - 1);
        const minIndex = curPage * this.pageSize;
        const maxIndex = minIndex + this.pageSize;

        const filteredRows = this.rows.filter(r => r.style.display !== "none");
        for (let i = 0; i < filteredRows.length; ++i) {
            if (i < minIndex || maxIndex <= i) {
                filteredRows[i].style.display = "none";
            }
        }

        this.noContentMessageElement.style.display = showCount === 0
            ? ""
            : "none";

        elementClearChildren(this.paginator);

        const pageNumbers: number[] = [];
        const addPage = (page: number) =>
            arraySortedInsert(pageNumbers, page, false);

        addPage(0);
        addPage(curPage);
        addPage(prevPage);
        addPage(nextPage);
        addPage(lastPage);

        const lastIndex = lastPage * this.pageSize;
        const prevPageIndex = Math.max(0, this.pageIndex - this.pageSize);

        const makeChunk = (text: string, enabled: boolean) => {
            let chunk: HTMLElement;
            if (enabled) {
                chunk = ButtonSecondaryOutlineSmall(text);
            }
            else {
                chunk = Span(padding(px(5), px(10)), text);
            }
            this.paginator.append(chunk);
            return chunk;
        };

        const makePageIndexLink = (text: string, index: number) => {
            const enabled = 0 <= index
                && index <= lastIndex
                && index !== this.pageIndex;
            const link = makeChunk(text, enabled);
            if (enabled) {
                link.addEventListener("click", () => {
                    this.pageIndex = index;
                    this.update();
                });
            }
        };

        const makePageSizeLink = (size: number) => {
            const enabled = size !== this.pageSize;
            const link = makeChunk(size.toString(), enabled);
            if (enabled) {
                link.addEventListener("click", () => {
                    this.pageSize = size;
                    this.update();
                });
            }
        };

        makePageIndexLink("<", prevPageIndex);

        let last = -1;
        for (const pageNumber of pageNumbers) {
            const delta = pageNumber - last;
            last = pageNumber;
            if (delta > 1) {
                makeChunk("...", false);
            }
            makePageIndexLink((pageNumber + 1).toFixed(0), pageNumber * this.pageSize);
        }

        makePageIndexLink(">", maxIndex);

        makeChunk("|", false);
        makeChunk(`${showCount} filtered items of ${this.rows.length} results`, false);

        if (this.pageSizes.length > 2) {
            makeChunk("|", false);
            makeChunk("Items per page:", false);
            for (let i = 1; i < this.pageSizes.length; ++i) {
                if (this.rows.length > this.pageSizes[i - 1]) {
                    makePageSizeLink(this.pageSizes[i]);
                }
            }
        }
    }
}
