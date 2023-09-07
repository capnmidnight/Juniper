import { identity } from "@juniper-lib/tslib/identity";
import { mapMap } from "./mapMap";
export function mapBuild(items, makeValue) {
    return mapMap(items, identity, makeValue);
}
//# sourceMappingURL=mapBuild.js.map