import { isDate } from "@juniper-lib/util";
import { INamed } from "./INamed";
import { ISequenced, isISequenced } from "./ISequenced";

export interface ITimeSeries<TypeT extends INamed> extends ISequenced {
    type: TypeT;
    startDate: Date;
    endDate: Date;
}

export function isITimeSeries<TypeT extends INamed>(obj: unknown): obj is ITimeSeries<TypeT> {
    return isISequenced(obj)
        && "type" in obj
        && "startDate" in obj
        && isDate(obj.startDate)
        && "endDate" in obj
        && isDate(obj.endDate);
}