import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
export function mapJoin(dest, ...sources) {
    for (const source of sources) {
        if (isDefined(source)) {
            for (const [key, value] of source) {
                dest.set(key, value);
            }
        }
    }
    return dest;
}
//# sourceMappingURL=mapJoin.js.map