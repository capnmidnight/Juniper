import { arrayClear, arrayReplace, insertSorted, binarySearch, compareBy } from "@juniper-lib/collections/arrays";
import { ClassList, ColSpan, CustomData, QueryAll } from "@juniper-lib/dom/attrs";
import { padding, px } from "@juniper-lib/dom/css";
import { getColumnIndex } from "@juniper-lib/dom/getColumnIndex";
import { ButtonReset, ButtonSecondaryOutlineSmall, Label, Span, TBody, TD, TFoot, TH, THead, TR, Table, buttonSetEnabled, HtmlRender, elementClearChildren, elementGetText, elementSetClass, elementSetText } from "@juniper-lib/dom/tags";
import { debounce } from "@juniper-lib/events/debounce";
import { identity } from "@juniper-lib/tslib/identity";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { isDate, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import "./style.css";
function makeDate(value) {
    if (/^\d{4}-\d\d?-\d\d?$/.test(value)) {
        const parts = value.split("-");
        const year = parts.shift();
        parts.push(year);
        value = parts.join("/");
    }
    return new Date(value);
}
function isRangeStart(filterElement) {
    return filterElement.classList.contains("range-start");
}
function isRangeEnd(filterElement) {
    return filterElement.classList.contains("range-end");
}
function parseValue(input, value) {
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
function getCellValue(cell, filterElement) {
    const inputs = Array.from(cell.querySelectorAll("input"));
    if (inputs.length === 0) {
        const text = cell.textContent.trim();
        return [parseValue(filterElement, text)];
    }
    else {
        return inputs.map(input => parseValue(input, input.value));
    }
}
const DEFAULT_PAGE_SIZES = [
    10,
    25,
    50,
    100
];
const comparer = compareBy(identity);
export class FilterableTable {
    static create(options) {
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
        const table = new FilterableTable(Table(ClassList("table", "table-responsive", "table-hover", "table-striped", "summary"), CustomData("resourcename", options.resourceName), THead(headerRow, filterRow), TBody()));
        options.columns.forEach((c, i) => table.setCellMapper(i, c.getCellValue));
        table.setPageSizes(options.pageSizes || DEFAULT_PAGE_SIZES);
        return table;
    }
    get pageIndex() {
        return this._pageIndex;
    }
    set pageIndex(v) {
        this._pageIndex = v;
        localStorage.setItem(this.pageIndexKey, v.toString());
    }
    get pageSize() {
        return this._pageSize;
    }
    set pageSize(v) {
        this._pageSize = v;
        localStorage.setItem(this.pageSizeKey, v.toString());
    }
    constructor(element) {
        this.element = element;
        this.ranges = new Map();
        this.valueCache = new Map();
        this.resetButton = null;
        this.cellMappers = new Map();
        this.pageSizes = [0, ...DEFAULT_PAGE_SIZES];
        this.update = debounce(this._update.bind(this));
        this.resourceName = this.element.dataset.resourcename;
        this.pageIndexKey = `${this.resourceName}-page-index`;
        this.pageSizeKey = `${this.resourceName}-page-size`;
        this.rows = QueryAll(this.element, "tbody > tr");
        this.ranges = new Map();
        if (this.element.tHead) {
            this.filterElements = QueryAll(this.element.tHead, "input,select");
            this.resetButton = this.element.tHead.querySelector("button[type=reset]");
        }
        else {
            this.filterElements = [];
        }
        this.colCount = Math.max(...QueryAll(this.element, "tr")
            .map(r => r.children.length));
        this.noContentMessageElement = TR(TD(ColSpan(this.colCount), "No content"));
        this.paginator = TD(ColSpan(this.colCount), ClassList("multi"));
        this.columnIndices = new Map();
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
        HtmlRender(this.element.tFoot, this.noContentMessageElement, ...this.element.tFoot.children, TR(this.paginator));
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
    setPageSizes(pageSizes) {
        if (isNullOrUndefined(pageSizes) || pageSizes.length === 0) {
            throw new Error("Need at least one page size");
        }
        arrayReplace(this.pageSizes, 0, ...pageSizes);
        this.pageSize = this.pageSizes[1];
        this.update();
    }
    setCellMapper(columnIndex, mapper) {
        this.cellMappers.set(columnIndex, mapper);
    }
    clear() {
        arrayClear(this.rows);
        for (const body of this.element.tBodies) {
            elementClearChildren(body);
        }
        this._update();
    }
    setValues(...values) {
        const newRows = values.map(value => {
            const row = TR();
            for (let i = 0; i < this.colCount; ++i) {
                const cell = TD();
                if (this.cellMappers.has(i)) {
                    HtmlRender(cell, this.cellMappers.get(i)(value, row));
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
        HtmlRender(this.contentRoot, ...this.rows);
        this._update();
    }
    select(sel) {
        let selectedIndex = null;
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
    get noContentMessage() {
        return elementGetText(this.noContentMessageElement);
    }
    set noContentMessage(v) {
        elementSetText(this.noContentMessageElement, v);
    }
    get contentRoot() { return this.element.tBodies[0]; }
    makeKey(filterElement) {
        return `yarrow:${this.resourceName}:${filterElement.id}`;
    }
    saveValue(filterElement, value) {
        const key = this.makeKey(filterElement);
        if (value) {
            localStorage.setItem(key, value.toString());
        }
        else {
            localStorage.removeItem(key);
        }
    }
    deleteValue(filterElement) {
        const key = this.makeKey(filterElement);
        localStorage.removeItem(key);
    }
    restoreValue(input) {
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
    isRange(filterElement) {
        const idx = getColumnIndex(filterElement);
        return this.ranges.has(idx);
    }
    _update(updatedElement = null) {
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
            const row = this.rows[r];
            let showCell = true;
            for (let f = 0; f < this.filterElements.length && showCell; ++f) {
                const element = this.filterElements[f];
                const columnIndex = this.columnIndices.get(element);
                if (this.isRange(element)) {
                    const [minFilterElement, maxFilterElement] = this.ranges.get(columnIndex);
                    const minFilterValue = this.valueCache.get(minFilterElement);
                    const maxFilterValue = this.valueCache.get(maxFilterElement);
                    const cell = row.children[columnIndex];
                    const cellValues = getCellValue(cell, minFilterElement);
                    let matches = null;
                    if (isDefined(minFilterValue)
                        && isDefined(maxFilterValue)) {
                        matches = cellValues.map(cellValue => minFilterValue <= cellValue && cellValue <= maxFilterValue);
                    }
                    else if (isDefined(minFilterValue)) {
                        matches = cellValues.map(cellValue => minFilterValue <= cellValue);
                    }
                    else if (isDefined(maxFilterValue)) {
                        matches = cellValues.map(cellValue => cellValue <= maxFilterValue);
                    }
                    if (matches) {
                        const match = matches.reduce((a, b) => a || b, false);
                        showCell &&= match;
                    }
                }
                else {
                    const filterValue = this.valueCache.get(element);
                    if (filterValue) {
                        const cell = row.children[columnIndex];
                        const cellValues = getCellValue(cell, element);
                        const matches = cellValues.map(cellValue => isNullOrUndefined(cellValue)
                            ? true
                            : isDate(cellValue)
                                ? filterValue.getTime() === cellValue.getTime()
                                : isNumber(cellValue)
                                    ? cellValue === filterValue
                                    : filterValue === "XXX_NONE_XXX"
                                        ? cellValue.length === 0 || cellValue === "NONE"
                                        : element.classList.contains("exact")
                                            ? cellValue.toLocaleLowerCase() === filterValue
                                            : cellValue.toLocaleLowerCase().indexOf(filterValue) > -1);
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
        const pageNumbers = [];
        const addPage = (page) => {
            const idx = binarySearch(pageNumbers, page, comparer);
            if (idx < 0) {
                insertSorted(pageNumbers, page, idx);
            }
        };
        addPage(0);
        addPage(curPage);
        addPage(prevPage);
        addPage(nextPage);
        addPage(lastPage);
        const lastIndex = lastPage * this.pageSize;
        const prevPageIndex = Math.max(0, this.pageIndex - this.pageSize);
        const makeChunk = (text, enabled) => {
            let chunk;
            if (enabled) {
                chunk = ButtonSecondaryOutlineSmall(text);
            }
            else {
                chunk = Span(padding(px(5), px(10)), text);
            }
            this.paginator.append(chunk);
            return chunk;
        };
        const makePageIndexLink = (text, index) => {
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
        const makePageSizeLink = (size) => {
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
//# sourceMappingURL=index.js.map