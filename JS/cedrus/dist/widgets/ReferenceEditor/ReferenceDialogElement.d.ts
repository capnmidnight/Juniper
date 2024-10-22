import { ElementChild } from "@juniper-lib/dom";
import { BaseDialogElement } from "@juniper-lib/widgets";
import { EntityModel } from "../../models";
export declare class ReferenceDialogElement extends BaseDialogElement<EntityModel, EntityModel> {
    #private;
    constructor();
    get selectedReference(): EntityModel;
    set selectedReference(v: EntityModel);
    static install(): import("@juniper-lib/dom").ElementFactory<ReferenceDialogElement>;
}
export declare function ReferenceDialog(...rest: ElementChild<ReferenceDialogElement>[]): ReferenceDialogElement;
//# sourceMappingURL=ReferenceDialogElement.d.ts.map