import { DataType } from "./DataType";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { PropertyTypeModel, isPropertyTypeModel } from "./PropertyTypeModel";
import { TemplateModel, isTemplateModel } from "./TemplateModel";

export interface TemplatePropertyModel<DataTypeT extends DataType = DataType> extends ITypeStamped<"templateProperty"> {
    template: TemplateModel;
    propertyType: PropertyTypeModel<DataTypeT>;
}

export function isTemplatePropertyModel(obj: unknown): obj is TemplatePropertyModel;
export function isTemplatePropertyModel<DataTypeT extends DataType>(obj: unknown, dataType: DataTypeT): obj is TemplatePropertyModel<DataTypeT>;
export function isTemplatePropertyModel<DataTypeT extends DataType>(obj: unknown, dataType?: DataTypeT): obj is TemplatePropertyModel<DataTypeT> {
    return isTypeStamped("templateProperty", obj)
        && "template" in obj
        && isTemplateModel(obj.template)
        && "propertyType" in obj
        && isPropertyTypeModel(obj.propertyType, dataType);
}
