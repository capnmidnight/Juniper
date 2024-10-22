import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export interface RelationshipTypeModel extends INamed, ITypeStamped<"relationshipType"> {
    parentRole: string;
    childRole: string;
}
export declare function isRelationshipTypeModel(obj: unknown): obj is RelationshipTypeModel;
export type RELATIONSHIP_TYPE = TypedNameOrId<"relationshipType">;
export declare function RELATIONSHIP_TYPE(idOrName: number | string): RELATIONSHIP_TYPE;
//# sourceMappingURL=RelationshipTypeModel.d.ts.map