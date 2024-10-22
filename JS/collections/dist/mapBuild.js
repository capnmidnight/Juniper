import { identity } from "@juniper-lib/tslib/dist/identity";
import { mapMap } from "./mapMap";
export function mapBuild(items, makeValue) {
    return mapMap(items, identity, makeValue);
}
//# sourceMappingURL=mapBuild.js.map