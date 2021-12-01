import { isArrayBufferView, isString } from "juniper-tslib";

export function isXHRBodyInit(obj: any): obj is BodyInit {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || obj instanceof ArrayBuffer
        || obj instanceof ReadableStream;
}