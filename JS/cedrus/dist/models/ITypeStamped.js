import { isObject, isString } from "@juniper-lib/util";
export function isTypeStamped(key, obj) {
    return isObject(obj)
        && "typeStamp" in obj
        && isString(obj.typeStamp)
        && obj.typeStamp === key;
}
//# sourceMappingURL=ITypeStamped.js.map