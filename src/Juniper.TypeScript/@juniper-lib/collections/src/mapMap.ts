export function mapMap<T, U, V>(items: readonly T[], makeID: (item: T) => U, makeValue: (item: T) => V) {
    return new Map(items.map((item) => [makeID(item), makeValue(item)]));
}