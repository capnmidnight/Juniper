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