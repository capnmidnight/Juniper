import { isArray, isNumber, isObject, isString } from "@juniper-lib/util";
import { INamed, isINamed } from "./INamed";

export interface FlatRelationshipModel extends INamed {
}

export function isFlatRelationshipModel(obj: unknown): obj is FlatRelationshipModel {
    return isINamed(obj);
}

export interface FlatEntityModel extends INamed {
    parentId?: number;
    typeId: number;
    typeName: string;
    parents: FlatRelationshipModel[];
    children: FlatRelationshipModel[];
    properties: number[];
}

export function isFlatEntityModel(obj: unknown) {
    return isObject(obj)
        && isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "typeName" in obj
        && isString(obj.typeName)
        && "parents" in obj
        && isArray(obj.parents)
        && obj.parents.every(isFlatRelationshipModel)
        && "children" in obj
        && isArray(obj.children)
        && obj.children.every(isFlatRelationshipModel)
        && "properties" in obj
        && isArray(obj.properties)
        && obj.properties.every(isNumber);
}