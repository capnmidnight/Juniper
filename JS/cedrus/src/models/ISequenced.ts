import { isNumber, isObject } from "@juniper-lib/util";

export interface ISequenced {
    id: number;
}

export function isISequenced(obj: unknown): obj is ISequenced {
    return isObject(obj)
        && "id" in obj
        && isNumber(obj.id);
}