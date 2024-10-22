import { isDefined } from "@juniper-lib/util";
import { CedrusDataAPI } from "../../adapters";
import { DataType, DataTypeMap, ENTITY, InputDataTypeMap, PROPERTY_TYPE, PropertyModel, StorageType } from "../../models";
import { IPropertyEditorElement, InputFields } from "./IPropertyEditorFactory";

export type ElementType = HTMLElement & InputFields;

const forwardedAttributes = new Set([
    "disabled",
    "readonly",
    "placeholder",
    "title"
]);

interface IInput extends HTMLElement {
    disabled: boolean;
    readOnly: boolean;
    placeholder: string;
}

export abstract class BasePropertyEditorElement<
    DataTypeT extends DataType,
    StorageTypeT extends StorageType,
    ElementT extends IInput>
    extends HTMLElement
    implements IPropertyEditorElement<DataTypeT, StorageTypeT> {

    static observedAttributes = Array.from(forwardedAttributes);

    readonly _input: ElementT;

    constructor(makeInput: () => ElementT) {
        super();
        this._input = makeInput();
    }

    abstract _getPreviewElement(property: PropertyModel<DataTypeT, StorageTypeT>): Node | string;
    abstract _getInputValue(input: ElementT): InputDataTypeMap[DataTypeT][StorageTypeT];
    abstract _inputToOutput(value: InputDataTypeMap[DataTypeT][StorageTypeT], ds: CedrusDataAPI): Promise<DataTypeMap[DataTypeT][StorageTypeT]>;

    _setInputValue(_input: ElementT, _value: InputDataTypeMap[DataTypeT][StorageTypeT]): void {
    }

    _setOptions(_input: ElementT, _values: DataTypeMap[DataTypeT]["Single"][]): void {
    }

    #setup = false;
    connectedCallback() {
        if (!this.#setup) {
            this.append(this._input);
            this.#setup = true;
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        if (forwardedAttributes.has(name)) {
            if (isDefined(newValue)) {
                this._input.setAttribute(name, newValue);
            }
            else {
                this._input.removeAttribute(name);
            }
        }
    }

    override get id() {
        return this._input.id;
    }

    override set id(v) {
        this._input.id = v;
    }

    getPreviewElement(property: PropertyModel<DataTypeT, StorageTypeT>): Node | string {
        return this._getPreviewElement(property);
    }

    get inputValue() {
        return this._getInputValue(this._input);
    }

    set inputValue(v) {
        this._setInputValue?.call(null, this._input, v);
    }

    #outputValue: DataTypeMap[DataTypeT][StorageTypeT];
    get outputValue() { return this.#outputValue; }

    setValidValues(v: DataTypeMap[DataTypeT]["Single"][]) {
        this._setOptions?.call(null, this._input, v);
    }

    get disabled() {
        return this._input.disabled;
    }

    set disabled(v) {
        this._input.disabled = v;
    }

    get readOnly() {
        return this._input.readOnly;
    }

    set readOnly(v) {
        this._input.readOnly = v;
    }

    get placeholder() {
        return this._input.placeholder;
    }

    set placeholder(v) {
        this._input.placeholder = v;
    }

    override get title() {
        return this._input.title;
    }

    override set title(v) {
        this._input.title = v;
    }

    async save(
        ds: CedrusDataAPI,
        entity: ENTITY,
        propertyType: PROPERTY_TYPE,
        unitOfMeasure: string,
        reference?: ENTITY
    ) {
        const inputValue = this._getInputValue(this._input);
        const outputValue = await this._inputToOutput(inputValue, ds);
        return await ds.setProperty<DataTypeT, StorageTypeT>(
            entity,
            propertyType,
            outputValue,
            unitOfMeasure,
            reference
        );
    }
}
