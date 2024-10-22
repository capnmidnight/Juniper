import { singleton } from "@juniper-lib/util";
import { ElementChild } from "@juniper-lib/dom";
import { DataType, DataTypeMap, InputDataTypeMap, PropertyModel } from "../../models";
import { BasePropertyEditorElement, ElementType } from "./BasePropertyEditorElement";
import { IPropertyEditorFactory } from "./IPropertyEditorFactory";
import { registerPropertyFactory } from "./registerPropertyFactory";

export function makeSingleInputElement<
    DataTypeT extends DataType,
    ElementT extends ElementType>(
        dataType: DataTypeT,
        makeInput: () => ElementT,
        getPreviewElement: (property: PropertyModel<DataTypeT, "Single">) => Node | string,
        inputToOutput: (input: InputDataTypeMap[DataTypeT]["Single"]) => Promise<DataTypeMap[DataTypeT]["Single"]> | DataTypeMap[DataTypeT]["Single"],
        getValue: (input: ElementT) => InputDataTypeMap[DataTypeT]["Single"],
        setValue?: (input: ElementT, value: InputDataTypeMap[DataTypeT]["Single"]) => void,
        setOptions?: (input: ElementT, values: DataTypeMap[DataTypeT]["Single"][]) => void
    ): IPropertyEditorFactory<DataTypeT, "Single"> {

    class PropertyEditorElement extends BasePropertyEditorElement<DataTypeT, "Single", ElementT> {
        constructor() {
            super(makeInput);
        }

        override _getPreviewElement(property: PropertyModel<DataTypeT, "Single">): Node | string {
            return getPreviewElement(property);
        }

        override async _inputToOutput(value: InputDataTypeMap[DataTypeT]["Single"]): Promise<DataTypeMap[DataTypeT]["Single"]> {
            return await inputToOutput(value);
        }

        override _getInputValue(input: ElementT) {
            return getValue(input);
        }

        override _setInputValue(input: ElementT, value: InputDataTypeMap[DataTypeT]["Single"]) {
            try {
                setValue?.call(null, input, value);
            }
            catch (err) {
                console.warn(err);
                setValue?.call(null, input, null);
            }
        }

        override _setOptions(input: ElementT, values: DataTypeMap[DataTypeT]["Single"][]): void {
            setOptions?.call(null, input, values);
        }

        static install() {
            return singleton(`Juniper::Cedrus::Single${dataType}EditorElement`, () => registerPropertyFactory("Single", dataType, PropertyEditorElement));
        }
    };

    return function (...rest: ElementChild<PropertyEditorElement>[]) {
        return PropertyEditorElement.install()(...rest);
    }
}

