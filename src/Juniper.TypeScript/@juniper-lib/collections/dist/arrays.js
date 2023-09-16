import { isDefined, isFunction, isNullOrUndefined, isNumber, isObject, isString } from "@juniper-lib/tslib/dist/typeChecks";
function isIComparable(obj) {
    return isObject(obj)
        && "compareTo" in obj
        && isFunction(obj.compareTo);
}
export function compareBy(directionOrFirstKeyGetter, ...getKeys) {
    let direction = null;
    if (isString(directionOrFirstKeyGetter)) {
        direction = directionOrFirstKeyGetter;
    }
    else {
        direction = "ascending";
        getKeys.unshift(directionOrFirstKeyGetter);
    }
    const d = direction === "ascending" ? 1 : -1;
    const comparer = (a, b) => {
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
export function binarySearch(arr, searchValue, comparer, mode = "search") {
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
export function insertSorted(arr, val, comparerOrIdx, mode = "search") {
    let idx = null;
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
export function removeSorted(arr, val, comparer) {
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
export function arrayClear(arr) {
    return arr.splice(0);
}
/**
 * Checks to see if two arrays contain the same elements
 * @returns -1 if the arrays match, the index of the first mismatched item if they don't.
 * @param arr1
 * @param arr2
 */
export function arrayCompare(arr1, arr2) {
    for (let i = 0; i < arr1.length; ++i) {
        if (arr1[i] !== arr2[i]) {
            return i;
        }
    }
    return -1;
}
export function arrayGen(count, thunk) {
    return Array.from(iterableGen(count, thunk));
}
export function* iterableGen(count, thunk) {
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
export function arrayInsertAt(arr, item, idx) {
    arr.splice(idx, 0, item);
}
/**
 * Returns a random item from an array of items.
 *
 * Provides an option to consider an additional item as part of the collection
 * for random selection.
 */
export function arrayRandom(arr, defaultValue) {
    const offset = defaultValue != null ? 1 : 0, idx = Math.floor(Math.random() * (arr.length + offset)) - offset;
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
export function arrayRemove(arr, value) {
    const idx = arr.indexOf(value);
    if (idx > -1) {
        arrayRemoveAt(arr, idx);
        return true;
    }
    return false;
}
export function arrayFilter(arr, predicate) {
    for (let i = arr.length - 1; i >= 0; --i) {
        if (predicate(arr[i])) {
            return arrayRemoveAt(arr, i);
        }
    }
    return null;
}
export function arrayRemoveByKey(arr, key, getKey) {
    return arrayFilter(arr, (v) => getKey(v) === key);
}
/**
 * Removes an item at the given index from an array.
 */
export function arrayRemoveAt(arr, idx) {
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
export function arrayReplace(arr, ...items) {
    arr.splice(0, arr.length, ...items);
}
export function arrayCreate(count, make) {
    const arr = new Array(count);
    for (let i = 0; i < count; ++i) {
        arr[i] = make(i, count);
    }
    return arr;
}
function _arrayScan(forward, arr, tests) {
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
export function arrayScan(arr, ...tests) {
    return _arrayScan(true, arr, tests);
}
export function arrayScanReverse(arr, ...tests) {
    return _arrayScan(false, arr, tests);
}
export function arrayShuffleInplace(arr) {
    for (let i = 0; i < arr.length - 1; ++i) {
        const subLength = arr.length - i;
        const subIndex = Math.floor(Math.random() * subLength);
        const temp = arr[i];
        const j = subIndex + i;
        arr[i] = arr[j];
        arr[j] = temp;
    }
}
export function arrayShuffle(arr) {
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
export function arraySortNumericByKey(arr, keySelector) {
    const comparer = compareBy(v => {
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
export function arrayZip(arr1, arr2, combine) {
    const len = Math.max(arr1.length, arr2.length);
    const output = new Array(len);
    for (let i = 0; i < len; ++i) {
        output[i] = combine(arr1[i], arr2[i]);
    }
    return output;
}
//# sourceMappingURL=arrays.js.map