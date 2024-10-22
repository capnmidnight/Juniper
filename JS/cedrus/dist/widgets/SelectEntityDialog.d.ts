import { ElementChild } from "@juniper-lib/dom";
import { BaseDialogElement } from "@juniper-lib/widgets";
import { EntityModel } from "../models/EntityModel";
import { EntityTypeModel } from "../models/EntityTypeModel";
export declare class SelectEntityDialogElement extends BaseDialogElement<EntityTypeModel[], EntityModel> {
    #private;
    constructor();
    static install(): import("@juniper-lib/dom").ElementFactory<SelectEntityDialogElement>;
}
export declare function SelectEntityDialog(...rest: ElementChild<SelectEntityDialogElement>[]): SelectEntityDialogElement;
//# sourceMappingURL=SelectEntityDialog.d.ts.map