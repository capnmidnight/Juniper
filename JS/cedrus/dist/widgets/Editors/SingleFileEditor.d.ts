import { ElementChild } from "@juniper-lib/dom";
import { FileInputElement, FileViewValue } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
export declare class SingleFileEditorElement extends BasePropertyEditorElement<"File", "Single", FileInputElement> {
    #private;
    constructor();
    get accept(): string;
    set accept(v: string);
    _getInputValue(input: FileInputElement): FileViewValue;
    _setInputValue(input: FileInputElement, value: FileViewValue): void;
    _getPreviewElement(property: PropertyModel<"File", "Single">): Node | string;
    _inputToOutput(value: FileViewValue, ds: CedrusDataAPI): Promise<string>;
    static install(): import("@juniper-lib/dom").ElementFactory<SingleFileEditorElement>;
}
export declare function SingleFileEditor(...rest: ElementChild<SingleFileEditorElement>[]): SingleFileEditorElement;
//# sourceMappingURL=SingleFileEditor.d.ts.map