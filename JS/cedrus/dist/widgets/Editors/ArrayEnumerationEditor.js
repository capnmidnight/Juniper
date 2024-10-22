import { singleton } from "@juniper-lib/util";
import { LI, SlotAttr, UL } from "@juniper-lib/dom";
import { ArrayEditor, TypedSelect } from "@juniper-lib/widgets";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";
export class ArrayEnumerationEditorElement extends BasePropertyEditorElement {
    constructor() {
        super(() => ArrayEditor(TypedSelect(SlotAttr("selector"))));
    }
    _getPreviewElement(property) {
        return UL(...property.value.map(v => LI(v)));
    }
    _getInputValue(input) {
        return input.values;
    }
    _inputToOutput(value) {
        return Promise.resolve(value);
    }
    _setOptions(input, values) {
        input.data = values;
    }
    static install() {
        return singleton("Juniper::Cedrus::ArrayEnumerationEditorElement", () => registerPropertyFactory("Array", "Enumeration", ArrayEnumerationEditorElement));
    }
}
export function ArrayEnumerationEditor(...rest) {
    return ArrayEnumerationEditorElement.install()(...rest);
}
//# sourceMappingURL=ArrayEnumerationEditor.js.map