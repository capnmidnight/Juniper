import { isString } from "@juniper-lib/util";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface ClassificationLevelModel extends INamed, ITypeStamped<"classificationLevel"> {
    description: string;
}

export function isClassificationLevelModel(obj: unknown): obj is ClassificationLevelModel {
    return isTypeStamped("classificationLevel", obj)
        && isINamed(obj)
        && "description" in obj
        && isString(obj.description);
}