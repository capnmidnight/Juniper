import { IDisposable } from "@juniper-lib/util";
import { IDisableable } from "@juniper-lib/dom";
import { TypedEventTarget } from "@juniper-lib/events";
import { RemovingEvent, TypedItemSelectedEvent, UpdatedEvent } from "@juniper-lib/widgets";
import { EntityModel, RelationshipModel } from "../models";
import { PropertyEditorElement } from "./PropertyEditorElement";
import { RelationshipEditorElement } from "./RelationshipEditorElement";
interface EntityManagerOptions {
    hideOthers: boolean;
    hideRelationships: boolean;
    deletable: boolean;
    readOnly: boolean;
}
type EntityManagerEvents = {
    "itemselected": TypedItemSelectedEvent<EntityModel>;
    "removing": RemovingEvent<EntityModel>;
    "updated": UpdatedEvent<EntityManager>;
};
export declare class EntityManager extends TypedEventTarget<EntityManagerEvents> implements IDisableable, IDisposable {
    #private;
    static load(entityId: number, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    static load(entity: EntityModel, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    static load(getEntity: Promise<EntityModel>, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    get references(): EntityModel[];
    get otherFieldsAvailable(): boolean;
    get relationships(): ReadonlyArray<RelationshipModel>;
    get entity(): EntityModel;
    private constructor();
    dispose(): void;
    get disabled(): boolean;
    set disabled(value: boolean);
    connectAt(container: HTMLElement, showLink?: boolean): void;
    createPropertyListing(groupName: string): HTMLFieldSetElement;
    createPropertyGroup(groupName: string): import("@juniper-lib/widgets").PropertyGroupElement;
    createRelationshipGroup(groupName: string): import("@juniper-lib/widgets").PropertyGroupElement;
    createRelationshipFieldSet(groupName: string): HTMLFieldSetElement;
    makeOtherPropertyEditor(): PropertyEditorElement;
    get propertyGroups(): string[];
    hasPropertyGroup(groupName: string): boolean;
    getPropertyGroup(groupName: string): PropertyEditorElement[];
    getPropertyGroupElement(groupName: string, propertyTypeName: string): PropertyEditorElement;
    saveProperty(editor: PropertyEditorElement): Promise<boolean>;
    deleteProperty(editor: PropertyEditorElement): Promise<boolean>;
    get relationshipGroups(): string[];
    hasRelationshipGroup(groupName: string): boolean;
    getRelationshipGroup(groupName: string): RelationshipEditorElement;
    saveRelationship(editor: RelationshipEditorElement, childEntity: EntityModel): Promise<boolean>;
    deleteRelationship(editor: RelationshipEditorElement, relationship: RelationshipModel): Promise<boolean>;
}
export {};
//# sourceMappingURL=EntityManager.d.ts.map