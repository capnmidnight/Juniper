import { mapMap } from "./mapMap";

export function mapBuild<T, U>(items: T[], makeValue: (item: T) => U): Map<T, U> {
    return mapMap(items, i => i, makeValue);
}
