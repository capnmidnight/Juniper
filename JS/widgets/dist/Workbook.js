import { readAsXML } from "@juniper-lib/dom";
const WORKSHEET = "Worksheet";
const TABLE = "Table";
const ROW = "Row";
const CELL = "Cell";
const DATA = "Data";
const NUMBER_FORMAT = "NumberFormat";
const STYLE = "Styles > Style";
const ID = "ss:ID";
const STYLE_ID = "ss:StyleID";
const FORMAT = "ss:Format";
const NAME = "ss:Name";
const INDEX = "ss:Index";
export class Workbook {
    static async loadXmlSpreadsheet2003(blobOrFile, name) {
        const root = await readAsXML(blobOrFile);
        if (!name && !(blobOrFile instanceof Blob)) {
            name = blobOrFile.name;
        }
        return Workbook.fromXmlSpreadsheet2003(name, root);
    }
    static fromXmlSpreadsheet2003(name, root) {
        const workbook = new Workbook(name);
        const styles = new Map(Array.from(root.querySelectorAll(STYLE))
            .map(style => {
            const id = style.getAttribute(ID);
            const numberFormat = style.querySelector(NUMBER_FORMAT);
            const format = numberFormat?.getAttribute(FORMAT);
            return [id, format === "@" ? "String" : format];
        })
            .filter(([_, format]) => !!format));
        workbook.setSheets(Array.from(root.querySelectorAll(WORKSHEET))
            .map(e => Worksheet.fromXmlSpreadsheet2003(workbook, e, styles)));
        return workbook;
    }
    #name;
    get Name() { return this.#name; }
    #worksheets = null;
    get worksheets() { return this.#worksheets; }
    setSheets(sheets) { this.#worksheets = sheets; }
    constructor(name) {
        this.#name = name;
    }
    globalReplaceValue(v1, v2) {
        for (const worksheet of this.worksheets) {
            worksheet.table.globalReplaceValue(v1, v2);
        }
    }
}
export class Worksheet {
    static fromXmlSpreadsheet2003(book, element, styles) {
        const worksheet = new Worksheet(book, element.getAttribute(NAME));
        worksheet.setTable(WorksheetTable.fromXmlSpreadsheet2003(worksheet, element.querySelector(TABLE), styles));
        return worksheet;
    }
    #book;
    get book() { return this.#book; }
    #table;
    get table() { return this.#table; }
    setTable(table) { this.#table = table; }
    constructor(book, name) {
        this.name = name;
        this.#book = book;
    }
}
export class WorksheetTable {
    static fromXmlSpreadsheet2003(worksheet, element, styles) {
        if (!element) {
            throw new Error("Expected a Table element");
        }
        const rows = Array.from(element.querySelectorAll(ROW));
        const headerRow = rows.shift();
        const headerCells = Array.from(headerRow.querySelectorAll(CELL));
        const headers = headerCells.map(cell => cell.querySelector(DATA)?.innerHTML);
        const table = new WorksheetTable(worksheet, headers);
        table.setRows(rows.map(row => {
            const dict = new Map();
            let index = 0;
            for (const cell of row.querySelectorAll(CELL)) {
                const indexStr = cell.getAttribute(INDEX);
                if (indexStr) {
                    const skipIndex = parseFloat(indexStr);
                    if (!Number.isNaN(skipIndex)) {
                        index = skipIndex - 1;
                    }
                }
                if (0 <= index && index < headers.length) {
                    const styleID = cell.getAttribute(STYLE_ID);
                    const style = styles.get(styleID);
                    const value = cell.querySelector(DATA)?.innerHTML;
                    const header = headers[index];
                    dict.set(header, { style, value });
                    ++index;
                }
            }
            return WorksheetRow.fromDict(table, dict);
        }));
        return table;
    }
    #sheet;
    get sheet() { return this.#sheet; }
    #headers;
    get headers() { return this.#headers; }
    #headerSet;
    #rows;
    get rows() { return this.#rows; }
    setRows(rows) { this.#rows = rows; }
    constructor(sheet, headers) {
        this.#sheet = sheet;
        this.#headers = headers;
        this.#headerSet = new Set(headers);
    }
    get isUnderused() { return this.rows.filter(r => r.isUnderused).length > 0; }
    get underusedRows() { return this.rows.filter(r => r.isUnderused); }
    has(...headers) {
        return headers.every(header => this.#headerSet.has(header));
    }
    get(header) {
        return this.#rows.map(row => row.get(header));
    }
    matches(...headers) {
        return headers.length === this.#headers.length
            && this.has(...headers);
    }
    globalReplaceValue(v1, v2) {
        for (const row of this.rows) {
            row.globalReplaceValue(v1, v2);
        }
    }
}
export class WorksheetRow {
    static fromDict(table, data) {
        const row = new WorksheetRow(table);
        const map = new Map();
        for (const [key, { style, value }] of data) {
            map.set(key, new WorksheetCell(row, key, value, style));
        }
        row.#setCells(map);
        return row;
    }
    #table;
    get table() { return this.#table; }
    #cells;
    get cells() { return this.#cells; }
    #setCells(cells) { this.#cells = cells; }
    constructor(table) {
        this.#table = table;
    }
    has(...headers) {
        return headers.every(header => this.cells.has(header));
    }
    peek(header) {
        return this.cells.get(header)?.peek;
    }
    globalReplaceValue(v1, v2) {
        for (const cell of this.cells.values()) {
            cell.replaceValue(v1, v2);
        }
    }
    get(header) {
        if (!this.has(header)) {
            return null;
        }
        return this.cells.get(header)?.value;
    }
    get isUnderused() {
        return Array.from(this.cells.values()).filter(v => !v.accessed).length > 0;
    }
    get unusedFields() {
        return Array.from(this.cells)
            .filter(kv => !kv[1].accessed)
            .map(kv => kv[0]);
    }
    toObject() {
        return Array
            .from(this.#cells.values())
            .reduce((obj, cell) => Object.assign({}, obj, cell.toObject()), {});
    }
}
export class WorksheetCell {
    #row;
    get row() { return this.#row; }
    #key;
    get key() { return this.#key; }
    #style;
    get style() { return this.#style; }
    #value;
    get value() {
        this.#accessed = true;
        return this.peek;
    }
    /**
     * Get the value without modifying the accessed flag;
     */
    get peek() { return this.#value; }
    #accessed = false;
    /**
     * A flag indicating that this cell's value has been accessed. Useful
     * for importers that do advanced validation that want to make sure they
     * use all of the cell values in a work sheet.
     */
    get accessed() {
        return this.#accessed;
    }
    constructor(row, key, value, style) {
        this.#row = row;
        this.#key = key;
        this.#style = style ?? "String";
        this.#value = value;
    }
    replaceValue(v1, v2) {
        if (this.#value.trim().toLocaleLowerCase() == v1.toLocaleLowerCase()) {
            this.#value = v2;
        }
    }
    toObject() {
        return { [this.key]: this.value };
    }
}
//# sourceMappingURL=Workbook.js.map