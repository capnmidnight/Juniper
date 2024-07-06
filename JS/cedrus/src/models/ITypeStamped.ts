import { isObject, isString } from "@juniper-lib/util";

export interface ITypeStamped<TypeStamp extends string> {
    typeStamp: TypeStamp;
}

export function isTypeStamped<T extends string>(key: T, obj: unknown): obj is ITypeStamped<T> {
    return isObject(obj)
        && "typeStamp" in obj
        && isString(obj.typeStamp)
        && obj.typeStamp === key;
}