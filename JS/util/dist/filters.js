/**
 * Standard pass-through function. When used as a filter, it will reject any falsy values.
 */
export function identity(v) { return v; }
/**
 * When mapping over an array, returns the index of the array items.
 * @param _ unused
 * @param i the index of the item
 */
export function getIndex(_, i) {
    return i;
}
/**
 * Creates a callback that always returns a specific value.
 * @param v The value to return
 * @returns
 */
export function always(v) { return () => v; }
/**
 * Always returns `false`, exactly.
 */
export function alwaysFalse(_ignored) { return false; }
/**
 * Always returns `true`, exactly.
 */
export function alwaysTrue(_ignored) { return true; }
/**
 * Always returns `null`, exactly.
 */
export function alwaysNull(_ignored) { return null; }
/**
 * Always returns `undefined`, exactly.
 */
export function alwaysUndefined(_ignored) { return undefined; }
/**
 * Never returns anything.
 */
export function alwaysVoid(_ignored) { }
/**
 * Creates a callback function that reads a field of a given
 * name from a object.
 */
export function readField(fieldName) {
    return (item) => item[fieldName];
}
export function filterJoin(a, b) {
    return (item) => a(item) && b(item);
}
export function nothing() {
}
export function negate(value) {
    return -value;
}
export function not(value) {
    return !value;
}
export function and(a, b) {
    return a && b;
}
export function or(a, b) {
    return a || b;
}
export function xor(a, b) {
    return a !== b;
}
export function nand(a, b) {
    return not(and(a, b));
}
export function nor(a, b) {
    return not(or(a, b));
}
export function equal(a, b) {
    return a === b;
}
export function reflectValue(v) {
    return () => v;
}
//# sourceMappingURL=filters.js.map