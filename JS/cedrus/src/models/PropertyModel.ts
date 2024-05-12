import { isString } from "@juniper-lib/util";
import { DataType, DataTypeMap } from "./DataType";
import { IClassificationMarked, isIClassificationMarked } from "./IClassificationMarked";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { IDOrName } from "./IDOrName";
import { ISequenced, isISequenced } from "./ISequenced";
import { ITimeSeries, isITimeSeries } from "./ITimeSeries";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { PropertyTypeModel, isPropertyTypeModel } from "./PropertyTypeModel";

export interface PropertyModel<DataTypeT extends DataType = DataType>
    extends ISequenced,
    ITypeStamped<"property">,
    ICreationTracked,
    IClassificationMarked,
    ITimeSeries<PropertyTypeModel<DataTypeT>> {
    value: DataTypeMap[DataTypeT];
    units: string;
    reference?: PropertyModel<"Link" | "File">;
}

export function isPropertyModel(obj: unknown): obj is PropertyModel;
export function isPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyModel<DataTypeT>;
export function isPropertyModel<DataTypeT extends DataType>(obj: unknown, dataType?: DataTypeT): obj is PropertyModel<DataTypeT> {
    return isTypeStamped("property", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && isIClassificationMarked(obj)
        && isITimeSeries(obj)
        && isPropertyTypeModel(obj.type, dataType)
        && "value" in obj
        && "units" in obj
        && isString(obj.units);
}

export type SetPropertyInput<DataTypeT extends DataType = DataType> = {
    type: IDOrName;
    value: DataTypeMap[DataTypeT];
    unitOfMeasure?: string;
    start?: Date;
    end?: Date;
    classification?: IDOrName
}