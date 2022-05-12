export function mapInvert<T, U>(map: Map<T, U>): Map<U, T> {
    const mapOut = new Map<U, T>();
    for (const [key, value] of map) {
        mapOut.set(value, key);
    }
    return mapOut;
}

