function t(o: any, s: string, c: Function) {
    return typeof o === s
        || o instanceof c;
}

export function isFunction(obj: any): obj is Function {
    return t(obj, "function", Function);
}

export function isString(obj: any): obj is string {
    return t(obj, "string", String);
}

export function isBoolean(obj: any): obj is boolean {
    return t(obj, "boolean", Boolean);
}

export function isNumber(obj: any): obj is number {
    return t(obj, "number", Number);
}

/**
 * Check a value to see if it is of a number type
 * and is the special NaN value or one of the Infinities.
 */
export function isBadNumber(num: number): boolean {
    return isNullOrUndefined(num)
        || !Number.isFinite(num)
        || Number.isNaN(num);
}

/**
 * Check a value to see if it is of a number type
 * and is not the special NaN value nor one of the Infinities.
 */
export function isGoodNumber(obj: any): obj is number {
    return isNumber(obj)
        && !isBadNumber(obj);
}

export function isObject(obj: any): obj is object {
    return isDefined(obj)
        && t(obj, "object", Object);
}

export function isPromise<T>(obj: any): obj is Promise<T> {
    return obj instanceof Promise;
}

export function isDate(obj: any): obj is Date {
    return obj instanceof Date;
}

export function isArray(obj: any): obj is Array<any> {
    return obj instanceof Array;
}

export function isSingleDimArray<T>(arr: T[] | T[][] | T[][][]): arr is T[] {
    return isArray(arr)
        && arr.length > 0
        && !isArray(arr[0])
}

export function assertNever(x: never, msg?: string): never {
    throw new Error((msg || "Unexpected object: ") + x);
}

export function isNullOrUndefined<T>(obj: T | null | undefined | void): obj is null | undefined | void {
    return obj === null
        || obj === undefined;
}

export function isDefined<T>(obj: T | null | undefined | void): obj is T {
    return obj !== null
        && obj !== undefined;
}

export function isEventListener(obj: EventListenerOrEventListenerObject): obj is EventListener {
    return isFunction(obj);
}

export function isEventListenerObject(obj: EventListenerOrEventListenerObject): obj is EventListenerObject {
    return !isEventListener(obj);
}

export function isArrayBufferView(obj: any): obj is ArrayBufferView {
    return obj instanceof Uint8Array
        || obj instanceof Uint8ClampedArray
        || obj instanceof Int8Array
        || obj instanceof Uint16Array
        || obj instanceof Int16Array
        || obj instanceof Uint32Array
        || obj instanceof Int32Array
        || obj instanceof Float32Array
        || obj instanceof Float64Array
        || "BigUint64Array" in globalThis && obj instanceof globalThis["BigUint64Array"]
        || "BigInt64Array" in globalThis && obj instanceof globalThis["BigInt64Array"];
}

export function isArrayBuffer(val: any): val is ArrayBuffer {
    if (isNullOrUndefined(val)) {
        return false;
    }

    if (typeof ArrayBuffer !== "undefined") {
        if (val instanceof ArrayBuffer ||
            // Sometimes we get an ArrayBuffer that doesn't satisfy instanceof
            (val.constructor && val.constructor.name === "ArrayBuffer")) {
            return true;
        }
    }

    if (typeof SharedArrayBuffer !== "undefined") {
        if (val instanceof SharedArrayBuffer ||
            // Sometimes we get an ArrayBuffer that doesn't satisfy instanceof
            (val.constructor && val.constructor.name === "SharedArrayBuffer")) {
            return true;
        }
    }

    return false;
}
