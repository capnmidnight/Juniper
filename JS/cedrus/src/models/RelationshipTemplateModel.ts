import { isArray } from "@juniper-lib/util";
import { ENTITY_TYPE, EntityTypeModel, isEntityTypeModel } from "./EntityTypeModel";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { RELATIONSHIP_TYPE, RelationshipTypeModel, isRelationshipTypeModel } from "./RelationshipTypeModel";

export interface RelationshipTemplateModel extends INamed, ITypeStamped<"relationshipTemplate"> {
    entityType: EntityTypeModel;
    relationshipType: RelationshipTypeModel;
    propertyEntityType?: EntityTypeModel;
    allowedEntityTypes: EntityTypeModel[];
}

export function isRelationshipTemplateModel(obj: unknown): obj is RelationshipTemplateModel {
    return isTypeStamped("relationshipTemplate", obj)
        && isINamed(obj)
        && "entityType" in obj
        && isEntityTypeModel(obj.entityType)
        && "relationshipType" in obj
        && isRelationshipTypeModel(obj.relationshipType)
        && "entityTypes" in obj
        && isArray(obj.entityTypes)
        && obj.entityTypes.every(isEntityTypeModel);
}

export interface SetRelationshipTemplateInput {
    name: string;
    relationshipType: RELATIONSHIP_TYPE;
    propertyEntityType?: ENTITY_TYPE;
    allowedEntityTypes?: ENTITY_TYPE[];
}

export type RELATIONSHIP_TEMPLATE = TypedNameOrId<"relationshipTemplate">;
export function RELATIONSHIP_TEMPLATE(idOrName: number | string): RELATIONSHIP_TEMPLATE {
    return TypedNameOrId(idOrName, "relationshipTemplate");
}
