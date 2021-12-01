export function arrayZip(arr1, arr2, combine) {
    const len = Math.max(arr1.length, arr2.length);
    const output = new Array(len);
    for (let i = 0; i < len; ++i) {
        output[i] = combine(arr1[i], arr2[i]);
    }
    return output;
}
