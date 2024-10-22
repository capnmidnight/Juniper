import { identity } from "@juniper-lib/tslib/dist/identity";
import { mapMap } from "./mapMap";
import { isNullOrUndefined } from "@juniper-lib/tslib/src/typeChecks";

export function makeLookup<T, U>(items: readonly T[], makeID: (item: T, i?: number) => U): Map<U, T> {
    return mapMap(items, makeID, identity);
}

export function makeReverseLookup<T>(items: readonly T[]): Map<T, number>
export function makeReverseLookup<T, U>(items: readonly T[], makeID: (item: T, i?: number) => U): Map<T, U>;
export function makeReverseLookup<T>(items: readonly T[], makeID?: (item: T, i?: number) => any): Map<T, any> {
    if(isNullOrUndefined(makeID)) {
        makeID = (_, i) => i;
    }
    return mapMap(items, identity, makeID);
}