import { isDate, isNullOrUndefined, isObject, isString } from "@juniper-lib/util";
import { ENTITY_TYPE, EntityTypeModel, isEntityTypeModel } from "./EntityTypeModel";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { INamed, isINamed } from "./INamed";
import { isTypeStamped, ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE } from "./PropertyTypeModel";
import { isUserModel, UserModel } from "./UserModel";

export interface ReviewParts {
    reviewedBy: UserModel;
    reviewedOn: Date;
}

export interface EntityModel extends INamed, ITypeStamped<"entity">, ICreationTracked, ReviewParts {
    link: string;
    type: EntityTypeModel;
}

export function isEntityModel(obj: unknown): obj is EntityModel {
    return isTypeStamped("entity", obj)
        && isINamed(obj)
        && isICreationTracked(obj)
        && "link" in obj
        && isString(obj.link)
        && "type" in obj
        && isEntityTypeModel(obj.type)
        && "reviewedBy" in obj
        && (
            isUserModel(obj.reviewedBy)
            || isObject(obj.reviewedBy)
                && "id" in obj.reviewedBy
                && isNullOrUndefined(obj.reviewedBy.id)
        )
        && "reviewedOn" in obj
        && (isNullOrUndefined(obj.reviewedOn)
            || isDate(obj.reviewedOn));
}

export interface SetEntityInput {
    type: ENTITY_TYPE;
    name: string;
}

export interface EntitySearchInput {
    entityTypes: ENTITY_TYPE[];
    entityName: string;
    propertyType: PROPERTY_TYPE;
    propertyValue: boolean | boolean[] | string | string[] | Date | Date[] | number | number[];
}

export type ENTITY = TypedNameOrId<"entity">;
export function ENTITY(idOrName: number | string): ENTITY {
    return TypedNameOrId(idOrName, "entity");
}