export type anyCallback<T> = (item: T) => any;
export type unknownCallback<T> = (item: T) => unknown;
export type asyncCallback = () => Promise<void>;
export type boolCallback<T> = (item: T) => boolean;
export type stringCallback<T> = (item: T) => string;
export type numberCallback<T> = (item: T) => number;
export type dateCallback<T> = (item: T) => Date;
export type elementCallback<T> = (item: T) => HTMLElement;
export type eventHandler<T extends Event = Event> = (evt: T) => any;
export type Predicate<T> = (value: T) => boolean;

/**
 * Standard pass-through function. When used as a filter, it will reject any falsy values.
 */
export function identity<T>(v: T): T { return v; }

/**
 * When mapping over an array, returns the index of the array items.
 * @param _ unused
 * @param i the index of the item
 */
export function getIndex<T>(_: T, i: number): number {
    return i;
}

/**
 * Creates a callback that always returns a specific value.
 * @param v The value to return
 * @returns 
 */
export function always<T>(v: T): () => T { return () => v; }

/**
 * Always returns `false`, exactly.
 */
export function alwaysFalse(_ignored?: any): false { return false; }

/**
 * Always returns `true`, exactly.
 */
export function alwaysTrue(_ignored?: any): true { return true; }

/**
 * Always returns `null`, exactly.
 */
export function alwaysNull(_ignored?: any): null { return null; }

/**
 * Always returns `undefined`, exactly.
 */
export function alwaysUndefined(_ignored?: any): undefined { return undefined; }

/**
 * Never returns anything.
 */
export function alwaysVoid(_ignored?: any): void { }

/**
 * Creates a callback function that reads a field of a given
 * name from a object.
 */
export function readField<T>(fieldName: keyof T): (item: T) => T[keyof T] {
    return (item: T) => item[fieldName];
}

export function filterJoin<T>(a: boolCallback<T>, b: boolCallback<T>): boolCallback<T> {
    return (item: T) =>
        a(item) && b(item);
}

export function nothing(): void {
}

export function negate(value: number) {
    return -value;
}

export function not(value: boolean) {
    return !value;
}

export function and(a: boolean, b: boolean): boolean {
    return a && b;
}

export function or(a: boolean, b: boolean): boolean {
    return a || b;
}

export function xor(a: boolean, b: boolean): boolean {
    return a !== b;
}

export function nand(a: boolean, b: boolean): boolean {
    return not(and(a, b));
}

export function nor(a: boolean, b: boolean): boolean {
    return not(or(a, b));
}

export function equal<T>(a: T, b: T): boolean {
    return a === b;
}

export function reflectValue<T>(v: T): () => T {
    return () => v;
}