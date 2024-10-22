import { getXMLFiles } from "@juniper-lib/dom";
import { Workbook } from "./Workbook";
/**
 * Invoke a file open dialog and return the results as an XmlSpreadsheet2003 Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getXMLSpreadsheet2003Files(allowMultiple) {
    const xmls = await getXMLFiles(allowMultiple);
    return Array.from(xmls).map(([name, doc]) => Workbook.fromXmlSpreadsheet2003(name, doc));
}
//# sourceMappingURL=getXMLSpreadsheet2003Files.js.map