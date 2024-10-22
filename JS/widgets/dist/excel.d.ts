import * as XLSX from "xlsx";
import { SheetDef } from "./DataTableElement";
import { Workbook } from "./Workbook";
export declare function importXLSX(file: Blob | ArrayBuffer, opts?: XLSX.ParsingOptions): Promise<XLSX.WorkBook>;
export declare function exportXLSX(fileName: string, ...sheetDefs: SheetDef<any>[]): Promise<void>;
/**
 * Invoke a file open dialog and return the results as an XLSX Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getWorkbooksFromXLSXFiles(allowMultiple: boolean): Promise<Workbook[]>;
//# sourceMappingURL=excel.d.ts.map