import { ElementChild } from "@juniper-lib/dom";
import { CedrusDataAPI } from "../../adapters";
import { DataType, DataTypeMap, ENTITY, EntityModel, InputDataTypeMap, PropertyModel, PropertyTypeModel, StorageType } from "../../models";

export interface InputFields {
    disabled: boolean;
    readOnly: boolean;
    placeholder: string;
    title: string;
}

export interface IPropertyEditorElement<DataTypeT extends DataType = DataType, StorageTypeT extends StorageType = StorageType> extends HTMLElement, InputFields {
    id: string;
    inputValue: InputDataTypeMap[DataTypeT][StorageTypeT];
    outputValue: DataTypeMap[DataTypeT][StorageTypeT];
    getPreviewElement(property: PropertyModel<DataTypeT, StorageTypeT>): Node | string;
    setValidValues(data: DataTypeMap[DataTypeT]["Single"][]): void;
    save(
        ds: CedrusDataAPI,
        entity: EntityModel,
        propertyType: PropertyTypeModel,
        unitOfMeasure: string,
        reference?: ENTITY
    ): Promise<PropertyModel<DataTypeT, StorageTypeT>>
}

export type IPropertyEditorFactory<
    DataTypeT extends DataType,
    StorageTypeT extends StorageType> = (...rest: ElementChild[]) => IPropertyEditorElement<DataTypeT, StorageTypeT>;
