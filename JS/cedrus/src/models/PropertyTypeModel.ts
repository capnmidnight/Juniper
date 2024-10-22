import { isString } from "@juniper-lib/util";
import { DataType, hasDataType, hasStorageType, isDataType, isStorageType, StorageType } from "./DataType";
import { INamed } from "./INamed";
import { isTypeStamped, ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";

export interface PropertyTypeModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends INamed, ITypeStamped<"propertyType"> {
    type: DataTypeT;
    storage: StorageTypeT;
    unitsCategory: string;
    description: string;
    ReferencePath?: string;
}

export function isPropertyTypeModel(obj: unknown): obj is PropertyTypeModel<DataType, StorageType>;
export function isPropertyTypeModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeModel<DataTypeT, StorageType>;
export function isPropertyTypeModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyTypeModel<DataType, StorageTypeT>;
export function isPropertyTypeModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyTypeModel<DataTypeT, StorageTypeT>;
export function isPropertyTypeModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataOrStorageType?: DataTypeT | StorageTypeT, storageType?: StorageTypeT): obj is PropertyTypeModel<DataTypeT, StorageTypeT> {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;

    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;
    
    return isTypeStamped("propertyType", obj)
        && hasDataType(obj, dataType)
        && hasStorageType(obj, storageType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "description" in obj
        && isString(obj.description);
}

export interface SetPropertyTypeInput {
    type: DataType;
    storage: StorageType;
    name: string;
    category: string;
    description: string;
    ReferencePath?: string;
}

export type PROPERTY_TYPE = TypedNameOrId<"propertyType">;
export function PROPERTY_TYPE(idOrName: number | string): PROPERTY_TYPE {
    return TypedNameOrId(idOrName, "propertyType");
}