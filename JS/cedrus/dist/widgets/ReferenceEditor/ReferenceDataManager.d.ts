import { TypedEventTarget } from "@juniper-lib/events";
import { UpdatedEvent } from "@juniper-lib/widgets";
import { EntityModel, PropertyModel } from "../../models";
export type ReferencePropertyTypeTypes = "ReferenceFile" | "ReferenceLink";
export declare const referencePropertyTypes: ReferencePropertyTypeTypes[];
export interface ReferenceModel {
    id: number;
    name: string;
    authors: string;
    date: Date;
    type: ReferencePropertyTypeTypes;
    value: string;
}
export declare function toReferenceModel(entity: EntityModel, properties: PropertyModel[]): ReferenceModel;
type ReferenceDataManagerEventsMap = {
    "updated": UpdatedEvent<ReferenceDataManager>;
};
export declare class ReferenceDataManager extends TypedEventTarget<ReferenceDataManagerEventsMap> {
    #private;
    static get instance(): ReferenceDataManager;
    get entities(): EntityModel[];
    get references(): ReferenceModel[];
    getReference(id: number): ReferenceModel;
    constructor();
    addReference(entity: EntityModel, properties: PropertyModel[]): ReferenceModel;
}
export {};
//# sourceMappingURL=ReferenceDataManager.d.ts.map