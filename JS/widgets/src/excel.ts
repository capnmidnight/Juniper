import { Exception, isString } from "@juniper-lib/util";
import { A, getFileSystemFileHandles, saveAs } from "@juniper-lib/dom";
import { Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet } from "@juniper-lib/mediatypes";
import type { SheetOptions } from "excellentexport";
import EE from "excellentexport";
import type { CellPatterns, CellType, FormatDefinition } from "excellentexport/dist/format";
import * as XLSX from "xlsx";
import { BaseColumnDef, ColumnType, SheetDef } from "./DataTableElement";
import { Workbook, Worksheet, WorksheetRow, WorksheetTable } from "./Workbook";

export async function importXLSX(file: Blob | ArrayBuffer, opts?: XLSX.ParsingOptions) {
    if (file instanceof Blob) {
        file = await file.arrayBuffer();
    }

    const workbook = XLSX.read(file, opts);

    return workbook;
}

function exportXLSXSheet<T>(sheetDef: SheetDef<T>): SheetOptions {
    return {
        name: sheetDef.name,
        from: { array: getExportData(sheetDef) },
        formats: getFormats(sheetDef)
    };
}

function getExportData<T>(sheetDef: SheetDef<T>) {
    return [
        sheetDef.columns.map(c => c.header),
        ...sheetDef.data.map(r =>
            sheetDef.columns.map((_c, i) => {
                const value = r[i];
                if (value === null
                    || value === undefined
                    || Number.isNaN(value)
                    || (typeof value === "number" && !Number.isFinite(value))) {
                    return null;
                }
                else {
                    return value.toString();
                }
            })
        )
    ];
}

function getFormats<T>(sheetDef: SheetDef<T>): FormatDefinition[] {
    return [
        // headers row
        {
            range: XLSX.utils.encode_range(
                { r: 0, c: 0 },
                { r: 0, c: sheetDef.columns.length - 1 }
            ),
            format: { type: "s" }
        },
        // data columns
        ...sheetDef.columns.map((c, i) =>
            getFormat(sheetDef, c, i))
    ];
}

function columnTypeToXSLXFormat(type: ColumnType): CellType {
    if (type === "date") {
        return "d";
    }
    else if (type === "number"
        || type === "dollars"
        || type === "integer"
        || type === "percent") {
        return "n";
    }
    else if (type === "boolean") {
        return "b";
    }
    else {
        return "s";
    }
}


function columnTypeToXSLXPattern(type: ColumnType): CellPatterns {
    if (type === "date") {
        return "mm/dd/yyyy" as CellPatterns;
    }
    else if (type === "number") {
        return "0.00" as CellPatterns;
    }
    else if (type === "dollars") {
        return "[$$-409]#,##0.00;[RED]-[$$-409]#,##0.00" as CellPatterns;
    }
    else if (type === "integer") {
        return "0" as CellPatterns;
    }
    else if (type === "percent") {
        return "0.00%" as CellPatterns;
    }
    else if (type === "string") {
        return "@" as CellPatterns;
    }
    else {
        return null;
    }
}

function getFormat<T>(sheetDef: SheetDef<T>, c: BaseColumnDef<T>, i: number): FormatDefinition {
    return {
        range: XLSX.utils.encode_range(
            { c: i, r: 1 },
            { c: i, r: sheetDef.data.length }
        ),
        format: {
            type: columnTypeToXSLXFormat(c.type),
            pattern: columnTypeToXSLXPattern(c.type)
        }
    };
}

export async function exportXLSX(fileName: string, ...sheetDefs: SheetDef<any>[]) {

    const sheets: SheetOptions[] = sheetDefs.map(exportXLSXSheet);

    const anchor = A();

    EE.convert({
        format: "xlsx",
        anchor
    }, sheets);

    const blobURL = anchor.href;
    const request = fetch(blobURL);
    const response = await request;
    const blob = await response.blob();

    await saveAs(blob, fileName, {
        description: "Excel Workbook (*.xlsx)",
        accept: {
            "application/vnd.ms-excel": [".xlsx"]
        }
    });
}

/**
 * Invoke a file open dialog and return the results as an XLSX Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getWorkbooksFromXLSXFiles(allowMultiple: boolean) {
    const handles = await getFileSystemFileHandles(allowMultiple, {
        description: "XLSX Files",
        accept: Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet.toFileSystemAPIAccepts()
    });
    return await Promise.all(handles.map(handle =>
        loadWorkbookFromXLSX(handle)));
}

function loadWorkbookFromXLSX(blob: Blob, name?: string, opts?: XLSX.ParsingOptions): Promise<Workbook>;
function loadWorkbookFromXLSX(file: File | FileSystemFileHandle, opts?: XLSX.ParsingOptions): Promise<Workbook>;
async function loadWorkbookFromXLSX(blobOrFile: File | FileSystemFileHandle | Blob, nameOrOpts?: string | XLSX.ParsingOptions, opts?: XLSX.ParsingOptions): Promise<Workbook> {
    let name: string = null;

    if (isString(nameOrOpts)) {
        name = nameOrOpts;
    }
    else {
        opts = nameOrOpts;
    }

    const blob = blobOrFile instanceof FileSystemFileHandle
        ? await blobOrFile.getFile()
        : blobOrFile;

    if (blob instanceof File) {
        name = blob.name;
    }

    const root = await importXLSX(blob, opts);

    const workbook = new Workbook(name);
    workbook.setSheets(root.SheetNames.map(e =>
        worksheetFromXSLX(workbook, e, root.Sheets[e])
    ));
    return workbook;
}

function worksheetFromXSLX(book: Workbook, sheetName: string, sheet: XLSX.WorkSheet): any {
    const worksheet = new Worksheet(book, sheetName);
    worksheet.setTable(worksheetTableFromXSLX(worksheet, sheet))
    return worksheet;
}


function worksheetTableFromXSLX(worksheet: Worksheet, sheet: XLSX.WorkSheet): WorksheetTable {
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
        headers
    );

    table.setRows(
        rows.map(row => {
            const dict = new Map<string, { style: string, value: string }>();
            for (let index = 0; index < row.length; ++index) {
                if (0 <= index && index < headers.length) {
                    const style = "String";
                    const value = row[index];
                    const header = headers[index];
                    dict.set(header, { style, value });
                }
            }

            return WorksheetRow.fromDict(table, dict);
        })
    );

    return table;
}
