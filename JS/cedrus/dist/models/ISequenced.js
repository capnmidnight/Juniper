import { isNumber, isObject } from "@juniper-lib/util";
export function isISequenced(obj) {
    return isObject(obj)
        && "id" in obj
        && isNumber(obj.id);
}
//# sourceMappingURL=ISequenced.js.map