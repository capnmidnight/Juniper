import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
export function mapGroupBy(items, makeID) {
    const map = new Map();
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
//# sourceMappingURL=mapGroupBy.js.map