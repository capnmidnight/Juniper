import { isArrayBufferView, isString } from "juniper-tslib";

export function isHTMLElement(obj: any): obj is HTMLElement {
    return obj instanceof HTMLElement;
}

export function isXHRBodyInit(obj: any): obj is BodyInit {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || obj instanceof ArrayBuffer
        || obj instanceof ReadableStream
        || "Document" in globalThis && obj instanceof Document;
}