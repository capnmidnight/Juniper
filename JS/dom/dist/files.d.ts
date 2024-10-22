import type { IProgress } from "@juniper-lib/progress";
export declare function polyfillFileSystemAPI(): void;
/**
 * Invoke a file save dialog
 * @param fileName
 * @param contentType the content type to allow
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showSaveFilePicker
 */
export declare function getFileSystemFileWritableStream(fileName: string, contentType: FilePickerAcceptType): Promise<FileSystemWritableFileStream>;
/**
 * Invoke a file open dialog
 * @param allowMultiple
 * @param contentType the content type to allow
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getFileSystemFileHandles(allowMultiple: boolean, contentType?: FilePickerAcceptType): Promise<FileSystemFileHandle[]>;
export declare function fileExists(dir: FileSystemDirectoryHandle, name: string): Promise<boolean>;
/**
 * Read the contents of a blob as a text file.
 * @param blob
 * @param encoding
 */
export declare function readAsText(blobOrHandle: Blob | File | FileSystemFileHandle, encoding?: string): Promise<string>;
/**
 * Invoke a file open dialog and return the results as text.
 * @param allowMultiple
 * @param contentType the content type to allow
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getTextFiles(allowMultiple: boolean, contentType?: FilePickerAcceptType, encoding?: string): Promise<Map<string, string>>;
/**
 * Read the contents of a blob or file system file handle as a JSON object.
 * @param blob
 * @param encoding
 */
export declare function readAsJSON(blob: Blob | File | FileSystemFileHandle, encoding?: string): Promise<any>;
/**
 * Invoke a file open dialog and return the results as Objects.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getJSONFiles(allowMultiple: boolean, encoding?: string): Promise<Map<string, unknown>>;
/**
 * Read the contents of a blob or file system file handle as a JSON object.
 * @param blob
 * @param encoding
 */
export declare function readAsXML(blob: Blob | File | FileSystemFileHandle, encoding?: string, contentType?: DOMParserSupportedType): Promise<Document>;
/**
 * Invoke a file open dialog and return the results as an XML DOM Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export declare function getXMLFiles(allowMultiple: boolean, encoding?: string): Promise<Map<string, Document>>;
/**
 * Invoke a file save dialog for a set of string data.
 * @param blob
 * @param fileName
 * @param contentType the content type to allow
 * @returns a promise the resolves to null if the save was successful.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showSaveFilePicker
 */
export declare function saveAs(blob: Blob | BlobPart[] | Promise<Blob | BlobPart[]> | (() => Promise<Blob | BlobPart[]>), fileName: string, contentType: FilePickerAcceptType): Promise<void>;
/**
 * Save an object to a JSON file.
 * @param fileName the name of the file to save.
 * @param value A JavaScript value, usually an object or array, to be converted.
 * @param replacer An array of strings and numbers that acts as an approved list for selecting the object properties that will be stringified.
 * @param space Adds indentation, white space, and line break characters to the return-value JSON text to make it easier to read.
 **/
export declare function saveAsJSON<T>(fileName: string, value: T, replacer?: (key: any, value: any) => any, space?: string | number): Promise<void>;
/**
 * Save a Canvas to a PNG file.
 * @param fileName the name of the file to save.
 * @param canvas the HTML Canvas element to save.
 **/
export declare function saveCanvasAsPNG(fileName: string, canvas: HTMLCanvasElement): Promise<void>;
/**
 * Save a Canvas to a PNG file.
 * @param fileName the name of the file to save.
 * @param svgElement the SVG element to save.
 **/
export declare function saveSVGAsPNG(fileName: string, svgElement: SVGSVGElement): Promise<void>;
/**
 *
 * @param fileName
 * @param fieldDescriptions
 * @param values
 * @returns
 */
export declare function saveAsCSV(fileName: string, fieldDescriptions: [string, string][], values: any[]): Promise<void>;
/**
 * Read the contents of a `File` and return a `string`
 */
export declare function readFile(file: File, read: (reader: FileReader) => void, prog?: IProgress): Promise<string | ArrayBuffer>;
/**
 * Read the contents of a `File` and return an `ArrayBuffer`
 */
export declare function fileToArrayBuffer(file: File, prog?: IProgress): Promise<ArrayBuffer>;
/**
 * Read the contents of a `File` and return a `string`
 */
export declare function fileToText(file: File, encoding?: string, prog?: IProgress): Promise<string>;
//# sourceMappingURL=files.d.ts.map