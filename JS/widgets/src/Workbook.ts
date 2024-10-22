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

    static async loadXmlSpreadsheet2003(blob: Blob, name?: string): Promise<Workbook>;
    static async loadXmlSpreadsheet2003(file: File | FileSystemFileHandle): Promise<Workbook>;
    static async loadXmlSpreadsheet2003(blobOrFile: File | FileSystemFileHandle | Blob, name?: string): Promise<Workbook> {
        const root = await readAsXML(blobOrFile);

        if (!name && !(blobOrFile instanceof Blob)) {
            name = blobOrFile.name;
        }

        return Workbook.fromXmlSpreadsheet2003(name, root);
    }

    static fromXmlSpreadsheet2003(name: string, root: Document): Workbook {
        const workbook = new Workbook(name);

        const styles = new Map(Array.from(root.querySelectorAll(STYLE))
            .map<[string, string]>(style => {
                const id = style.getAttribute(ID);
                const numberFormat = style.querySelector(NUMBER_FORMAT);
                const format = numberFormat?.getAttribute(FORMAT);
                return [id, format === "@" ? "String" : format];
            })
            .filter(([_, format]) => !!format));
        workbook.setSheets(Array.from(root.querySelectorAll(WORKSHEET))
            .map(e => Worksheet.fromXmlSpreadsheet2003(workbook, e, styles))
        );
        return workbook;
    }

    readonly #name: string
    public get Name() { return this.#name; }

    #worksheets: Worksheet[] = null;
    get worksheets() { return this.#worksheets; }
    setSheets(sheets: Worksheet[]) { this.#worksheets = sheets; }

    constructor(name: string) {
        this.#name = name;
    }

    globalReplaceValue(v1: string, v2: string) {
        for (const worksheet of this.worksheets) {
            worksheet.table.globalReplaceValue(v1, v2);
        }
    }
}


export class Worksheet {
    static fromXmlSpreadsheet2003(book: Workbook, element: Element, styles: Map<string, string>) {
        const worksheet = new Worksheet(book, element.getAttribute(NAME));
        worksheet.setTable(WorksheetTable.fromXmlSpreadsheet2003(worksheet, element.querySelector(TABLE), styles));
        return worksheet;
    }

    readonly #book: Workbook;
    get book() { return this.#book; }

    #table: WorksheetTable;
    get table() { return this.#table; }
    setTable(table: WorksheetTable) { this.#table = table; }

    constructor(book: Workbook, public readonly name: string) {
        this.#book = book;
    }
}

export class WorksheetTable {
    static fromXmlSpreadsheet2003(worksheet: Worksheet, element: Element, styles: Map<string, string>) {
        if (!element) {
            throw new Error("Expected a Table element");
        }

        const rows = Array.from(element.querySelectorAll(ROW));
        const headerRow = rows.shift();
        const headerCells = Array.from(headerRow.querySelectorAll(CELL));
        const headers = headerCells.map(cell => cell.querySelector(DATA)?.innerHTML);
        const table = new WorksheetTable(
            worksheet,
            headers
        );

        table.setRows(
            rows.map(row => {
                const dict = new Map<string, { style: string, value: string }>();
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
            })
        );

        return table;
    }

    readonly #sheet: Worksheet;
    get sheet() { return this.#sheet; }

    readonly #headers: string[];
    get headers() { return this.#headers; }

    readonly #headerSet: Set<string>;

    #rows: WorksheetRow[];
    get rows() { return this.#rows; }
    setRows(rows: WorksheetRow[]) { this.#rows = rows; }

    constructor(sheet: Worksheet, headers: string[]) {
        this.#sheet = sheet;
        this.#headers = headers;
        this.#headerSet = new Set(headers);
    }

    get isUnderused() { return this.rows.filter(r => r.isUnderused).length > 0; }
    get underusedRows() { return this.rows.filter(r => r.isUnderused); }

    has(...headers: string[]) {
        return headers.every(header => this.#headerSet.has(header));
    }

    get(header: string) {
        return this.#rows.map(row => row.get(header));
    }

    matches(...headers: string[]) {
        return headers.length === this.#headers.length
            && this.has(...headers);
    }

    globalReplaceValue(v1: string, v2: string) {
        for (const row of this.rows) {
            row.globalReplaceValue(v1, v2);
        }
    }
}

export class WorksheetRow {
    public static fromDict(table: WorksheetTable, data: Map<string, { style: string, value: string }>) {
        const row = new WorksheetRow(table);
        const map = new Map<string, WorksheetCell>();
        for (const [key, { style, value }] of data) {
            map.set(key, new WorksheetCell(row, key, value, style));
        }
        row.#setCells(map);
        return row;
    }

    readonly #table: WorksheetTable;
    get table() { return this.#table; }

    #cells: Map<string, WorksheetCell>;
    get cells() { return this.#cells; }
    #setCells(cells: Map<string, WorksheetCell>) { this.#cells = cells; }

    constructor(table: WorksheetTable) {
        this.#table = table;
    }

    has(...headers: string[]) {
        return headers.every(header => this.cells.has(header));
    }

    peek(header: string) {
        return this.cells.get(header)?.peek;
    }

    globalReplaceValue(v1: string, v2: string) {
        for (const cell of this.cells.values()) {
            cell.replaceValue(v1, v2);
        }
    }

    get(header: string) {
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
            .reduce((obj, cell) =>
                Object.assign({}, obj, cell.toObject()),
                {}
            );
    }
}

export class WorksheetCell {
    readonly #row: WorksheetRow;
    get row() { return this.#row; }

    readonly #key: string;
    get key() { return this.#key; }

    readonly #style: string;
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

    constructor(row: WorksheetRow, key: string, value: string, style?: string) {
        this.#row = row;
        this.#key = key;
        this.#style = style ?? "String";
        this.#value = value;
    }

    replaceValue(v1: string, v2: string) {
        if (this.#value.trim().toLocaleLowerCase() == v1.toLocaleLowerCase()) {
            this.#value = v2;
        }
    }

    toObject() {
        return { [this.key]: this.value };
    }
}