import { isDefined, isFunction } from "@juniper-lib/tslib/typeChecks";
export function isProgressCallback(obj) {
    return isDefined(obj)
        && isFunction(obj.report)
        && isFunction(obj.attach)
        && isFunction(obj.end);
}
//# sourceMappingURL=IProgress.js.map