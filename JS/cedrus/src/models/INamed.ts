import { isString } from "@juniper-lib/util";
import { ISequenced, isISequenced } from "./ISequenced";

export interface INamed extends ISequenced {
    name: string;
}

export function isINamed(obj: unknown): obj is INamed {
    return isISequenced(obj)
        && "name" in obj
        && isString(obj.name);
}

export const getName: (v: INamed) => string = v => v?.name;