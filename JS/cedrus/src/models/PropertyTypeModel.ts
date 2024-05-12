import { isArray, isString } from "@juniper-lib/util";
import { DataType, hasDataType } from "./DataType";
import { INamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TemplateModel } from "./TemplateModel";

export interface PropertyTypeModel<DataTypeT extends DataType = DataType> extends INamed, ITypeStamped<"propertyType"> {
    dataType: DataTypeT;
    unitsCategory: string;
    description: string;
    templates: TemplateModel[];
}

export function isPropertyTypeModel(obj: unknown): obj is PropertyTypeModel
export function isPropertyTypeModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is PropertyTypeModel<DataTypeT>;
export function isPropertyTypeModel<DataTypeT extends DataType>(obj: unknown, dataType?: DataTypeT): obj is PropertyTypeModel<DataTypeT> {
    return isTypeStamped("propertyType", obj)
        && hasDataType(obj, dataType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "description" in obj
        && isString(obj.description)
        && "templates" in obj
        && isArray(obj.templates);
}

export interface SetPropertyTypeInput {
    dataType: DataType;
    name: string;
    category: string;
    description: string;
}