export function mapMap<T, U, V>(items: readonly T[], makeID: (item: T, i?: number) => U, makeValue: (item: T, i?: number) => V) {
    return new Map(items.map((item, i) => [
        makeID(item, i), 
        makeValue(item, i)
    ]));
}