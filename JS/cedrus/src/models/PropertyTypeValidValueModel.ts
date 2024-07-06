import { isObject } from "@juniper-lib/util";
import { DataType, hasDataType } from "./DataType";
import { ISequenced } from "./ISequenced";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { PropertyTypeModel } from "./PropertyTypeModel";

export interface PropertyTypeValidValueModel<DataTypeT extends DataType = DataType> extends ISequenced, ITypeStamped<"propertyTypeValidValue"> {
    value: string;
    propertyType: PropertyTypeModel<DataTypeT>;
}

export function isPropertyTypeValidValueModel(obj: unknown): obj is PropertyTypeValidValueModel;
export function isPropertyTypeValidValueModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeValidValueModel<DataTypeT>;
export function isPropertyTypeValidValueModel<DataTypeT extends DataType>(obj: unknown, dataType?: DataTypeT): obj is PropertyTypeValidValueModel<DataTypeT> {
    return isTypeStamped("propertyTypeValidValue", obj)
        && "propertyType" in obj
        && isObject(obj.propertyType)
        && hasDataType(obj.propertyType, dataType);
}
