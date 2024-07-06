import { Exception, isNullOrUndefined, isString, mapMap } from "@juniper-lib/util";
import { getFileSystemFileHandles, getXMLFiles, readAsXML } from "@juniper-lib/dom";
import { Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet } from "@juniper-lib/mediatypes";
import * as XLSX from "xlsx";
import { importXLSX } from "./excel";


const NAMESPACE = "urn:schemas-microsoft-com:office:spreadsheet";
const WORKSHEET = "Worksheet";
const TABLE = "Table";
const ROW = "Row";
const CELL = "Cell";
const DATA = "Data";

const NAME = "Name";
const INDEX = "Index";


/**
 * Invoke a file open dialog and return the results as an XmlSpreadsheet2003 Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getXMLSpreadsheet2003Files(allowMultiple: boolean) {
    const xmls = await getXMLFiles(allowMultiple)
    return Array.from(xmls).map(([name, doc]) =>
        Workbook.fromXmlSpreadsheet2003(name, doc));
}


/**
 * Invoke a file open dialog and return the results as an XLSX Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getXLSXFiles(allowMultiple: boolean) {
    const handles = await getFileSystemFileHandles(allowMultiple, {
        description: "XLSX Files",
        accept: Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet.toFileSystemAPIAccepts()
    });
    return await Promise.all(handles.map(handle =>
        Workbook.loadXLSX(handle)));
}

export class Workbook {

    static async loadXmlSpreadsheet2003(blob: Blob, name?: string): Promise<Workbook>;
    static async loadXmlSpreadsheet2003(file: File | FileSystemFileHandle): Promise<Workbook>;
    static async loadXmlSpreadsheet2003(blobOrFile: File | FileSystemFileHandle | Blob, name?: string): Promise<Workbook> {
        const root = await readAsXML(blobOrFile);

        if (isNullOrUndefined(name) && !(blobOrFile instanceof Blob)) {
            name = blobOrFile.name;
        }

        return Workbook.fromXmlSpreadsheet2003(name, root);
    }

    static fromXmlSpreadsheet2003(name: string, root: Document): Workbook {
        const workbook = new Workbook(name);
        workbook.#setSheets(Array.from(root.querySelectorAll(WORKSHEET))
            .map(e => Worksheet.fromXmlSpreadsheet2003(workbook, e))
        );
        return workbook;
    }

    static async loadXLSX(blob: Blob, name?: string, opts?: XLSX.ParsingOptions): Promise<Workbook>;
    static async loadXLSX(file: File | FileSystemFileHandle, opts?: XLSX.ParsingOptions): Promise<Workbook>;
    static async loadXLSX(blobOrFile: File | FileSystemFileHandle | Blob, nameOrOpts?: string | XLSX.ParsingOptions, opts?: XLSX.ParsingOptions): Promise<Workbook> {
        let name: string = null;

        if (isString(nameOrOpts)) {
            name = nameOrOpts;
        }
        else {
            opts = nameOrOpts;
        }

        if (blobOrFile instanceof FileSystemFileHandle) {
            blobOrFile = await blobOrFile.getFile();
        }

        if (blobOrFile instanceof File) {
            name = blobOrFile.name;
        }

        const root = await importXLSX(blobOrFile, opts);

        const workbook = new Workbook(name);
        workbook.#setSheets(root.SheetNames.map(e =>
            Worksheet.fromXSLX(workbook, e, root.Sheets[e])
        ));
        return workbook;
    }

    #worksheets: Worksheet[] = null;
    get worksheets() { return this.#worksheets; }
    #setSheets(sheets: Worksheet[]) { this.#worksheets = sheets; }

    constructor(public readonly Name: string) {
    }

    globalReplaceValue(v1: string, v2: string) {
        for (const worksheet of this.worksheets) {
            worksheet.table.globalReplaceValue(v1, v2);
        }
    }
}


export class Worksheet {
    static fromXmlSpreadsheet2003(book: Workbook, element: Element) {
        const worksheet = new Worksheet(book, element.getAttributeNS(NAMESPACE, NAME));
        worksheet.#setTable(WorksheetTable.fromXmlSpreadsheet2003(worksheet, element.querySelector(TABLE)));
        return worksheet;
    }
    
    static fromXSLX(book: Workbook, sheetName: string, sheet: XLSX.WorkSheet): any {
        const worksheet = new Worksheet(book, sheetName);
        worksheet.#setTable(WorksheetTable.fromXSLX(worksheet, sheet))
        return worksheet;
    }

    #table: WorksheetTable;
    get table() { return this.#table; }
    #setTable(table: WorksheetTable) { this.#table = table; }

    constructor(public readonly book: Workbook, public readonly name: string) {
    }
}

export class WorksheetTable {
    static fromXmlSpreadsheet2003(worksheet: Worksheet, element: Element) {
        if (!element) {
            throw new Exception("Expected a Table element");
        }

        const rows = Array.from(element.querySelectorAll(ROW));
        const headerRow = rows.shift();
        const headerCells = Array.from(headerRow.querySelectorAll(CELL));
        const headers = headerCells.map(cell => cell.querySelector(DATA)?.innerHTML);
        const table = new WorksheetTable(
            worksheet,
            new Set(headers)
        );

        table.#setRows(
            rows.map(row => {
                const dict = new Map<string, string>();
                let index = 0;
                for (const cell of row.querySelectorAll(CELL)) {
                    const indexStr = cell.getAttributeNS(NAMESPACE, INDEX);
                    if (indexStr) {
                        const skipIndex = parseFloat(indexStr);
                        if (!Number.isNaN(skipIndex)) {
                            index = skipIndex - 1;
                        }
                    }

                    if (0 <= index && index < headers.length) {
                        const val = cell.querySelector(DATA)?.innerHTML;
                        const header = headers[index];
                        dict.set(header, val);
                        ++index;
                    }
                }

                return WorksheetRow.fromDict(table, dict);
            })
        );

        return table;
    }
    
    static fromXSLX(worksheet: Worksheet, sheet: XLSX.WorkSheet): WorksheetTable {
        if (!sheet) {
            throw new Exception("Expected a Sheet object");
        }

        const rows = XLSX.utils.sheet_to_json<string[]>(sheet, {
            raw: false,
            header: 1
        });
        const headers = rows.shift();
        const table = new WorksheetTable(
            worksheet,
            new Set(headers)
        );

        table.#setRows(
            rows.map(row => {
                const dict = new Map<string, string>();
                for(let index = 0; index < row.length; ++index) {
                    const cell = row[index];

                    if (0 <= index && index < headers.length) {
                        const header = headers[index];
                        dict.set(header, cell);
                    }
                }

                return WorksheetRow.fromDict(table, dict);
            })
        );

        return table;
    }

    #rows: WorksheetRow[];
    get rows() { return this.#rows; }
    #setRows(rows: WorksheetRow[]) { this.#rows = rows; }

    constructor(public readonly sheet: Worksheet, public readonly headers: Set<string>) {
    }

    get isUnderused() { return this.rows.filter(r => r.isUnderused).length > 0; }
    get underusedRows() { return this.rows.filter(r => r.isUnderused); }

    has(...headers: string[]) {
        return headers.every(header => this.headers.has(header));
    }

    matches(...headers: string[]) {
        return headers.length === this.headers.size
            && this.has(...headers);
    }

    globalReplaceValue(v1: string, v2: string) {
        for (const row of this.rows) {
            row.globalReplaceValue(v1, v2);
        }
    }
}

export class WorksheetRow {
    public static fromDict(table: WorksheetTable, data: Map<string, string>) {
        const row = new WorksheetRow(table);
        row.#setCells(mapMap(
            data,
            k => k,
            (value, key) => new WorksheetCell(row, key, value)
        ));
        return row;
    }

    #cells: Map<string, WorksheetCell>;
    get cells() { return this.#cells; }
    #setCells(cells: Map<string, WorksheetCell>) { this.#cells = cells; }

    constructor(public readonly table: WorksheetTable) {
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
}

export class WorksheetCell {
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

    constructor(public readonly row: WorksheetRow, public readonly key: string, value: string) {
        this.#value = value;
    }

    replaceValue(v1: string, v2: string) {
        if (this.#value.trim().toLocaleLowerCase() == v1.toLocaleLowerCase()) {
            this.#value = v2;
        }
    }
}