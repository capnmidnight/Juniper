import { isString } from "@juniper-lib/util";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface RelationshipTypeModel extends INamed, ITypeStamped<"relationshipType"> {
    parentRole: string;
    childRole: string;
}

export function isRelationshipTypeModel(obj: unknown): obj is RelationshipTypeModel {
    return isTypeStamped("relationshipType", obj)
        && isINamed(obj)
        && "parentRole" in obj
        && isString(obj.parentRole)
        && "childRole" in obj
        && isString(obj.childRole);
}