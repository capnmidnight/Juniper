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
export declare function identity<T>(v: T): T;
/**
 * When mapping over an array, returns the index of the array items.
 * @param _ unused
 * @param i the index of the item
 */
export declare function getIndex<T>(_: T, i: number): number;
/**
 * Creates a callback that always returns a specific value.
 * @param v The value to return
 * @returns
 */
export declare function always<T>(v: T): () => T;
/**
 * Always returns `false`, exactly.
 */
export declare function alwaysFalse(_ignored?: any): false;
/**
 * Always returns `true`, exactly.
 */
export declare function alwaysTrue(_ignored?: any): true;
/**
 * Always returns `null`, exactly.
 */
export declare function alwaysNull(_ignored?: any): null;
/**
 * Always returns `undefined`, exactly.
 */
export declare function alwaysUndefined(_ignored?: any): undefined;
/**
 * Never returns anything.
 */
export declare function alwaysVoid(_ignored?: any): void;
/**
 * Creates a callback function that reads a field of a given
 * name from a object.
 */
export declare function readField<T>(fieldName: keyof T): (item: T) => T[keyof T];
export declare function filterJoin<T>(a: boolCallback<T>, b: boolCallback<T>): boolCallback<T>;
export declare function nothing(): void;
export declare function negate(value: number): number;
export declare function not(value: boolean): boolean;
export declare function and(a: boolean, b: boolean): boolean;
export declare function or(a: boolean, b: boolean): boolean;
export declare function xor(a: boolean, b: boolean): boolean;
export declare function nand(a: boolean, b: boolean): boolean;
export declare function nor(a: boolean, b: boolean): boolean;
export declare function equal<T>(a: T, b: T): boolean;
export declare function reflectValue<T>(v: T): () => T;
//# sourceMappingURL=filters.d.ts.map