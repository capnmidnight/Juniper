import { isNullOrUndefined } from "../";

export function mapGroupBy<T, U>(items: T[], makeID: (item: T) => U) {
    const map = new Map<U, T[]>();
    for (const item of items) {
        const id = makeID(item);
        let group = map.get(id);
        if (isNullOrUndefined(group)) {
            map.set(id, group = []);
        }
        group.push(item);
    }

    return map;
}

