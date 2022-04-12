import { arrayRemoveAt } from "./arrayRemoveAt";

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

export function arrayFilter<T>(arr: T[], predicate: (v: T) => boolean): T {
    for (let i = arr.length - 1; i >= 0; --i) {
        if (predicate(arr[i])) {
            return arrayRemoveAt(arr, i);
        }
    }

    return null;
}

export function arrayRemoveByKey<T, K>(arr: T[], key: K, getKey: (v: T) => K): T {
    return arrayFilter(arr, (v) => getKey(v) === key);
}