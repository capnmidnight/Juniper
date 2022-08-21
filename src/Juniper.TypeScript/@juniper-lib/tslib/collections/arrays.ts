import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "../typeChecks";

function defaultKeySelector<T>(obj: T): any {
    return obj;
}

export function arrayBinarySearchFind<T, V>(arr: T[], key: V, keySelector: (obj: T) => V): T | undefined {
    const idx = arrayBinarySearchByKey(arr, key, keySelector);
    if (Number.isInteger(idx)) {
        return arr[idx - 1];
    }

    return undefined;
}

/**
 * Performs a binary search on a list to find where the item should be inserted.
 *
 * If the item is found, the returned index will be an exact integer.
 *
 * If the item is not found, the returned insertion index will be 0.5 greater than
 * the index at which it should be inserted.
 */
export function arrayBinarySearchByKey<T, V>(arr: T[], itemKey: V, keySelector: (obj: T) => V): number {
    let left = 0;
    let right = arr.length;
    let idx = Math.floor((left + right) / 2);
    let found = false;
    while (left < right && idx < arr.length) {
        const compareTo = arr[idx];
        const compareToKey = isNullOrUndefined(compareTo)
            ? null
            : keySelector(compareTo);
        if (isDefined(compareToKey)
            && itemKey < compareToKey) {
            right = idx;
        }
        else {
            if (itemKey === compareToKey) {
                found = true;
            }
            left = idx + 1;
        }

        idx = Math.floor((left + right) / 2);
    }

    if (!found) {
        idx += 0.5;
    }

    return idx;
}

/**
 * Performs a binary search on a list to find where the item should be inserted.
 *
 * If the item is found, the returned index will be an exact integer.
 *
 * If the item is not found, the returned insertion index will be 0.5 greater than
 * the index at which it should be inserted.
 */
export function arrayBinarySearch<T, V>(arr: T[], item: T, keySelector?: (obj: T) => V): number {
    keySelector = keySelector || defaultKeySelector;
    const itemKey = keySelector(item);
    return arrayBinarySearchByKey(arr, itemKey, keySelector);
}

export function arrayBinaryContains<T, V>(arr: T[], item: T, keySelector?: (obj: T) => V): boolean {
    return Number.isInteger(arrayBinarySearch(arr, item, keySelector));
}

function removeAtIndex<T>(arr: T[], idx: number): boolean {
    if (Number.isInteger(idx)) {
        arrayRemoveAt(arr, idx - 1);
        return true;
    }

    return false;
}

export function arrayBinarySearchRemoveByKey<T, V>(arr: T[], itemKey: V, keySelector: (obj: T) => V): boolean {
    const idx = arrayBinarySearchByKey(arr, itemKey, keySelector);
    return removeAtIndex(arr, idx);
}

export function arrayBinarySearchRemove<T, V>(arr: T[], item: T, keySelector?: (obj: T) => V): boolean {
    const idx = arrayBinarySearch(arr, item, keySelector);
    return removeAtIndex(arr, idx);
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


function _arrayScan<T>(forward: boolean, arr: readonly T[], tests: ((val: T) => boolean)[]) {
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
export function arrayScan<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]) {
    return _arrayScan(true, arr, tests);
}

/**
 * Scans through a series of filters to find an item that matches
 * any of the filters. The last item of the first filter that matches
 * will be returned.
 */
export function arrayScanReverse<T>(arr: readonly T[], ...tests: ((val: T) => boolean)[]) {
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

/**
 * Performs an insert operation that maintains the sort
 * order of the array, returning the index at which the
 * item was inserted.
 */
export function arraySortedInsert<T>(arr: T[], item: T): number;
export function arraySortedInsert<T, V>(arr: T[], item: T, keySelector?: (obj: T) => V): number;
export function arraySortedInsert<T>(arr: T[], item: T, allowDuplicates?: boolean): number;
export function arraySortedInsert<T, V>(arr: T[], item: T, keySelector?: (obj: T) => V, allowDuplicates?: boolean): number;
export function arraySortedInsert<T, V>(arr: T[], item: T, keySelector?: ((obj: T) => V) | boolean, allowDuplicates?: boolean): number {
    let ks: ((obj: T) => V) | undefined;

    if (isFunction(keySelector)) {
        ks = keySelector;
    }
    else if (isBoolean(keySelector)) {
        allowDuplicates = keySelector;
    }

    if (isNullOrUndefined(allowDuplicates)) {
        allowDuplicates = true;
    }

    return arraySortedInsertInternal<T, V>(arr, item, ks, allowDuplicates);
}

function arraySortedInsertInternal<T, V>(arr: T[], item: T, ks: ((obj: T) => V) | undefined, allowDuplicates: boolean) {
    let idx = arrayBinarySearch(arr, item, ks);
    const found = (idx % 1) === 0;
    idx = idx | 0;
    if (!found || allowDuplicates) {
        arrayInsertAt(arr, item, idx);
    }

    return idx;
}

/**
 * Creates a new array that is sorted by the key extracted
 * by the keySelector callback, not modifying the input array,
 * (unlike JavaScript's own Array.prototype.sort).
 * @param arr
 * @param keySelector
 */
export function arraySortByKey<T, V>(arr: ReadonlyArray<T>, keySelector: (obj: T) => V): T[] {
    const newArr = Array.from(arr);
    arraySortByKeyInPlace<T, V>(newArr, keySelector);
    return newArr;
}

/**
 * Sorts an existing array by the key extracted by the keySelector
 * callback, without creating a new array.
 * @param arr
 * @param keySelector
 */
export function arraySortByKeyInPlace<T, V>(newArr: T[], keySelector: (obj: T) => V) {
    newArr.sort((a, b) => {
        const keyA = keySelector(a);
        const keyB = keySelector(b);
        if (keyA < keyB) {
            return -1;
        }
        else if (keyA > keyB) {
            return 1;
        }
        else {
            return 0;
        }
    });
}

const numericPattern = /^(\d+)/;
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
    const newArr = Array.from(arr);
    newArr.sort((a, b) => {
        const keyA = keySelector(a);
        const keyB = keySelector(b);
        const matchA = keyA.match(numericPattern);
        const matchB = keyB.match(numericPattern);
        if (isDefined(matchA)
            && isDefined(matchB)) {
            const numberA = parseFloat(matchA[1]);
            const numberB = parseFloat(matchB[1]);

            if (numberA < numberB) {
                return -1;
            }
            else if (numberA > numberB) {
                return 1;
            }
        }

        if (keyA < keyB) {
            return -1;
        }
        else if (keyA > keyB) {
            return 1;
        }
        else {
            return 0;
        }
    });

    return newArr;
}

export function arrayZip<T, V>(arr1: readonly T[], arr2: readonly T[], combine: (a: T, b: T) => V): V[] {
    const len = Math.max(arr1.length, arr2.length);
    const output = new Array<V>(len);
    for (let i = 0; i < len; ++i) {
        output[i] = combine(arr1[i], arr2[i]);
    }

    return output;
}
