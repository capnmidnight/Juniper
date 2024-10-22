import { EntityTypeModel } from "./EntityTypeModel";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE, PropertyTypeModel } from "./PropertyTypeModel";
export interface PropertyTemplateModel extends INamed, ITypeStamped<"propertyTemplate"> {
    entityType: EntityTypeModel;
    propertyTypes: PropertyTypeModel[];
}
export declare function isPropertyTemplateModel(obj: unknown): obj is PropertyTemplateModel;
export interface SetPropertyTemplateInput {
    name: string;
    propertyTypes?: PROPERTY_TYPE[];
}
export type PROPERTY_TEMPLATE = TypedNameOrId<"propertyTemplate">;
export declare function PROPERTY_TEMPLATE(idOrName: number | string): PROPERTY_TEMPLATE;
//# sourceMappingURL=PropertyTemplateModel.d.ts.map