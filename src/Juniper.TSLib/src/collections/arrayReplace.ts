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
