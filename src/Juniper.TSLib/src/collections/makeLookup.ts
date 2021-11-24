import { mapMap } from "../";

export function makeLookup<T, U>(items: T[], makeID: (item: T) => U): Map<U, T> {
    return mapMap(items, makeID, i => i);
}