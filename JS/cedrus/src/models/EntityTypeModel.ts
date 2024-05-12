import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface EntityTypeModel extends INamed, ITypeStamped<"entityType"> {
}

export function isEntityTypeModel(obj: unknown): obj is EntityTypeModel {
    return isTypeStamped("entityType", obj)
        && isINamed(obj);
}
