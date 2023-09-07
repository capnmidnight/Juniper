function t(o, s, c) {
    return typeof o === s
        || o instanceof c;
}
export function isFunction(obj) {
    return t(obj, "function", Function);
}
export function isString(obj) {
    return t(obj, "string", String);
}
export function isBoolean(obj) {
    return t(obj, "boolean", Boolean);
}
export function isNumber(obj) {
    return t(obj, "number", Number);
}
/**
 * Check a value to see if it is of a number type
 * and is the special NaN value or one of the Infinities.
 */
export function isBadNumber(num) {
    return isNullOrUndefined(num)
        || !Number.isFinite(num)
        || Number.isNaN(num);
}
/**
 * Check a value to see if it is of a number type
 * and is not the special NaN value nor one of the Infinities.
 */
export function isGoodNumber(obj) {
    return isNumber(obj)
        && !isBadNumber(obj);
}
export function isObject(obj) {
    return isDefined(obj)
        && t(obj, "object", Object);
}
export function isPromise(obj) {
    return obj instanceof Promise;
}
export function isDate(obj) {
    return obj instanceof Date;
}
export function isArray(obj) {
    return obj instanceof Array;
}
export function assertNever(x, msg) {
    throw new Error((msg || "Unexpected object: ") + x);
}
export function isNullOrUndefined(obj) {
    return obj === null
        || obj === undefined;
}
export function isDefined(obj) {
    return !isNullOrUndefined(obj);
}
export function isEventListener(obj) {
    return isFunction(obj);
}
export function isEventListenerObject(obj) {
    return !isEventListener(obj);
}
export function isArrayBufferView(obj) {
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
export function isArrayBuffer(val) {
    return val && typeof ArrayBuffer !== "undefined" &&
        (val instanceof ArrayBuffer ||
            // Sometimes we get an ArrayBuffer that doesn't satisfy instanceof
            (val.constructor && val.constructor.name === "ArrayBuffer"));
}
//# sourceMappingURL=typeChecks.js.map