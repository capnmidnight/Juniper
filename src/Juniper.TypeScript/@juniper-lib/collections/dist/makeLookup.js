import { identity } from "@juniper-lib/tslib/dist/identity";
import { mapMap } from "./mapMap";
export function makeLookup(items, makeID) {
    return mapMap(items, makeID, identity);
}
//# sourceMappingURL=makeLookup.js.map