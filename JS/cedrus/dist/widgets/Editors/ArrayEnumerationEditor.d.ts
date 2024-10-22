import { ElementChild } from "@juniper-lib/dom";
import { ArrayEditorElement, TypedSelectElement } from "@juniper-lib/widgets";
import { PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
export declare class ArrayEnumerationEditorElement extends BasePropertyEditorElement<"Enumeration", "Array", ArrayEditorElement<TypedSelectElement<string>>> {
    constructor();
    _getPreviewElement(property: PropertyModel<"Enumeration", "Array">): Node | string;
    _getInputValue(input: ArrayEditorElement<TypedSelectElement<string>, string>): string[];
    _inputToOutput(value: string[]): Promise<string[]>;
    _setOptions(input: ArrayEditorElement<TypedSelectElement<string>, string>, values: string[]): void;
    static install(): import("@juniper-lib/dom").ElementFactory<ArrayEnumerationEditorElement>;
}
export declare function ArrayEnumerationEditor(...rest: ElementChild<ArrayEnumerationEditorElement>[]): ArrayEnumerationEditorElement;
//# sourceMappingURL=ArrayEnumerationEditor.d.ts.map