import { isArray } from "@juniper-lib/util";
import { EntityTypeModel, isEntityTypeModel } from "./EntityTypeModel";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE, PropertyTypeModel } from "./PropertyTypeModel";

export interface PropertyTemplateModel extends INamed, ITypeStamped<"propertyTemplate"> {
    entityType: EntityTypeModel;
    propertyTypes: PropertyTypeModel[];
}

export function isPropertyTemplateModel(obj: unknown): obj is PropertyTemplateModel {
    return isTypeStamped("propertyTemplate", obj)
        && isINamed(obj)
        && "entityType" in obj
        && isEntityTypeModel(obj.entityType)
        && "propertyTypes" in obj
        && isArray(obj.propertyTypes);
}

export interface SetPropertyTemplateInput {
    name: string;
    propertyTypes?: PROPERTY_TYPE[];
}

export type PROPERTY_TEMPLATE = TypedNameOrId<"propertyTemplate">;
export function PROPERTY_TEMPLATE(idOrName: number | string): PROPERTY_TEMPLATE {
    return TypedNameOrId(idOrName, "propertyTemplate");
}