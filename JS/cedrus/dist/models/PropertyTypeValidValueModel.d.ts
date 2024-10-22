import { DataType, StorageType } from "./DataType";
import { ISequenced } from "./ISequenced";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PropertyTypeModel } from "./PropertyTypeModel";
export interface PropertyTypeValidValueModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends ISequenced, ITypeStamped<"propertyTypeValidValue"> {
    value: string;
    propertyType: PropertyTypeModel<DataTypeT, StorageTypeT>;
}
export declare function isPropertyTypeValidValueModel(obj: unknown): obj is PropertyTypeValidValueModel<DataType, StorageType>;
export declare function isPropertyTypeValidValueModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeValidValueModel<DataTypeT, StorageType>;
export declare function isPropertyTypeValidValueModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyTypeValidValueModel<DataType, StorageTypeT>;
export declare function isPropertyTypeValidValueModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyTypeValidValueModel<DataTypeT, StorageTypeT>;
export type PROPERTY_TYPE_VALID_VALUE = TypedNameOrId<"propertyTypeValidValue">;
export declare function PROPERTY_TYPE_VALID_VALUE(idOrName: number | string): PROPERTY_TYPE_VALID_VALUE;
//# sourceMappingURL=PropertyTypeValidValueModel.d.ts.map