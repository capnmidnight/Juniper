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
