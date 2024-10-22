import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { UpdatedEvent } from "@juniper-lib/widgets";
import { EntityModel } from "../../models";
type ReferenceEditorEventMap = {
    "updated": UpdatedEvent<ReferenceEditorElement>;
};
export declare class ReferenceEditorElement extends TypedHTMLElement<ReferenceEditorEventMap> {
    #private;
    static observedAttributes: string[];
    static format(refEntity: EntityModel): HTMLSpanElement;
    get selectedReference(): EntityModel;
    set selectedReference(v: EntityModel);
    constructor();
    get disabled(): boolean;
    set disabled(value: boolean);
    get readOnly(): boolean;
    set readOnly(value: boolean);
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    connectedCallback(): void;
    showDialog(): Promise<EntityModel>;
    static install(): import("@juniper-lib/dom").ElementFactory<ReferenceEditorElement>;
}
export declare function ReferenceEditor(...rest: ElementChild<ReferenceEditorElement>[]): ReferenceEditorElement;
export {};
//# sourceMappingURL=ReferenceEditorElement.d.ts.map