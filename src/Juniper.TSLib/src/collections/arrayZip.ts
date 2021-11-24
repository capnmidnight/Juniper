export function arrayZip<T, V>(arr1: readonly T[], arr2: readonly T[], combine: (a: T, b: T) => V): V[] {
    const len = Math.max(arr1.length, arr2.length);
    const output = new Array<V>(len);
    for (let i = 0; i < len; ++i) {
        output[i] = combine(arr1[i], arr2[i]);
    }

    return output;
}
