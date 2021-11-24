/**
 * Empties out an array, returning the items that were in the array.
 * 
 * @param arr the array to empty
 */
export function arrayClear<T>(arr: T[]) {
    return arr.splice(0);
}