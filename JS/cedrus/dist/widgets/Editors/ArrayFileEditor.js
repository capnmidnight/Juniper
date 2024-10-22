import { singleton } from "@juniper-lib/util";
import { Multiple, ReadOnly } from "@juniper-lib/dom";
import { FileInput, OnRequestInput } from "@juniper-lib/widgets";
import { isFileModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";
export class ArrayFileEditorElement extends BasePropertyEditorElement {
    #preview;
    constructor() {
        super(() => FileInput(Multiple(true)));
        this.#preview = FileInput(Multiple(true), ReadOnly(true), OnRequestInput(evt => {
            if (isFileModel(evt.value)) {
                evt.resolve(evt.value.path);
            }
            else {
                evt.reject();
            }
        }));
    }
    get accept() { return this._input.accept; }
    set accept(v) { this._input.accept = v; }
    _getInputValue(input) {
        return input.getFiles();
    }
    _setInputValue(input, values) {
        input.addFiles(values);
    }
    _getPreviewElement(property) {
        this.#preview.clear();
        this.#preview.addFiles(property.value);
        return this.#preview;
    }
    async _inputToOutput(values, ds) {
        return await ds.mergeFiles(values);
    }
    static install() {
        return singleton("Juniper::Cedrus::ArrayFileEditorElement", () => registerPropertyFactory("Array", "File", ArrayFileEditorElement));
    }
}
export function ArrayFileEditor(...rest) {
    return ArrayFileEditorElement.install()(...rest);
}
//# sourceMappingURL=ArrayFileEditor.js.map