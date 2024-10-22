import { Exception, IDisposable, arrayFilter, arrayRemove, arrayReplace, arrayScan, compareBy, distinct, groupBy, isDefined, isNullOrUndefined, isNumber, makeLookup } from "@juniper-lib/util";
import { Button, ClassList, Div, FieldSet, H2, HtmlRender, I, IDisableable, Label, Legend, OnClick, Open, Optional, ReadOnly, TitleAttr } from "@juniper-lib/dom";
import { magnifyingGlassTiltedRight, wastebasket } from "@juniper-lib/emoji";
import { TypedEventTarget } from "@juniper-lib/events";
import { Cancelable, Deletable, OnDelete, OnItemSelected, OnRemoving, OnUpdated, PropertyGroup, PropertyList, RemovingEvent, TypedItemSelectedEvent, UpdatedEvent } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../adapters";
import { EntityModel, EntityTypeModel, PropertyModel, PropertyTemplateModel, PropertyTypeModel, PropertyTypeValidValueModel, RelationshipModel, RelationshipTemplateModel, RelationshipTypeModel, isEntityModel } from "../models";
import { AllowedPropertyTypes, LockPropertyType, Property, PropertyEditor, PropertyEditorElement, PropertyType, PropertyTypes, UnitAbbreviations, Units, ValidValues } from "./PropertyEditorElement";
import { ReferenceDataManager } from "./ReferenceEditor";
import { AllowedEntityTypes, RelationshipEditor, RelationshipEditorElement, RelationshipType, RelationshipTypes, Relationships } from "./RelationshipEditorElement";
import { TemplateName } from "./TemplateName";

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

export class EntityManager extends TypedEventTarget<EntityManagerEvents> implements IDisableable, IDisposable {

    static async load(entityId: number, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    static async load(entity: EntityModel, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    static async load(getEntity: Promise<EntityModel>, options?: Partial<EntityManagerOptions>): Promise<EntityManager>;
    static async load(entityOrIdOrGetEntity: number | EntityModel | Promise<EntityModel>, options?: Partial<EntityManagerOptions>): Promise<EntityManager> {
        const ds = await CedrusDataAPI.dataSourceTask;

        const getEntity = entityOrIdOrGetEntity instanceof Promise
            ? entityOrIdOrGetEntity
            : isNumber(entityOrIdOrGetEntity)
                ? ds.getEntity(entityOrIdOrGetEntity)
                : Promise.resolve(entityOrIdOrGetEntity);

        const getProperties = getEntity.then(entity =>
            ds.getProperties(entity));

        const getPropertyTemplates = getEntity.then(entity =>
            ds.getPropertyTemplates(entity.type));

        const getRelationships = getEntity.then(entity =>
            ds.getRelationships({ parent: [entity] }));

        const getRelationshipTemplates = getEntity.then(entity =>
            ds.getRelationshipTemplates(entity.type));

        const [entity, properties, propertyTemplates, allPropertyTypes, validValues, relationships, relationshipTemplates, allRelationshipTypes, allEntityTypes, units, abbreviations] = await Promise.all([
            getEntity,
            getProperties,
            getPropertyTemplates,
            ds.getPropertyTypes(),
            ds.getPropertyTypeValidValues(),
            getRelationships,
            getRelationshipTemplates,
            ds.getRelationshipTypes(),
            ds.getEntityTypes(),
            ds.getUnitsData(),
            ds.getUnitAbbreviations()
        ]);

        return new EntityManager(
            options,
            ds,
            entity,
            properties,
            propertyTemplates,
            allPropertyTypes,
            validValues,
            relationships,
            relationshipTemplates,
            allRelationshipTypes.filter(r => r.name !== "Comment" && r.name !== "Tag"),
            allEntityTypes,
            units,
            abbreviations
        );
    }

    readonly #deletable: boolean;
    readonly #readOnly: boolean;
    readonly #showRelationships: boolean;
    readonly #ds: CedrusDataAPI;
    readonly #propertiesByPropertyTypeId: Map<number, PropertyModel>;
    readonly #allPropertyTypes: Map<string, PropertyTypeModel>;
    readonly #propertyTemplates: PropertyTemplateModel[];
    readonly #validValuesByPropertyTypeId: Map<number, string[]>;
    readonly #propertyGroups: Map<string, PropertyEditorElement[]>;
    readonly #extraPropertyTypes: PropertyTypeModel[];
    readonly #updated: UpdatedEvent<EntityManager>;

    readonly #references: EntityModel[] = [];
    get references() { return this.#references; }

    get otherFieldsAvailable() { return this.#extraPropertyTypes.length > 0; }

    readonly #relationships: RelationshipModel[];
    get relationships(): ReadonlyArray<RelationshipModel> { return this.#relationships; }

    readonly #refMgr: ReferenceDataManager;
    readonly #relationshipGroups: Map<string, RelationshipEditorElement>;
    readonly #relationshipTemplates: RelationshipTemplateModel[];
    readonly #allRelationshipTypes: Map<string, RelationshipTypeModel>;
    readonly #units: Map<string, string[]>;
    readonly #abbreviations: Map<string, string>;
    readonly #entity: EntityModel;

    get entity() { return this.#entity; }

    private constructor(
        options: Partial<EntityManagerOptions>,
        ds: CedrusDataAPI,
        entity: EntityModel,
        properties: PropertyModel[],
        propertyTemplates: PropertyTemplateModel[],
        allPropertyTypes: PropertyTypeModel[],
        validValues: PropertyTypeValidValueModel[],
        relationships: RelationshipModel[],
        relationshipTemplates: RelationshipTemplateModel[],
        allRelationshipTypes: RelationshipTypeModel[],
        allEntityTypes: EntityTypeModel[],
        units: Map<string, string[]>,
        abbreviations: Map<string, string>) {

        super();

        this.#updated = new UpdatedEvent();
        this.#deletable = !!options?.deletable;
        this.#readOnly = !!options?.readOnly;
        this.#showRelationships = !options?.hideRelationships;
        this.#ds = ds;

        this.#refMgr = ReferenceDataManager.instance;
        this.#refMgr.addScopedEventListener(this, "updated", () =>
            this.#updateReferences());

        this.#allPropertyTypes = makeLookup(allPropertyTypes, p => p.name);
        this.#relationships = relationships;
        this.#allRelationshipTypes = makeLookup(allRelationshipTypes, p => p.name);
        this.#units = units;
        this.#abbreviations = abbreviations;

        this.#entity = entity;

        this.#propertyTemplates = propertyTemplates;
        this.#validValuesByPropertyTypeId = groupBy(validValues, v => v.propertyType.id, v => v.value);

        this.#propertiesByPropertyTypeId = makeLookup(
            properties,
            v => v.type.id
        );

        this.#extraPropertyTypes = Array.from(this.#allPropertyTypes.values())
            .filter(v => !this.#propertiesByPropertyTypeId.has(v.id)
                && !v.name.startsWith("Reference"));

        const templatedPropertyTypeIds = new Set(propertyTemplates.flatMap(t =>
            t.propertyTypes.map(pt => pt.id)));

        if (!options?.hideOthers) {
            this.#propertyTemplates.push({
                typeStamp: "propertyTemplate",
                id: 0,
                entityType: entity.type,
                name: "Other",
                propertyTypes: properties
                    .map(p => p.type)
                    .filter(pt => !templatedPropertyTypeIds.has(pt.id))
            });
        }

        this.#propertyGroups = new Map();
        for (const template of this.#propertyTemplates) {
            if (!this.#propertyGroups.has(template.name)) {
                this.#propertyGroups.set(template.name, []);
            }

            const propGroup = this.#propertyGroups.get(template.name);
            propGroup.push(...template
                .propertyTypes
                .filter(pt => !this.#readOnly || this.#propertiesByPropertyTypeId.has(pt.id))
                .map(pt => {
                    const property = this.#propertiesByPropertyTypeId.get(pt.id);
                    
                    return PropertyEditor(
                        TemplateName(template.name),
                        LockPropertyType(true),
                        Open(!property),
                        Cancelable(!!property),
                        PropertyTypes(this.#allPropertyTypes),
                        ValidValues(this.#validValuesByPropertyTypeId),
                        Units(this.#units),
                        UnitAbbreviations(this.#abbreviations),
                        Deletable(property && template.name === "Other"),
                        ReadOnly(this.#readOnly),
                        isDefined(property) && Property(property) || PropertyType(pt),
                        OnUpdated<PropertyEditorElement>(evt =>
                            this.saveProperty(evt.target)),
                        OnDelete<PropertyEditorElement>(evt =>
                            this.deleteProperty(evt.target))
                    );
                }));

            if (template.name === "Other"
                && this.otherFieldsAvailable
                && !this.#readOnly) {
                propGroup.push(this.makeOtherPropertyEditor());
            }
        }

        this.#updateReferences();

        if (this.#showRelationships) {

            this.#relationshipTemplates = relationshipTemplates;

            const usedEntityTypeIds = new Set(relationshipTemplates.flatMap(template => template.allowedEntityTypes.map(et => et.id)));
            const leftoverEntityTypes = allEntityTypes.filter(et => !usedEntityTypeIds.has(et.id));

            if (leftoverEntityTypes.length > 0
                && !options?.hideOthers
                && !this.#readOnly) {
                this.#relationshipTemplates.push({
                    typeStamp: "relationshipTemplate",
                    id: 0,
                    entityType: entity.type,
                    name: "Other",
                    relationshipType: this.#allRelationshipTypes.get("Default"),
                    allowedEntityTypes: leftoverEntityTypes
                });
            }

            const relationshipsByTemplateId = groupBy(relationships, relationship => {
                const template = arrayScan(
                    this.#relationshipTemplates,
                    template => template.relationshipType.id === relationship.type.id
                        && template.allowedEntityTypes.some(et => relationship.child.type.id === et.id));
                return template?.id ?? 0;
            });

            this.#relationshipGroups = new Map(this.#relationshipTemplates
                .map(template => {
                    const relationships = relationshipsByTemplateId.get(template.id);
                    return [
                        template.name,
                        RelationshipEditor(
                            TemplateName(template.name),
                            RelationshipTypes(allRelationshipTypes),
                            RelationshipType(template.relationshipType),
                            AllowedEntityTypes(template.allowedEntityTypes),
                            Relationships(relationships),
                            ReadOnly(this.#readOnly)
                        )
                    ];
                }));
        }
    }

    dispose() {
        this.#refMgr.removeScope(this);
    }

    #disabled = false;
    get disabled() { return this.#disabled; }
    set disabled(value) {
        if (value !== this.disabled) {
            this.#disabled = value;
            for (const propGroup of this.#propertyGroups.values()) {
                for (const editor of propGroup) {
                    editor.disabled = value;
                }
            }
        }
    }

    connectAt(container: HTMLElement, showLink = false) {
        const img = this.hasPropertyGroup("Info")
            && this.getPropertyGroupElement("Info", "Image")
            || null;
        if (img) {
            img.showLabel = false;
        }

        HtmlRender(
            container,
            Div(
                H2(
                    ...Optional(this.#deletable,
                        Button(
                            ClassList("borderless"),
                            TitleAttr("Delete entity"),
                            I(wastebasket.value),
                            OnClick(() => this.dispatchEvent(new RemovingEvent(this.entity)))
                        )
                    ),
                    this.entity.type.name,
                    ...Optional(showLink,
                        Button(
                            ClassList("borderless"),
                            TitleAttr("View entity"),
                            I(magnifyingGlassTiltedRight.value),
                            OnClick(() => this.dispatchEvent(new TypedItemSelectedEvent(this.entity)))
                        )
                    )
                ),
                ...Optional(isDefined(img), img),
                PropertyList(
                    ...this.propertyGroups.map(groupName =>
                        this.createPropertyGroup(groupName)
                    ),
                    ...Optional(!this.#readOnly && this.#relationships.length > 1,
                        ...this.relationshipGroups.map(groupName =>
                            this.createRelationshipGroup(groupName)
                        )
                    )
                )
            )
        );
    }

    createPropertyListing(groupName: string) {
        return FieldSet(
            Legend(groupName),
            PropertyList(
                this.createPropertyGroup(groupName)
            )
        );
    }

    createPropertyGroup(groupName: string) {
        return PropertyGroup(
            groupName,
            ...this.getPropertyGroup(groupName)
        );
    }

    createRelationshipGroup(groupName: string) {
        return PropertyGroup(
            groupName,
            Label(groupName),
            this.getRelationshipGroup(groupName),
            OnItemSelected<EntityModel | RelationshipModel, RelationshipEditorElement>(evt => {
                if (isEntityModel(evt.item)) {
                    this.saveRelationship(evt.target, evt.item);
                    evt.preventDefault();
                }
            }),
            OnRemoving<RelationshipModel, RelationshipEditorElement>(evt =>
                this.deleteRelationship(evt.target, evt.item)
            )
        )
    }

    createRelationshipFieldSet(groupName: string) {
        return FieldSet(
            Legend(groupName),
            PropertyList(
                this.getRelationshipGroup(groupName),
                OnItemSelected<EntityModel | RelationshipModel, RelationshipEditorElement>(evt => {
                    if (isEntityModel(evt.item)) {
                        this.saveRelationship(evt.target, evt.item);
                        evt.preventDefault();
                    }
                }),
                OnRemoving<RelationshipModel, RelationshipEditorElement>(evt =>
                    this.deleteRelationship(evt.target, evt.item)
                )
            )
        )
    }

    makeOtherPropertyEditor() {
        return PropertyEditor(
            TemplateName("Other"),
            Open(true),
            LockPropertyType(false),
            Cancelable(false),
            Deletable(false),
            PropertyTypes(this.#allPropertyTypes),
            ValidValues(this.#validValuesByPropertyTypeId),
            Units(this.#units),
            UnitAbbreviations(this.#abbreviations),
            AllowedPropertyTypes(this.#extraPropertyTypes),
            OnUpdated<PropertyEditorElement>(evt =>
                this.saveProperty(evt.target)),
            OnDelete<PropertyEditorElement>(evt =>
                this.deleteProperty(evt.target))
        );
    }

    get propertyGroups() { return Array.from(this.#propertyGroups.keys()); }

    hasPropertyGroup(groupName: string) {
        return this.#propertyGroups.has(groupName);
    }

    getPropertyGroup(groupName: string) {
        if (!this.hasPropertyGroup(groupName)) {
            throw new Error(`Property Group ${groupName} does not exist. Existing groups are ${this.propertyGroups.join(", ")}.`)
        }

        return this.#propertyGroups.get(groupName);
    }

    getPropertyGroupElement(groupName: string, propertyTypeName: string) {
        const group = this.getPropertyGroup(groupName);

        if (group) {
            for (const element of group) {
                if (element.propertyTypeName === propertyTypeName) {
                    return element;
                }
            }
        }

        return null;
    }

    saveProperty(editor: PropertyEditorElement) {
        return this.#withEditorDisable(editor, async () => {
            const isNew = isNullOrUndefined(editor.property);
            await editor.saveProperty(this.#ds, this.#entity, this.#readOnly);
            this.#propertiesByPropertyTypeId.set(editor.property.type.id, editor.property)
            if (isNew) {
                arrayFilter(this.#extraPropertyTypes, p => p.id === editor.property.type.id);
            }

            if (editor.templateName === "Other"
                && this.otherFieldsAvailable
                && !this.#readOnly) {
                editor.parentElement.append(this.makeOtherPropertyEditor());
            }

            this.#updateReferences();
        });
    }

    async deleteProperty(editor: PropertyEditorElement) {
        const prop = editor.property;
        return isDefined(prop) && await this.#withEditorDisable(editor, async () => {
            await this.#ds.deleteProperty(prop);
            this.#propertiesByPropertyTypeId.delete(prop.type.id);
            if (editor.templateName === "Other") {
                const toAdd = this.#allPropertyTypes.get(prop.type.name);
                this.#extraPropertyTypes.push(toAdd);
            }
            editor.remove();
            this.#updateReferences();
        });
    }

    #updateReferences() {
        const props = Array.from(this.#propertiesByPropertyTypeId.values());
        const references = distinct(props
            .map(p => p.reference)
            .filter(e => e?.id),
            compareBy(e => e.id)
        );;
        arrayReplace(this.#references, references);
        this.dispatchEvent(this.#updated);
    }

    get relationshipGroups() {
        return this.#showRelationships
            ? Array.from(this.#relationshipGroups.keys())
            : [];
    }

    hasRelationshipGroup(groupName: string) {
        return this.#showRelationships && this.#relationshipGroups.has(groupName);
    }

    getRelationshipGroup(groupName: string) {
        if (this.#showRelationships && !this.hasRelationshipGroup(groupName)) {
            throw new Error(`Relationship Group ${groupName} does not exist. Existing groups are ${this.relationshipGroups.join(", ")}.`)
        }

        return this.#showRelationships && this.#relationshipGroups.get(groupName);
    }

    async saveRelationship(editor: RelationshipEditorElement, childEntity: EntityModel) {
        return isDefined(childEntity) && await this.#withEditorDisable(editor, async () => {
            const template = arrayScan(this.#relationshipTemplates, t => t.name === editor.templateName);
            let propertyEntity: EntityModel = null;
            if (template?.propertyEntityType) {
                const propertyEntityName = `${this.#entity.name} - ${childEntity.name}`;
                propertyEntity = await this.#ds.setEntity(propertyEntityName, template.propertyEntityType);
            }
            const relationships = Array.from(editor.relationships);
            const relationship = await this.#ds.setRelationship(
                this.#entity,
                childEntity,
                propertyEntity,
                editor.relationshipType
            );
            relationships.push(relationship);
            this.#relationships.push(relationship);
            editor.relationships = relationships;
        });
    }

    async deleteRelationship(editor: RelationshipEditorElement, relationship: RelationshipModel) {
        return isDefined(relationship) && await this.#withEditorDisable(editor, async () => {
            const relationships = [...editor.relationships];
            await this.#ds.deleteRelationship(relationship);
            arrayRemove(relationships, relationship);
            arrayRemove(this.#relationships, relationship);
            editor.relationships = relationships;
        });
    }

    async #withEditorDisable(editor: IDisableable, action: () => Promise<void>) {
        try {
            editor.disabled = true;
            document.body.style.cursor = "wait";
            await action();
            return true;
        }
        catch (exp) {
            if (exp instanceof Error || exp instanceof Exception) {
                const match = exp.message.match(/Reason: (.+)/m);
                const reason = match && match[1] || exp.message;
                alert(reason);
            }
            console.error(exp);
        }
        finally {
            document.body.style.removeProperty("cursor");
            editor.disabled = false;
        }

        return false;

    }
}
