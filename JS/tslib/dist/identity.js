export function identity(item) {
    return item;
}
export function nothing() {
}
export function negate(value) {
    return -value;
}
export function alwaysTrue() {
    return true;
}
export function alwaysFalse() {
    return false;
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
//# sourceMappingURL=identity.js.map