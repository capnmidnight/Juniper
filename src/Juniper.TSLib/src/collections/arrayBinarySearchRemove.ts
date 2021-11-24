import { arrayBinarySearch, arrayBinarySearchByKey, arrayRemoveAt } from "../";

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
