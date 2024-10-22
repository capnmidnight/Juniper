import { singleton } from "@juniper-lib/util";
import { ReadOnly } from "@juniper-lib/dom";
import { FileInput, OnRequestInput } from "@juniper-lib/widgets";
import { isFileModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";
export class SingleFileEditorElement extends BasePropertyEditorElement {
    #preview;
    constructor() {
        super(() => FileInput());
        this.#preview = FileInput(ReadOnly(true), OnRequestInput(evt => {
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
        return input.getFiles()[0];
    }
    _setInputValue(input, value) {
        input.addFiles([value]);
    }
    _getPreviewElement(property) {
        this.#preview.clear();
        this.#preview.addFiles([property.value]);
        return this.#preview;
    }
    async _inputToOutput(value, ds) {
        return (await ds.mergeFiles([value]))[0];
    }
    static install() {
        return singleton("Juniper::Cedrus::SingleFileEditorElement", () => registerPropertyFactory("Single", "File", SingleFileEditorElement));
    }
}
export function SingleFileEditor(...rest) {
    return SingleFileEditorElement.install()(...rest);
}
//# sourceMappingURL=SingleFileEditor.js.map