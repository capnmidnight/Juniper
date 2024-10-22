import { isDefined, isFunction } from "@juniper-lib/util";
export function isProgressCallback(obj) {
    return isDefined(obj)
        && isFunction(obj.report)
        && isFunction(obj.attach)
        && isFunction(obj.end);
}
//# sourceMappingURL=IProgress.js.map