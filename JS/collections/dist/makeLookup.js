import { identity } from "@juniper-lib/tslib/dist/identity";
import { mapMap } from "./mapMap";
import { isNullOrUndefined } from "@juniper-lib/tslib/src/typeChecks";
export function makeLookup(items, makeID) {
    return mapMap(items, makeID, identity);
}
export function makeReverseLookup(items, makeID) {
    if (isNullOrUndefined(makeID)) {
        makeID = (_, i) => i;
    }
    return mapMap(items, identity, makeID);
}
//# sourceMappingURL=makeLookup.js.map