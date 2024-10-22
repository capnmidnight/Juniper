import { singleton } from "@juniper-lib/util";
import { ElementChild, Multiple, ReadOnly } from "@juniper-lib/dom";
import { FileInput, FileInputElement, FileViewValue, IBasicFile, OnRequestInput } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { isFileModel, PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";

export class ArrayFileEditorElement extends BasePropertyEditorElement<"File", "Array", FileInputElement> {
    readonly #preview: FileInputElement;
    constructor() {
        super(() => FileInput(Multiple(true)));

        this.#preview = FileInput(
            Multiple(true),
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

    override _getInputValue(input: FileInputElement): FileViewValue[] {
        return input.getFiles();
    }

    override _setInputValue(input: FileInputElement, values: FileViewValue[]) {
        input.addFiles(values);
    }

    override _getPreviewElement(property: PropertyModel<"File", "Array">): Node | string {
        this.#preview.clear();
        this.#preview.addFiles(property.value);
        return this.#preview;
    }

    override async _inputToOutput(values: FileViewValue[], ds: CedrusDataAPI): Promise<string[]> {
        return await ds.mergeFiles(values);
    }

    static install() {
        return singleton("Juniper::Cedrus::ArrayFileEditorElement", () => registerPropertyFactory("Array", "File", ArrayFileEditorElement));
    }
}

export function ArrayFileEditor(...rest: ElementChild<ArrayFileEditorElement>[]) {
    return ArrayFileEditorElement.install()(...rest);
}
