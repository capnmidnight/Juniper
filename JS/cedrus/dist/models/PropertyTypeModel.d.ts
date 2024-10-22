import { DataType, StorageType } from "./DataType";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export interface PropertyTypeModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends INamed, ITypeStamped<"propertyType"> {
    type: DataTypeT;
    storage: StorageTypeT;
    unitsCategory: string;
    description: string;
    ReferencePath?: string;
}
export declare function isPropertyTypeModel(obj: unknown): obj is PropertyTypeModel<DataType, StorageType>;
export declare function isPropertyTypeModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeModel<DataTypeT, StorageType>;
export declare function isPropertyTypeModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyTypeModel<DataType, StorageTypeT>;
export declare function isPropertyTypeModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyTypeModel<DataTypeT, StorageTypeT>;
export interface SetPropertyTypeInput {
    type: DataType;
    storage: StorageType;
    name: string;
    category: string;
    description: string;
    ReferencePath?: string;
}
export type PROPERTY_TYPE = TypedNameOrId<"propertyType">;
export declare function PROPERTY_TYPE(idOrName: number | string): PROPERTY_TYPE;
//# sourceMappingURL=PropertyTypeModel.d.ts.map