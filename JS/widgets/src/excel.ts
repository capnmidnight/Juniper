import { A, saveAs } from "@juniper-lib/dom";
import type { SheetOptions } from "excellentexport";
import EE from "excellentexport";
import type { CellPatterns, CellType, FormatDefinition } from "excellentexport/dist/format";
import * as XLSX from "xlsx";
import { FieldName } from '../../util/src/features';

export type ColumnType =
    | "array"
    | "boolean"
    | "button"
    | "color"
    | "date"
    | "dollars"
    | "enum"
    | "html"
    | "integer"
    | "link"
    | "none"
    | "number"
    | "percent"
    | "string";

export interface BaseColumnDef<T> {
    field: FieldName<T>;
    header: string;
    type: ColumnType;
    hidden?: boolean;
}

function columnTypeToXSLXFormat(type: ColumnType): CellType {
    if (type === "date") {
        return EE.cellTypes.DATE;
    }
    else if (type === "number"
        || type === "dollars"
        || type === "integer"
        || type === "percent") {
        return EE.cellTypes.NUMBER;
    }
    else if (type === "boolean") {
        return EE.cellTypes.BOOLEAN;
    }
    else {
        return EE.cellTypes.TEXT;
    }
}


function columnTypeToXSLXPattern(type: ColumnType): CellPatterns {
    if (type === "date") {
        return "mm/dd/yyyy" as CellPatterns;
    }
    else if (type === "number") {
        return EE.cellPatterns.DECIMAL;
    }
    else if (type === "dollars") {
        return EE.cellPatterns.CURRENCY;
    }
    else if (type === "integer") {
        return EE.cellPatterns.INTEGER;
    }
    else if (type === "percent") {
        return EE.cellPatterns.PERCENTAGE;
    }
    else if (type === "string") {
        return EE.cellPatterns.TEXT;
    }
    else {
        return null;
    }
}


export interface SheetDef<T> {
    name: string;
    columns: BaseColumnDef<T>[];
    data: any[][];
}

export async function importXLSX(file: Blob | ArrayBuffer, opts?: XLSX.ParsingOptions) {
    if (file instanceof Blob) {
        file = await file.arrayBuffer();
    }

    const workbook = XLSX.read(file, opts);

    return workbook;
}

export async function exportXLSX<T>(fileName: string, ...sheetDefs: SheetDef<T>[]) {

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