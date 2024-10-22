import { ENTITY, EntityModel } from "./EntityModel";
import { ICreationTracked } from "./ICreationTracked";
import { ISequenced } from "./ISequenced";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { RELATIONSHIP_TYPE, RelationshipTypeModel } from "./RelationshipTypeModel";
export interface RelationshipModel extends ISequenced, ITypeStamped<"relationship">, ICreationTracked {
    type: RelationshipTypeModel;
    parent: EntityModel;
    child: EntityModel;
    propertyEntity?: EntityModel;
}
export declare function isRelationshipModel(obj: unknown): obj is RelationshipModel;
export type SetRelationshipInput = {
    type?: RELATIONSHIP_TYPE;
    childEntity: ENTITY;
    propertyEntity?: ENTITY;
};
export type GetRelationshipInput = {
    expandGraph?: boolean;
    parent?: TypedNameOrId<"entity" | "entityType">[];
    child?: TypedNameOrId<"entity" | "entityType">[];
    both?: TypedNameOrId<"entity" | "entityType" | "relationshipType" | "relationship">[];
};
export type RELATIONSHIP = TypedNameOrId<"relationship">;
export declare function RELATIONSHIP(id: number): RELATIONSHIP;
//# sourceMappingURL=RelationshipModel.d.ts.map