import { isArray } from "@juniper-lib/util";
import { ClassificationCaveatModel, isClassificationCaveatModel } from "./ClassificationCaveatModel";
import { ClassificationLevelModel, isClassificationLevelModel } from "./ClassificationLevelModel";
import { IDOrName } from "./IDOrName";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface ClassificationModel extends INamed, ITypeStamped<"classification"> {
    description: string;
    level: ClassificationLevelModel;
    caveats: ClassificationCaveatModel[];
}

export function isClassificationModel(obj: unknown): obj is ClassificationModel {
    return isTypeStamped("classification", obj)
        && isINamed(obj)
        && "level" in obj
        && isClassificationLevelModel(obj.level)
        && "caveats" in obj
        && isArray(obj.caveats)
        && obj.caveats.every(isClassificationCaveatModel);
}

export interface SetClassificationInput {
    level: IDOrName;
    caveats: IDOrName[];
}