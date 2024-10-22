import { singleton } from "@juniper-lib/util";
import { ElementChild, ReadOnly } from "@juniper-lib/dom";
import { FileInput, FileInputElement, FileViewValue, IBasicFile, OnRequestInput } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { isFileModel, PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";

export class SingleFileEditorElement extends BasePropertyEditorElement<"File", "Single", FileInputElement> {
    readonly #preview: FileInputElement;
    constructor() {
        super(() => FileInput());

        this.#preview = FileInput(
            ReadOnly(true),
            OnRequestInput<IBasicFile, string>(evt => {
                if (isFileModel(evt.value)) {
                    evt.resolve(evt.value.path);
                }
                else {
                    evt.reject();
                }
            })
        );
    }

    get accept() { return this._input.accept; }
    set accept(v) { this._input.accept = v; }

    override _getInputValue(input: FileInputElement): FileViewValue {
        return input.getFiles()[0];
    }

    override _setInputValue(input: FileInputElement, value: FileViewValue) {
        input.addFiles([value]);
    }

    override _getPreviewElement(property: PropertyModel<"File", "Single">): Node | string {
        this.#preview.clear();
        this.#preview.addFiles([property.value]);
        return this.#preview;
    }

    override async _inputToOutput(value: FileViewValue, ds: CedrusDataAPI): Promise<string> {
        return (await ds.mergeFiles([value]))[0];
    }

    static install() {
        return singleton("Juniper::Cedrus::SingleFileEditorElement", () => registerPropertyFactory("Single", "File", SingleFileEditorElement));
    }
}

export function SingleFileEditor(...rest: ElementChild<SingleFileEditorElement>[]) {
    return SingleFileEditorElement.install()(...rest);
} 
