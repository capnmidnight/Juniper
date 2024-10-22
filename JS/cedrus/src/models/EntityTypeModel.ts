import { isBoolean } from "@juniper-lib/util";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";

export interface EntityTypeModel extends INamed, ITypeStamped<"entityType"> {
    isPrimary: boolean;
    parent?: EntityTypeModel;
}

export function isEntityTypeModel(obj: unknown): obj is EntityTypeModel {
    return isTypeStamped("entityType", obj)
        && isINamed(obj)
        && "isPrimary" in obj
        && isBoolean(obj.isPrimary);
}

export interface SetEntityTypeInput {
    name: string;
    isPrimary: boolean;
    parentEntityType?: ENTITY_TYPE;
}

export type ENTITY_TYPE = TypedNameOrId<"entityType">;
export function ENTITY_TYPE(idOrName: number | string): ENTITY_TYPE {
    return TypedNameOrId(idOrName, "entityType");
}