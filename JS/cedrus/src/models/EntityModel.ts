import { isString } from "@juniper-lib/util";
import { EntityTypeModel, isEntityTypeModel } from "./EntityTypeModel";
import { IClassificationMarked, isIClassificationMarked } from "./IClassificationMarked";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { IDOrName } from "./IDOrName";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { PropertyModel } from "./PropertyModel";

type PropertyTypeMap = {
    [name: string]: PropertyModel;
}

export interface EntityModel<PropertyTypeMapT extends PropertyTypeMap = PropertyTypeMap> extends INamed, ITypeStamped<"entity">, ICreationTracked, IClassificationMarked {
    link: string;
    type: EntityTypeModel;
    properties: PropertyTypeMapT;
}

export function isEntityModel(obj: unknown): obj is EntityModel {
    return isTypeStamped("entity", obj)
        && isINamed(obj)
        && isICreationTracked(obj)
        && isIClassificationMarked(obj)
        && "link" in obj
        && isString(obj.link)
        && "type" in obj
        && isEntityTypeModel(obj.type);
}

export type SetEntityInput = {
    type: IDOrName;
    name: string;
    classification?: IDOrName;
}
