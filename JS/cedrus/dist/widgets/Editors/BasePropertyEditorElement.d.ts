import { CedrusDataAPI } from "../../adapters";
import { DataType, DataTypeMap, ENTITY, InputDataTypeMap, PROPERTY_TYPE, PropertyModel, StorageType } from "../../models";
import { IPropertyEditorElement, InputFields } from "./IPropertyEditorFactory";
export type ElementType = HTMLElement & InputFields;
interface IInput extends HTMLElement {
    disabled: boolean;
    readOnly: boolean;
    placeholder: string;
}
export declare abstract class BasePropertyEditorElement<DataTypeT extends DataType, StorageTypeT extends StorageType, ElementT extends IInput> extends HTMLElement implements IPropertyEditorElement<DataTypeT, StorageTypeT> {
    #private;
    static observedAttributes: string[];
    readonly _input: ElementT;
    constructor(makeInput: () => ElementT);
    abstract _getPreviewElement(property: PropertyModel<DataTypeT, StorageTypeT>): Node | string;
    abstract _getInputValue(input: ElementT): InputDataTypeMap[DataTypeT][StorageTypeT];
    abstract _inputToOutput(value: InputDataTypeMap[DataTypeT][StorageTypeT], ds: CedrusDataAPI): Promise<DataTypeMap[DataTypeT][StorageTypeT]>;
    _setInputValue(_input: ElementT, _value: InputDataTypeMap[DataTypeT][StorageTypeT]): void;
    _setOptions(_input: ElementT, _values: DataTypeMap[DataTypeT]["Single"][]): void;
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get id(): string;
    set id(v: string);
    getPreviewElement(property: PropertyModel<DataTypeT, StorageTypeT>): Node | string;
    get inputValue(): InputDataTypeMap[DataTypeT][StorageTypeT];
    set inputValue(v: InputDataTypeMap[DataTypeT][StorageTypeT]);
    get outputValue(): DataTypeMap[DataTypeT][StorageTypeT];
    setValidValues(v: DataTypeMap[DataTypeT]["Single"][]): void;
    get disabled(): boolean;
    set disabled(v: boolean);
    get readOnly(): boolean;
    set readOnly(v: boolean);
    get placeholder(): string;
    set placeholder(v: string);
    get title(): string;
    set title(v: string);
    save(ds: CedrusDataAPI, entity: ENTITY, propertyType: PROPERTY_TYPE, unitOfMeasure: string, reference?: ENTITY): Promise<PropertyModel<DataTypeT, StorageTypeT>>;
}
export {};
//# sourceMappingURL=BasePropertyEditorElement.d.ts.map