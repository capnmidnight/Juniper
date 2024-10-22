export declare class Workbook {
    #private;
    static loadXmlSpreadsheet2003(blob: Blob, name?: string): Promise<Workbook>;
    static loadXmlSpreadsheet2003(file: File | FileSystemFileHandle): Promise<Workbook>;
    static fromXmlSpreadsheet2003(name: string, root: Document): Workbook;
    get Name(): string;
    get worksheets(): Worksheet[];
    setSheets(sheets: Worksheet[]): void;
    constructor(name: string);
    globalReplaceValue(v1: string, v2: string): void;
}
export declare class Worksheet {
    #private;
    readonly name: string;
    static fromXmlSpreadsheet2003(book: Workbook, element: Element, styles: Map<string, string>): Worksheet;
    get book(): Workbook;
    get table(): WorksheetTable;
    setTable(table: WorksheetTable): void;
    constructor(book: Workbook, name: string);
}
export declare class WorksheetTable {
    #private;
    static fromXmlSpreadsheet2003(worksheet: Worksheet, element: Element, styles: Map<string, string>): WorksheetTable;
    get sheet(): Worksheet;
    get headers(): string[];
    get rows(): WorksheetRow[];
    setRows(rows: WorksheetRow[]): void;
    constructor(sheet: Worksheet, headers: string[]);
    get isUnderused(): boolean;
    get underusedRows(): WorksheetRow[];
    has(...headers: string[]): boolean;
    get(header: string): string[];
    matches(...headers: string[]): boolean;
    globalReplaceValue(v1: string, v2: string): void;
}
export declare class WorksheetRow {
    #private;
    static fromDict(table: WorksheetTable, data: Map<string, {
        style: string;
        value: string;
    }>): WorksheetRow;
    get table(): WorksheetTable;
    get cells(): Map<string, WorksheetCell>;
    constructor(table: WorksheetTable);
    has(...headers: string[]): boolean;
    peek(header: string): string;
    globalReplaceValue(v1: string, v2: string): void;
    get(header: string): string;
    get isUnderused(): boolean;
    get unusedFields(): string[];
    toObject(): {};
}
export declare class WorksheetCell {
    #private;
    get row(): WorksheetRow;
    get key(): string;
    get style(): string;
    get value(): string;
    /**
     * Get the value without modifying the accessed flag;
     */
    get peek(): string;
    /**
     * A flag indicating that this cell's value has been accessed. Useful
     * for importers that do advanced validation that want to make sure they
     * use all of the cell values in a work sheet.
     */
    get accessed(): boolean;
    constructor(row: WorksheetRow, key: string, value: string, style?: string);
    replaceValue(v1: string, v2: string): void;
    toObject(): {
        [x: string]: string;
    };
}
//# sourceMappingURL=Workbook.d.ts.map