import { isNumber, isObject, isString } from "@juniper-lib/util";
import { DataType, DataTypeMap, hasDataType } from "./DataType";
import { INamed, isINamed } from "./INamed";

export interface FlatPropertyModel<DataTypeT extends DataType = DataType> extends INamed {
    typeId: number;
    description: string;
    dataType: DataTypeT;
    unitsCategory: string;
    units: string;
    value: DataTypeMap[DataTypeT];
    referenceId?: number;
}

export function isFlatPropertyModel(obj: unknown): obj is FlatPropertyModel;
export function isFlatPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is FlatPropertyModel<DataTypeT>;
export function isFlatPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType?: DataTypeT): obj is FlatPropertyModel<DataTypeT> {
    return isObject(obj)
        && isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "description" in obj
        && isString(obj.description)
        && hasDataType(obj, dataType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "units" in obj
        && isString(obj.units)
        && "value" in obj;
}
