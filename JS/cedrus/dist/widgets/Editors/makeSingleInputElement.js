import { singleton } from "@juniper-lib/util";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";
export function makeSingleInputElement(dataType, makeInput, getPreviewElement, inputToOutput, getValue, setValue, setOptions) {
    class PropertyEditorElement extends BasePropertyEditorElement {
        constructor() {
            super(makeInput);
        }
        _getPreviewElement(property) {
            return getPreviewElement(property);
        }
        async _inputToOutput(value) {
            return await inputToOutput(value);
        }
        _getInputValue(input) {
            return getValue(input);
        }
        _setInputValue(input, value) {
            try {
                setValue?.call(null, input, value);
            }
            catch (err) {
                console.warn(err);
                setValue?.call(null, input, null);
            }
        }
        _setOptions(input, values) {
            setOptions?.call(null, input, values);
        }
        static install() {
            return singleton(`Juniper::Cedrus::Single${dataType}EditorElement`, () => registerPropertyFactory("Single", dataType, PropertyEditorElement));
        }
    }
    ;
    return function (...rest) {
        return PropertyEditorElement.install()(...rest);
    };
}
//# sourceMappingURL=makeSingleInputElement.js.map