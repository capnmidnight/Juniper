export function distance_WagnerFischer_Damerau_Levenshtein(a: string, b: string): number {
    if (!a) {
        throw new Error(`Argument ${a} is null`);
    }

    if (!b) {
        throw new Error(`Argument ${b} is null`);
    }

    const m = a.length + 1;
    const n = b.length + 1;
    const matrix = new Array<Array<number>>(m);
    for (let i = 0; i < m; ++i) {
        matrix[i] = new Array<number>(n);
        for (let j = 0; j < m; ++j) {
            if (j === 0) {
                matrix[i][j] = i;
            }
            else if (i === 0) {
                matrix[i][j] = j;
            }
            else {
                matrix[i][j] = 0;
            }
        }
    }

    for (let j = 1; j < n; ++j) {
        const y = b[j - 1];
        for (let i = 1; i < m; ++i) {
            const x = a[i - 1];
            const deleteCost = matrix[i - 1][j] + 1;
            const insertCost = matrix[i][j - 1] + 1;
            const subCost = (x == y ? 0 : 1);
            const substitutionCost = matrix[i - 1][j - 1] + subCost;
            matrix[i][j] = Math.min(Math.min(deleteCost, insertCost), substitutionCost);
            if (j > 1 && i > 1 && a[i - 2] == y && b[j - 2] == x) {
                const transpositionCost = matrix[i - 2][j - 2] + subCost;
                matrix[i][j] = Math.min(matrix[i][j], transpositionCost);
            }
        }
    }

    return matrix[a.length][b.length];
}

export function similarity_WagnerFischerDamerauLevenshtein(a: string, b: string): number {
    if (!a) {
        throw new Error(`Argument ${a} is null`);
    }

    if (!b) {
        throw new Error(`Argument ${b} is null`);
    }

    if (a.length < b.length) {
        const c = a;
        a = b;
        b = c;
    }

    const distance = distance_WagnerFischer_Damerau_Levenshtein(a, b);
    let prop = 1;
    if (a.length > 0) {
        prop = distance / a.length;
    }
    else if (b.length == 0) {
        prop = 0;
    }

    return 1 - prop;
}

/// <summary>
/// The Winkler modification will not be applied unless the
/// percent match was at or above the mWeightThreshold percent
/// without the modification. Winkler's paper used a default
/// value of 0.7
/// </summary>
const WinklerThreshold = 0.7;

/// <summary>
/// Size of the prefix to be considered by the Winkler
/// modification. Winkler's paper used a default value of 4.
/// </summary>
const WinklerPrefixSize = 4;

/// <summary>
/// Returns the Jaro-Winkler distance between the specified
/// strings. The distance is symmetric and will fall in the
/// range 0 (no match) to 1 (perfect match).
/// </summary>
/// <param name="a">First String</param>
/// <param name="b">Second String</param>
/// <returns></returns>
export function similarity_JaroWinkler(a: string, b: string): number {
    if (!a) {
        throw new Error(`Argument ${a} is null`);
    }

    if (!b) {
        throw new Error(`Argument ${b} is null`);
    }

    if (a.length == 0) {
        return b.length == 0 ? 1 : 0;
    }

    const searchRange = Math.max(0, (Math.max(a.length, b.length) / 2) - 1);

    const matchesA = new Array<boolean>(a.length);
    const matchesB = new Array<boolean>(b.length);

    let numCommon = 0;
    for (let i = 0; i < a.length; ++i) {
        const start = Math.max(0, i - searchRange);
        const end = Math.min(i + searchRange + 1, b.length);
        for (let j = start; j < end; ++j) {
            if (!matchesB[j] && a[i] == b[j]) {
                matchesA[i] = true;
                matchesB[j] = true;
                ++numCommon;
                break;
            }
        }
    }

    if (numCommon == 0) {
        return 0;
    }

    let numTransposed = 0;
    let k = 0;
    for (let i = 0; i < a.length; ++i) {
        if (matchesA[i]) {
            while (!matchesB[k]) {
                ++k;
            }

            if (a[i] != b[k]) {
                ++numTransposed;
            }

            ++k;
        }
    }

    const halfNumTransposed = numTransposed / 2;

    const numCommonFloat = numCommon;
    const weight = ((numCommonFloat / a.length)
        + (numCommonFloat / b.length)
        + ((numCommon - halfNumTransposed) / numCommonFloat)) / 3;

    if (weight <= WinklerThreshold) {
        return weight;
    }

    const prefixLength = Math.min(WinklerPrefixSize, Math.min(a.length, b.length));
    let position = 0;
    while (position < prefixLength
        && a[position] == b[position]) {
        ++position;
    }

    if (position == 0) {
        return weight;
    }

    return weight + (0.1 * position * (1 - weight));
}