import { isObject } from "@juniper-lib/util";
import { DataType, hasDataType, hasStorageType, isDataType, isStorageType, StorageType } from "./DataType";
import { ISequenced } from "./ISequenced";
import { isTypeStamped, ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PropertyTypeModel } from "./PropertyTypeModel";

export interface PropertyTypeValidValueModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends ISequenced, ITypeStamped<"propertyTypeValidValue"> {
    value: string;
    propertyType: PropertyTypeModel<DataTypeT, StorageTypeT>;
}

export function isPropertyTypeValidValueModel(obj: unknown): obj is PropertyTypeValidValueModel<DataType, StorageType>;
export function isPropertyTypeValidValueModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeValidValueModel<DataTypeT, StorageType>;
export function isPropertyTypeValidValueModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyTypeValidValueModel<DataType, StorageTypeT>;
export function isPropertyTypeValidValueModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyTypeValidValueModel<DataTypeT, StorageTypeT>;
export function isPropertyTypeValidValueModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataOrStorageType?: DataTypeT | StorageTypeT, storageType?: StorageTypeT): obj is PropertyTypeValidValueModel<DataTypeT, StorageTypeT> {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;

    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;

    return isTypeStamped("propertyTypeValidValue", obj)
        && "propertyType" in obj
        && isObject(obj.propertyType)
        && hasDataType(obj.propertyType, dataType)
        && hasStorageType(obj.propertyType, storageType);
}

export type PROPERTY_TYPE_VALID_VALUE = TypedNameOrId<"propertyTypeValidValue">;
export function PROPERTY_TYPE_VALID_VALUE(idOrName: number | string): PROPERTY_TYPE_VALID_VALUE {
    return TypedNameOrId(idOrName, "propertyTypeValidValue");
}
