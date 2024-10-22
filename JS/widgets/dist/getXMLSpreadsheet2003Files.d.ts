import { Workbook } from "./Workbook";
/**
 * Invoke a file open dialog and return the results as an XmlSpreadsheet2003 Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getXMLSpreadsheet2003Files(allowMultiple: boolean): Promise<Workbook[]>;
//# sourceMappingURL=getXMLSpreadsheet2003Files.d.ts.map