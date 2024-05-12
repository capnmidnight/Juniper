import { isObject } from "@juniper-lib/util";
import { ClassificationModel, isClassificationModel } from "./ClassificationModel";
import { ISequenced, isISequenced } from "./ISequenced";

export interface IClassificationMarked extends ISequenced {
    classification: ClassificationModel;
}

export function isIClassificationMarked(obj: unknown): obj is IClassificationMarked {
    return isObject(obj)
        && isISequenced(obj)
        && "classification" in obj
        && isClassificationModel(obj.classification);
}