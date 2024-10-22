import { singleton } from "@juniper-lib/util";
import { ElementChild, LI, SlotAttr, UL } from "@juniper-lib/dom";
import { ArrayEditor, ArrayEditorElement, TypedSelect, TypedSelectElement } from "@juniper-lib/widgets";
import { PropertyModel } from "../../models";
import { BasePropertyEditorElement } from "./BasePropertyEditorElement";
import { registerPropertyFactory } from "./registerPropertyFactory";

export class ArrayEnumerationEditorElement extends BasePropertyEditorElement<"Enumeration", "Array", ArrayEditorElement<TypedSelectElement<string>>> {

    constructor() {
        super(() => ArrayEditor(
            TypedSelect(
                SlotAttr("selector")
            )
        ));
    }

    override _getPreviewElement(property: PropertyModel<"Enumeration", "Array">): Node | string {
        return UL(...property.value.map(v => LI(v)));
    }

    override _getInputValue(input: ArrayEditorElement<TypedSelectElement<string>, string>): string[] {
        return input.values;
    }

    override _inputToOutput(value: string[]): Promise<string[]> {
        return Promise.resolve(value);
    }

    override _setOptions(input: ArrayEditorElement<TypedSelectElement<string>, string>, values: string[]) {
        input.data = values;
    }

    static install() {
        return singleton("Juniper::Cedrus::ArrayEnumerationEditorElement", () => registerPropertyFactory("Array", "Enumeration", ArrayEnumerationEditorElement));
    }
}

export function ArrayEnumerationEditor(...rest: ElementChild<ArrayEnumerationEditorElement>[]) {
    return ArrayEnumerationEditorElement.install()(...rest);
}
