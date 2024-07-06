import { isArray } from "@juniper-lib/util";
import { EntityTypeModel, isEntityTypeModel } from "./EntityTypeModel";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { PropertyTypeModel } from "./PropertyTypeModel";

export interface TemplateModel extends INamed, ITypeStamped<"template"> {
    entityType: EntityTypeModel;
    propertyTypes: PropertyTypeModel[];
}

export function isTemplateModel(obj: unknown): obj is TemplateModel {
    return isTypeStamped("template", obj)
        && isINamed(obj)
        && "entityType" in obj
        && isEntityTypeModel(obj.entityType)
        && "propertyTypes" in obj
        && isArray(obj.propertyTypes);
}