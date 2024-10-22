import { ENTITY, EntityModel, isEntityModel } from "./EntityModel";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { ISequenced, isISequenced } from "./ISequenced";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { RELATIONSHIP_TYPE, RelationshipTypeModel, isRelationshipTypeModel } from "./RelationshipTypeModel";

export interface RelationshipModel extends ISequenced, ITypeStamped<"relationship">, ICreationTracked {
    type: RelationshipTypeModel;
    parent: EntityModel;
    child: EntityModel;
    propertyEntity?: EntityModel;
}

export function isRelationshipModel(obj: unknown): obj is RelationshipModel {
    return isTypeStamped("relationship", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && "type" in obj
        && isRelationshipTypeModel(obj.type)
        && "parent" in obj
        && isEntityModel(obj.parent)
        && "child" in obj
        && isEntityModel(obj.child);
}

export type SetRelationshipInput = {
    type?: RELATIONSHIP_TYPE;
    childEntity: ENTITY;
    propertyEntity?: ENTITY;
}

export type GetRelationshipInput = {
    expandGraph?: boolean;
    parent?: TypedNameOrId<"entity" | "entityType">[];
    child?: TypedNameOrId<"entity" | "entityType">[];
    both?: TypedNameOrId<"entity" | "entityType" | "relationshipType" | "relationship">[];
}

export type RELATIONSHIP = TypedNameOrId<"relationship">;
export function RELATIONSHIP(id: number): RELATIONSHIP {
    return TypedNameOrId(id, "relationship");
}
