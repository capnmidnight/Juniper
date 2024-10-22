interface IComparable<T> {
    compareTo(b: T): number;
}
export type Comparable = number | Date | string | IComparable<any>;
export type CompareDirection = "ascending" | "descending";
export type CompareFunction<T> = ((a: T, b: T) => number) & {
    direction: CompareDirection;
};
export type ComparableSelector<T> = (obj: T) => Comparable;
export declare function compareBy<T>(direction: CompareDirection, ...getKeys: ComparableSelector<T>[]): CompareFunction<T>;
export declare function compareBy<T>(...getKeys: ComparableSelector<T>[]): CompareFunction<T>;
export type SearchMode = "append" | "prepend" | "search";
export declare function binarySearch<T>(arr: ArrayLike<T>, searchValue: T, comparer: CompareFunction<T>, mode?: SearchMode): number;
export type InsertMode = "set" | SearchMode;
export declare function insertSorted<T>(arr: T[], val: T, idx: number): number;
export declare function insertSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>): number;
export declare function insertSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>, mode: InsertMode): number;
export declare function removeSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>): number;
/**
 * Empties out an array, returning the items that were in the array.
 *
 * @param arr the array to empty
 */
export declare function arrayClear<T>(arr: T[]): T[];
/**
 * Checks to see if two arrays contain the same elements
 * @returns -1 if the arrays match, the index of the first mismatched item if they don't.
 * @param arr1
 * @param arr2
 */
export declare function arrayCompare<T>(arr1: ReadonlyArray<T>, arr2: ReadonlyArray<T>): number;
export declare function arrayGen<T>(count: number, thunk: (i: number) => T): T[];
export declare function iterableGen<T>(count: number, thunk: (i: number) => T): Generator<T, void, unknown>;
/**
 * Inserts an item at the given index into an array.
 * @param arr
 * @param item
 * @param idx
 */
export declare function arrayInsertAt<T>(arr: T[], item: T, idx: number): void;
/**
 * Returns a random item from an array of items.
 *
 * Provides an option to consider an additional item as part of the collection
 * for random selection.
 */
export declare function arrayRandom<T>(arr: T[], defaultValue?: T): T | undefined;
/**
 * Removes a given item from an array, returning true if the item was removed.
 */
export declare function arrayRemove<T>(arr: T[], value: T): boolean;
export declare function arrayFilter<T>(arr: T[], predicate: (v: T) => boolean): T | null;
export declare function arrayRemoveByKey<T, K>(arr: T[], key: K, getKey: (v: T) => K): T | null;
/**
 * Removes an item at the given index from an array.
 */
export declare function arrayRemoveAt<T>(arr: T[], idx: number): T;
/**
 * Replaces all of the items in an array with the given items.
 *
 * This helps reduce GC pressure as you're not creating arrays
 * and then dropping them on the floor.
 *
 * @param arr the array to fill
 * @param items the items to put into the array
 */
export declare function arrayReplace<T>(arr: T[], ...items: T[]): void;
export declare function arrayCreate<T>(count: number, make: (i: number, len?: number) => T): T[];
/**
 * Scans through a series of filters to find an item that matches
 * any of the filters. The first item of the first filter that matches
 * will be returned.
 */
export declare function arrayScan<T, S extends T>(arr: readonly T[], ...tests: ((val: T) => val is S)[]): S;
export declare function arrayScan<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T;
/**
 * Scans through a series of filters to find an item that matches
 * any of the filters. The last item of the first filter that matches
 * will be returned.
 */
export declare function arrayScanReverse<T, S extends T>(arr: readonly T[], ...tests: ((val: T) => val is S)[]): S;
export declare function arrayScanReverse<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T;
export declare function arrayShuffleInplace<T>(arr: T[]): void;
export declare function arrayShuffle<T>(arr: readonly T[]): T[];
/**
 * Creates a new array that is sorted by the key extracted
 * by the keySelector callback, not modifying the input array,
 * (unlike JavaScript's own Array.prototype.sort).
 *
 * If the values have a number at the beginning, they'll be sorted
 * by that number.
 * @param arr
 * @param keySelector
 */
export declare function arraySortNumericByKey<T>(arr: ReadonlyArray<T>, keySelector: (obj: T) => string): T[];
export declare function arrayZip<T, V>(arr1: readonly T[], arr2: readonly T[], combine: (a: T, b: T) => V): V[];
export {};
//# sourceMappingURL=arrays.d.ts.map