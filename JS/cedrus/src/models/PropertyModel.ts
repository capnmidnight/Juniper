import { isDate, isNullOrUndefined, isString } from "@juniper-lib/util";
import { DataType, DataTypeMap, StorageType, isDataType, isStorageType } from "./DataType";
import { ENTITY, EntityModel } from "./EntityModel";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { ISequenced, isISequenced } from "./ISequenced";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { PROPERTY_TYPE, PropertyTypeModel, isPropertyTypeModel } from "./PropertyTypeModel";
import { isUserModel, UserModel } from "./UserModel";

export interface PropertyModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType>
    extends ISequenced,
    ITypeStamped<"property">,
    ICreationTracked {
    type: PropertyTypeModel<DataTypeT, StorageTypeT>;
    value: DataTypeMap[DataTypeT][StorageTypeT];
    units: string;
    reference?: EntityModel;
    entity?: EntityModel;
    updatedBy: UserModel;
    updatedOn: Date;
}

export function isPropertyModel(obj: unknown): obj is PropertyModel<DataType, StorageType>;
export function isPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyModel<DataTypeT, StorageType>;
export function isPropertyModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is PropertyModel<DataType, StorageTypeT>;
export function isPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is PropertyModel<DataTypeT, StorageTypeT>;
export function isPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataOrStorageType?: DataTypeT | StorageTypeT, storageType?: StorageTypeT): obj is PropertyModel<DataTypeT, StorageTypeT> {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;
    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;

    return isTypeStamped("property", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && "type" in obj
        && isPropertyTypeModel(obj.type, dataType, storageType)
        && "value" in obj
        && "units" in obj
        && isString(obj.units)
        && "updatedBy" in obj
        && isUserModel(obj.updatedBy)
        && "updatedOn" in obj
        && (isNullOrUndefined(obj.updatedOn)
            || isDate(obj.updatedOn));
}

export type SetPropertyInput<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> = {
    type: PROPERTY_TYPE;
    value: DataTypeMap[DataTypeT][StorageTypeT];
    unitOfMeasure?: string;
    reference?: ENTITY;
}

export type PROPERTY = TypedNameOrId<"property">;
export function PROPERTY(idOrName: number | string): PROPERTY {
    return TypedNameOrId(idOrName, "property");
}