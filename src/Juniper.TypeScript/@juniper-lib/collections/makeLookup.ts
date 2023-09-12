import { identity } from "@juniper-lib/tslib/identity";
import { mapMap } from "./mapMap";

export function makeLookup<T, U>(items: readonly T[], makeID: (item: T) => U): Map<U, T> {
    return mapMap(items, makeID, identity);
}