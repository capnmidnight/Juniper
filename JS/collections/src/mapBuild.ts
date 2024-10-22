import { identity } from "@juniper-lib/tslib/dist/identity";
import { mapMap } from "./mapMap";

export function mapBuild<T, U>(items: readonly T[], makeValue: (item: T) => U): Map<T, U> {
    return mapMap(items, identity, makeValue);
}

