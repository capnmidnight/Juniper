import { isArray, isBoolean, isNumber, isString } from "@juniper-lib/util";
import { DataType, DataTypeMap, hasDataType, hasStorageType, isDataType, isStorageType, StorageType } from "./DataType";
import { INamed, isINamed } from "./INamed";
import { isISequenced } from "./ISequenced";

export interface DataTreeModel {
    entityTypes: Record<number, FlatEntityTypeModel>;
    entities: Record<number, FlatEntityModel>;
    properties: Record<number, FlatPropertyModel>;
}

export interface FlatEntityTypeModel extends INamed {
    isPrimary: boolean;
    parentId?: number;
}

export function isFlatEntityTypeModel(obj: unknown): obj is FlatEntityTypeModel {
    return isINamed(obj)
        && "isPrimary" in obj
        && isBoolean(obj.isPrimary)
        && (!("parentId" in obj)
            || obj.parentId === null
            || isNumber(obj.parentId));
}


export interface FlatEntityModel extends INamed {
    typeId: number;
    properties: number[];
    parents: FlatRelationshipModel[];
    children: FlatRelationshipModel[];
}

export function isFlatEntityModel(obj: unknown) {
    return isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "properties" in obj
        && isArray(obj.properties)
        && obj.properties.every(isNumber)
        && "parents" in obj
        && isArray(obj.parents)
        && obj.parents.every(isFlatRelationshipModel)
        && "children" in obj
        && isArray(obj.children)
        && obj.children.every(isFlatRelationshipModel);
}


export interface FlatPropertyModel<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends INamed {
    typeId: number;
    description: string;
    type: DataTypeT;
    storage: StorageTypeT;
    unitsCategory: string;
    units: string;
    value: DataTypeMap[DataTypeT][StorageTypeT];
}

export function isFlatPropertyModel(obj: unknown): obj is FlatPropertyModel;
export function isFlatPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is FlatPropertyModel<DataTypeT, StorageType>;
export function isFlatPropertyModel<StorageTypeT extends StorageType>(obj: unknown, storageType: StorageTypeT): obj is FlatPropertyModel<DataType, StorageTypeT>;
export function isFlatPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataType: DataTypeT, storageType: StorageTypeT): obj is FlatPropertyModel<DataTypeT, StorageTypeT>;
export function isFlatPropertyModel<DataTypeT extends DataType, StorageTypeT extends StorageType>(obj: unknown, dataOrStorageType?: DataTypeT | StorageTypeT, storageType?: StorageTypeT): obj is FlatPropertyModel<DataTypeT, StorageTypeT> {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;

    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;

    return isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "description" in obj
        && isString(obj.description)
        && hasDataType(obj, dataType)
        && hasStorageType(obj, storageType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "units" in obj
        && isString(obj.units)
        && "value" in obj;
}

export interface FlatRelationshipModel extends INamed {
}

export function isFlatRelationshipModel(obj: unknown): obj is FlatRelationshipModel {
    return isISequenced(obj);
}