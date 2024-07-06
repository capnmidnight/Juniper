import { isString } from "@juniper-lib/util";
import { ClassificationLevelModel, isClassificationLevelModel } from "./ClassificationLevelModel";
import { IDOrName } from "./IDOrName";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface ClassificationCaveatModel extends INamed, ITypeStamped<"classificationCaveat"> {
    description: string;
    level: ClassificationLevelModel;
}

export function isClassificationCaveatModel(obj: unknown): obj is ClassificationCaveatModel {
    return isTypeStamped("classificationCaveat", obj)
        && isINamed(obj)
        && "description" in obj
        && isString(obj.description)
        && "level" in obj
        && isClassificationLevelModel(obj.level);
}

export type SetClassificationCaveatInput = {
    classificationLevel: IDOrName;
    name: string;
    description: string;
}