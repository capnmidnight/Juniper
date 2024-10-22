import { ENTITY_TYPE, EntityTypeModel } from "./EntityTypeModel";
import { ICreationTracked } from "./ICreationTracked";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE } from "./PropertyTypeModel";
import { UserModel } from "./UserModel";
export interface ReviewParts {
    reviewedBy: UserModel;
    reviewedOn: Date;
}
export interface EntityModel extends INamed, ITypeStamped<"entity">, ICreationTracked, ReviewParts {
    link: string;
    type: EntityTypeModel;
}
export declare function isEntityModel(obj: unknown): obj is EntityModel;
export interface SetEntityInput {
    type: ENTITY_TYPE;
    name: string;
}
export interface EntitySearchInput {
    entityTypes: ENTITY_TYPE[];
    entityName: string;
    propertyType: PROPERTY_TYPE;
    propertyValue: boolean | boolean[] | string | string[] | Date | Date[] | number | number[];
}
export type ENTITY = TypedNameOrId<"entity">;
export declare function ENTITY(idOrName: number | string): ENTITY;
//# sourceMappingURL=EntityModel.d.ts.map