import { identity, isDefined, isNullOrUndefined, isObject, once, pivot, singleton } from "@juniper-lib/util";
import { Application_Json, Image_Png, Text_Plain, Text_Xml } from "@juniper-lib/mediatypes";
import { Target } from "./attrs";
import { renderSVGElement } from "./svg";
import { A, InputFile } from "./tags";
export function polyfillFileSystemAPI() {
    if (!("showOpenFilePicker" in globalThis)) {
        const fileInput = InputFile();
        const anchor = A(Target("_blank"));
        Object.assign(globalThis, {
            async showOpenFilePicker(options) {
                fileInput.multiple = options && options.multiple;
                if (options && options.types) {
                    fileInput.accept = options
                        .types
                        .filter(v => v.accept)
                        .flatMap(v => Object.keys(v.accept))
                        .join(",");
                }
                const task = once(fileInput, "input", "cancel")
                    .catch(() => {
                    throw new DOMException("The user aborted a request.");
                });
                fileInput.click();
                await task;
                return Array.from(fileInput.files)
                    .map(file => {
                    const f = {
                        getFile() {
                            return Promise.resolve(file);
                        },
                        kind: "file",
                        isFile: true,
                        isDirectory: false,
                        name: file.name,
                        createWritable() {
                            throw new Error("Not implemented");
                        },
                        isSameEntry: function (_other) {
                            throw new Error("Function not implemented.");
                        },
                        queryPermission: function (_descriptor) {
                            throw new Error("Function not implemented.");
                        },
                        requestPermission: function (_descriptor) {
                            throw new Error("Function not implemented.");
                        }
                    };
                    return f;
                });
            },
            async showSaveFilePicker(options) {
                const blobParts = new Array();
                return {
                    async createWritable() {
                        return {
                            async write(blob) {
                                blobParts.push(blob);
                                return Promise.resolve();
                            },
                            async close() {
                                const types = options
                                    && options.types
                                    && options.types.filter(v => v.accept)
                                        .flatMap(v => Object.keys(v.accept))
                                        .filter(identity);
                                const type = types && types.length > 0 && types[0] || null;
                                const blob = new Blob(blobParts, { type });
                                const url = URL.createObjectURL(blob);
                                const task = once(anchor, "click");
                                anchor.download = options && options.suggestedName || null;
                                anchor.href = url;
                                anchor.click();
                                await task;
                            }
                        };
                    }
                };
            }
        });
    }
}
function makeFileSystemAPIID() {
    return location.hostname.replace(/\./g, "_");
}
/**
 * Invoke a file save dialog
 * @param fileName
 * @param contentType the content type to allow
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showSaveFilePicker
 */
export async function getFileSystemFileWritableStream(fileName, contentType) {
    polyfillFileSystemAPI();
    const options = {
        id: makeFileSystemAPIID(),
        excludeAcceptAllOption: true,
        suggestedName: fileName,
        types: [contentType]
    };
    const handle = await showSaveFilePicker(options);
    return await handle.createWritable();
}
/**
 * Invoke a file open dialog
 * @param allowMultiple
 * @param contentType the content type to allow
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getFileSystemFileHandles(allowMultiple, contentType) {
    polyfillFileSystemAPI();
    const options = {
        id: makeFileSystemAPIID(),
        startIn: "desktop",
        multiple: allowMultiple,
        excludeAcceptAllOption: !!contentType
    };
    if (contentType) {
        options.types = [contentType];
    }
    try {
        return await showOpenFilePicker(options);
    }
    catch (error) {
        return [];
    }
}
function isFileSystemFileHandle(blob) {
    return "FileSystemFileHandle" in window
        && blob instanceof FileSystemFileHandle;
}
export async function fileExists(dir, name) {
    try {
        const file = await dir.getFileHandle(name, { create: false });
        return isDefined(file);
    }
    catch {
        return false;
    }
}
/**
 * Read the contents of a blob as a text file.
 * @param blob
 * @param encoding
 */
export async function readAsText(blobOrHandle, encoding) {
    const blob = isFileSystemFileHandle(blobOrHandle)
        ? await blobOrHandle.getFile()
        : blobOrHandle;
    const reader = new FileReader();
    const task = once(reader, "load");
    reader.readAsText(blob, encoding);
    await task;
    return reader.result;
}
/**
 * Invoke a file open dialog and return the results as text.
 * @param allowMultiple
 * @param contentType the content type to allow
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getTextFiles(allowMultiple, contentType, encoding) {
    if (!contentType) {
        contentType = {
            description: "Text Files",
            accept: Text_Plain.toFileSystemAPIAccepts()
        };
    }
    const handles = await getFileSystemFileHandles(allowMultiple, contentType);
    return new Map(await Promise.all(handles.map(async (h) => [
        h.name,
        await readAsText(h, encoding)
    ])));
}
/**
 * Read the contents of a blob or file system file handle as a JSON object.
 * @param blob
 * @param encoding
 */
export async function readAsJSON(blob, encoding) {
    return JSON.parse(await readAsText(blob, encoding));
}
/**
 * Invoke a file open dialog and return the results as Objects.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getJSONFiles(allowMultiple, encoding) {
    const handles = await getFileSystemFileHandles(allowMultiple, {
        description: "JSON Files",
        accept: Application_Json.toFileSystemAPIAccepts()
    });
    return new Map(await Promise.all(handles.map(async (h) => [
        h.name,
        await readAsJSON(h, encoding)
    ])));
}
/**
 * Read the contents of a blob or file system file handle as a JSON object.
 * @param blob
 * @param encoding
 */
export async function readAsXML(blob, encoding, contentType) {
    const parser = singleton("Juniper::DOM::files::XMLParser", () => new DOMParser());
    if (isDefined(contentType)) {
        return parser.parseFromString(await readAsText(blob, encoding), contentType);
    }
    else {
        return parser.parseFromString(await readAsText(blob, encoding), Text_Xml.value);
    }
}
/**
 * Invoke a file open dialog and return the results as an XML DOM Object.
 * @param allowMultiple
 * @param encoding
 * @returns a promise the resolves to a writable stream.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showOpenFilePicker
 */
export async function getXMLFiles(allowMultiple, encoding) {
    const handles = await getFileSystemFileHandles(allowMultiple, {
        description: "XML Files",
        accept: Text_Xml.toFileSystemAPIAccepts()
    });
    return new Map(await Promise.all(handles.map(async (h) => [
        h.name,
        await readAsXML(h, encoding, Text_Xml.value)
    ])));
}
/**
 * Invoke a file save dialog for a set of string data.
 * @param blob
 * @param fileName
 * @param contentType the content type to allow
 * @returns a promise the resolves to null if the save was successful.
 * @see https://developer.mozilla.org/en-US/docs/Web/API/window/showSaveFilePicker
 */
export async function saveAs(blob, fileName, contentType) {
    try {
        const writable = await getFileSystemFileWritableStream(fileName, contentType);
        try {
            if (blob instanceof Function) {
                blob = blob();
            }
            if (blob instanceof Promise) {
                blob = await blob;
            }
            if (!(blob instanceof Blob)) {
                const blobType = Object.keys(contentType.accept)[0];
                blob = new Blob(blob, { type: blobType });
            }
            writable.write(blob);
        }
        finally {
            writable.close();
        }
    }
    catch (err) {
        if (isObject(err)
            && "message" in err
            && err.message !== "The user aborted a request.") {
            console.warn(err.message);
        }
        throw err;
    }
}
/**
 * Save an object to a JSON file.
 * @param fileName the name of the file to save.
 * @param value A JavaScript value, usually an object or array, to be converted.
 * @param replacer An array of strings and numbers that acts as an approved list for selecting the object properties that will be stringified.
 * @param space Adds indentation, white space, and line break characters to the return-value JSON text to make it easier to read.
 **/
export function saveAsJSON(fileName, value, replacer, space) {
    const blobParts = [JSON.stringify(value, replacer, space)];
    return saveAs(blobParts, fileName, {
        description: "JSON files",
        accept: Application_Json.toFileSystemAPIAccepts()
    });
}
/**
 * Save a Canvas to a PNG file.
 * @param fileName the name of the file to save.
 * @param canvas the HTML Canvas element to save.
 **/
export async function saveCanvasAsPNG(fileName, canvas) {
    const blob = () => new Promise(resolve => canvas.toBlob(resolve, "image/png"));
    return await saveAs(blob, fileName, {
        description: "Portable Network Graphics (PNG)",
        accept: Image_Png.toFileSystemAPIAccepts()
    });
}
/**
 * Save a Canvas to a PNG file.
 * @param fileName the name of the file to save.
 * @param svgElement the SVG element to save.
 **/
export async function saveSVGAsPNG(fileName, svgElement) {
    const canvas = await renderSVGElement(svgElement);
    return await saveCanvasAsPNG(fileName, canvas);
}
/**
 *
 * @param fileName
 * @param fieldDescriptions
 * @param values
 * @returns
 */
export function saveAsCSV(fileName, fieldDescriptions, values) {
    const [fields, headers] = pivot(fieldDescriptions);
    const body = [
        [...headers],
        ...values.map(value => fields.map(field => escapeValueForCSV(value[field])))
    ];
    return saveAs([body.join("\n")], fileName, {
        description: "CSV files",
        accept: {
            "text/csv": [".csv"]
        }
    });
}
function escapeValueForCSV(value) {
    if (isNullOrUndefined(value)) {
        return "";
    }
    value = value.toString();
    if (/[",\n]/.test(value)) {
        value = `"${value.replace(/"/g, "\"\"")}"`;
    }
    return value;
}
/**
 * Read the contents of a `File` and return a `string`
 */
export async function readFile(file, read, prog) {
    const reader = new FileReader();
    if (prog) {
        reader.addEventListener("loadstart", () => prog.start(file.name));
        reader.addEventListener("loadend", () => prog.end(file.name));
        reader.addEventListener("progress", evt => prog.report(evt.loaded, evt.total, file.name));
    }
    const task = once(reader, "load", "error", "abort");
    read(reader);
    await task;
    return reader.result;
}
/**
 * Read the contents of a `File` and return an `ArrayBuffer`
 */
export function fileToArrayBuffer(file, prog) {
    return readFile(file, (reader) => reader.readAsArrayBuffer(file), prog);
}
/**
 * Read the contents of a `File` and return a `string`
 */
export function fileToText(file, encoding, prog) {
    return readFile(file, (reader) => reader.readAsText(file, encoding), prog);
}
//# sourceMappingURL=files.js.map