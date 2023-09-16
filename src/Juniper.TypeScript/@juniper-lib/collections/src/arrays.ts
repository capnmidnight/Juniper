import { isDefined, isFunction, isNullOrUndefined, isNumber, isObject, isString } from "@juniper-lib/tslib/dist/typeChecks";

interface IComparable<T> {
    compareTo(b: T): number;
}

function isIComparable<T>(obj: any): obj is IComparable<T> {
    return isObject(obj)
        && "compareTo" in obj
        && isFunction(obj.compareTo);
}

export type Comparable = number | Date | string | IComparable<any>;
export type CompareDirection = "ascending" | "descending";
export type CompareFunction<T> = ((a: T, b: T) => number) & {
    direction: CompareDirection;
}

export type ComparableSelector<T> = (obj: T) => Comparable;

export function compareBy<T>(direction: CompareDirection, ...getKeys: ComparableSelector<T>[]): CompareFunction<T>;
export function compareBy<T>(...getKeys: ComparableSelector<T>[]): CompareFunction<T>;
export function compareBy<T>(directionOrFirstKeyGetter: CompareDirection | ComparableSelector<T>, ...getKeys: ((obj: T) => Comparable)[]): CompareFunction<T> {
    let direction: CompareDirection = null;
    if (isString(directionOrFirstKeyGetter)) {
        direction = directionOrFirstKeyGetter;
    }
    else {
        direction = "ascending";
        getKeys.unshift(directionOrFirstKeyGetter);
    }

    const d = direction === "ascending" ? 1 : -1;

    const comparer = (a: T, b: T) => {
        if (a === b) {
            return 0;
        }

        for (const getKey of getKeys) {
            const keyA = isNullOrUndefined(a) ? null : getKey(a);
            const keyB = isNullOrUndefined(b) ? null : getKey(b);
            const relation = keyA === keyB
                ? 0
                : isString(keyA) && isString(keyB)
                    ? d * keyA.localeCompare(keyB)
                    : isIComparable(keyA) && isIComparable(keyB)
                        ? d * keyA.compareTo(keyB)
                        : direction === "ascending" && keyA > keyB
                            || direction === "descending" && keyA < keyB
                            ? 1 : -1;

            if (relation !== 0) {
                return relation;
            }
        }

        return 0;
    };

    return Object.assign(comparer, {
        direction
    });
}

export type SearchMode = "append" | "prepend" | "search";

export function binarySearch<T>(arr: ArrayLike<T>, searchValue: T, comparer: CompareFunction<T>, mode: SearchMode = "search") {
    let left = 0;
    let right = arr.length - 1;
    while (left <= right) {
        let mid = (left + right) >> 1;
        let relation = comparer(arr[mid], searchValue);
        if (relation === 0) {
            if (mode !== "search") {
                const scanDirection = mode === "append" ? 1 : -1;
                if (scanDirection > 0) {
                    mid += scanDirection;
                }
                while (0 <= mid
                    && mid < arr.length
                    && (relation = comparer(arr[mid], searchValue)) === 0) {
                    mid += scanDirection;
                }
                if (scanDirection < 0) {
                    mid -= scanDirection;
                }
            }

            return mid;
        }
        else if (relation < 0) {
            left = mid - relation;
        }
        else {
            right = mid - relation;
        }
    }

    return -left - 1;
}

export function insertSorted<T>(arr: T[], val: T, idx: number): number;
export function insertSorted<T>(arr: T[], val: T, idx: number, mode: SearchMode): number;
export function insertSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>): number;
export function insertSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>, mode: SearchMode): number;
export function insertSorted<T>(arr: T[], val: T, comparerOrIdx: CompareFunction<T> | number, mode: SearchMode = "search"): number {
    let idx: number = null;
    if (isNumber(comparerOrIdx)) {
        idx = comparerOrIdx;
    }
    else {
        idx = binarySearch(arr, val, comparerOrIdx, mode);
    }

    if (idx < 0) {
        idx = -idx - 1;
    }

    arrayInsertAt(arr, val, idx);
    return idx;
}

export function removeSorted<T>(arr: T[], val: T, comparer: CompareFunction<T>): number {
    const idx = binarySearch(arr, val, comparer);
    if (idx >= 0) {
        arrayRemoveAt(arr, idx);
        return idx;
    }
    return -1;
}


/**
 * Empties out an array, returning the items that were in the array.
 * 
 * @param arr the array to empty
 */
export function arrayClear<T>(arr: T[]) {
    return arr.splice(0);
}

/**
 * Checks to see if two arrays contain the same elements
 * @returns -1 if the arrays match, the index of the first mismatched item if they don't.
 * @param arr1
 * @param arr2
 */

export function arrayCompare<T>(arr1: ReadonlyArray<T>, arr2: ReadonlyArray<T>): number {
    for (let i = 0; i < arr1.length; ++i) {
        if (arr1[i] !== arr2[i]) {
            return i;
        }
    }

    return -1;
}


export function arrayGen<T>(count: number, thunk: (i: number) => T): T[] {
    return Array.from(iterableGen(count, thunk));
}

export function* iterableGen<T>(count: number, thunk: (i: number) => T) {
    for (let i = 0; i < count; ++i) {
        yield thunk(i);
    }
}


/**
 * Inserts an item at the given index into an array.
 * @param arr
 * @param item
 * @param idx
 */

export function arrayInsertAt<T>(arr: T[], item: T, idx: number) {
    arr.splice(idx, 0, item);
}

/**
 * Returns a random item from an array of items.
 *
 * Provides an option to consider an additional item as part of the collection
 * for random selection.
 */
export function arrayRandom<T>(arr: T[], defaultValue?: T): T | undefined {
    const offset = defaultValue != null ? 1 : 0,
        idx = Math.floor(Math.random() * (arr.length + offset)) - offset;
    if (idx < 0) {
        return defaultValue;
    }
    else {
        return arr[idx];
    }
}


/**
 * Removes a given item from an array, returning true if the item was removed.
 */
export function arrayRemove<T>(arr: T[], value: T) {
    const idx = arr.indexOf(value);
    if (idx > -1) {
        arrayRemoveAt(arr, idx);
        return true;
    }

    return false;
}

export function arrayFilter<T>(arr: T[], predicate: (v: T) => boolean): T | null {
    for (let i = arr.length - 1; i >= 0; --i) {
        if (predicate(arr[i])) {
            return arrayRemoveAt(arr, i);
        }
    }

    return null;
}

export function arrayRemoveByKey<T, K>(arr: T[], key: K, getKey: (v: T) => K): T | null {
    return arrayFilter(arr, (v) => getKey(v) === key);
}

/**
 * Removes an item at the given index from an array.
 */
export function arrayRemoveAt<T>(arr: T[], idx: number) {
    return arr.splice(idx, 1)[0];
}

/**
 * Replaces all of the items in an array with the given items.
 *
 * This helps reduce GC pressure as you're not creating arrays
 * and then dropping them on the floor.
 * 
 * @param arr the array to fill
 * @param items the items to put into the array
 */
export function arrayReplace<T>(arr: T[], ...items: T[]) {
    arr.splice(0, arr.length, ...items);
}

export function arrayCreate<T>(count: number, make: (i: number, len?: number) => T): T[] {
    const arr = new Array<T>(count);
    for (let i = 0; i < count; ++i) {
        arr[i] = make(i, count);
    }
    return arr;
}


function _arrayScan<T>(forward: boolean, arr: readonly T[], tests: ((val: T) => boolean)[]): T {
    const start = forward ? 0 : arr.length - 1;
    const end = forward ? arr.length : -1;
    const inc = forward ? 1 : -1;
    for (const test of tests) {
        for (let i = start; i != end; i += inc) {
            const item = arr[i];
            if (test(item)) {
                return item;
            }
        }
    }

    return null;
}

/**
 * Scans through a series of filters to find an item that matches
 * any of the filters. The first item of the first filter that matches
 * will be returned.
 */
export function arrayScan<T, S extends T>(arr: readonly T[], ...tests: ((val: T) => val is S)[]): S;
export function arrayScan<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T
export function arrayScan<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T {
    return _arrayScan(true, arr, tests);
}

/**
 * Scans through a series of filters to find an item that matches
 * any of the filters. The last item of the first filter that matches
 * will be returned.
 */
export function arrayScanReverse<T, S extends T>(arr: readonly T[], ...tests: ((val: T) => val is S)[]): S;
export function arrayScanReverse<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T;
export function arrayScanReverse<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]): T {
    return _arrayScan(false, arr, tests);
}

export function arrayShuffleInplace<T>(arr: T[]): void {
    for (let i = 0; i < arr.length - 1; ++i) {
        const subLength = arr.length - i;
        const subIndex = Math.floor(Math.random() * subLength);
        const temp = arr[i];
        const j = subIndex + i;
        arr[i] = arr[j];
        arr[j] = temp;
    }
}

export function arrayShuffle<T>(arr: readonly T[]): T[] {
    const output = arr.slice();
    arrayShuffleInplace(output);
    return output;
}

const numericPattern = /^(-?(?:\d+\.)\d+)/;
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
export function arraySortNumericByKey<T>(arr: ReadonlyArray<T>, keySelector: (obj: T) => string): T[] {
    const comparer = compareBy<T>(v => {
        const key = keySelector(v);
        const match = key.match(numericPattern);
        if (isDefined(match)) {
            return parseFloat(match[1]);
        }
        return key;
    });

    return Array
        .from(arr)
        .sort(comparer);
}

export function arrayZip<T, V>(arr1: readonly T[], arr2: readonly T[], combine: (a: T, b: T) => V): V[] {
    const len = Math.max(arr1.length, arr2.length);
    const output = new Array<V>(len);
    for (let i = 0; i < len; ++i) {
        output[i] = combine(arr1[i], arr2[i]);
    }

    return output;
}
