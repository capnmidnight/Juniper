import { isArrayBufferView, isString } from "juniper-tslib";
export function isXHRBodyInit(obj) {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || obj instanceof ArrayBuffer
        || obj instanceof ReadableStream;
}
