import { isDefined, isFunction } from "../";
export function isProgressCallback(obj) {
    return isDefined(obj)
        && isFunction(obj.report)
        && isFunction(obj.attach)
        && isFunction(obj.end);
}
