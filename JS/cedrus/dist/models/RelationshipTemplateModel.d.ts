import { ENTITY_TYPE, EntityTypeModel } from "./EntityTypeModel";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { RELATIONSHIP_TYPE, RelationshipTypeModel } from "./RelationshipTypeModel";
export interface RelationshipTemplateModel extends INamed, ITypeStamped<"relationshipTemplate"> {
    entityType: EntityTypeModel;
    relationshipType: RelationshipTypeModel;
    propertyEntityType?: EntityTypeModel;
    allowedEntityTypes: EntityTypeModel[];
}
export declare function isRelationshipTemplateModel(obj: unknown): obj is RelationshipTemplateModel;
export interface SetRelationshipTemplateInput {
    name: string;
    relationshipType: RELATIONSHIP_TYPE;
    propertyEntityType?: ENTITY_TYPE;
    allowedEntityTypes?: ENTITY_TYPE[];
}
export type RELATIONSHIP_TEMPLATE = TypedNameOrId<"relationshipTemplate">;
export declare function RELATIONSHIP_TEMPLATE(idOrName: number | string): RELATIONSHIP_TEMPLATE;
//# sourceMappingURL=RelationshipTemplateModel.d.ts.map