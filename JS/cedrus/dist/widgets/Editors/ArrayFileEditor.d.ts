import { ElementChild } from "@juniper-lib/dom";
import { FileInputElement, FileViewValue } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
export declare class ArrayFileEditorElement extends BasePropertyEditorElement<"File", "Array", FileInputElement> {
    #private;
    constructor();
    get accept(): string;
    set accept(v: string);
    _getInputValue(input: FileInputElement): FileViewValue[];
    _setInputValue(input: FileInputElement, values: FileViewValue[]): void;
    _getPreviewElement(property: PropertyModel<"File", "Array">): Node | string;
    _inputToOutput(values: FileViewValue[], ds: CedrusDataAPI): Promise<string[]>;
    static install(): import("@juniper-lib/dom").ElementFactory<ArrayFileEditorElement>;
}
export declare function ArrayFileEditor(...rest: ElementChild<ArrayFileEditorElement>[]): ArrayFileEditorElement;
//# sourceMappingURL=ArrayFileEditor.d.ts.map