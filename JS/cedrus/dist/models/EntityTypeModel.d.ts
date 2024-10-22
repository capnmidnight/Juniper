import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export interface EntityTypeModel extends INamed, ITypeStamped<"entityType"> {
    isPrimary: boolean;
    parent?: EntityTypeModel;
}
export declare function isEntityTypeModel(obj: unknown): obj is EntityTypeModel;
export interface SetEntityTypeInput {
    name: string;
    isPrimary: boolean;
    parentEntityType?: ENTITY_TYPE;
}
export type ENTITY_TYPE = TypedNameOrId<"entityType">;
export declare function ENTITY_TYPE(idOrName: number | string): ENTITY_TYPE;
//# sourceMappingURL=EntityTypeModel.d.ts.map