import { arrayScan, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { Button, Clear, ElementChild, For, HtmlRender, ID, IDisableable, Label, Multiple, OnClick, PlaceHolder, SingletonStyleBlob, TitleAttr, TypedHTMLElement, TypedHtmlProp, columnGap, content, display, elementSetDisplay, em, flexDirection, fontWeight, height, maxHeight, px, registerFactory, rule, vh } from "@juniper-lib/dom";
import { plus } from "@juniper-lib/emoji";
import { LabelField, Nullable, OnItemSelected, PropertyGroup, PropertyList, PropertyListElement, SortKeyField, TypedItemSelectedEvent, TypedSelect, TypedSelectElement, ValueField } from "@juniper-lib/widgets";
import { EntityModel, EntityTypeModel, RelationshipModel, RelationshipTypeModel } from "../models";
import { SelectEntityDialog, SelectEntityDialogElement } from "./SelectEntityDialog";
import { IHasTemplateName } from "./TemplateName";

export function AllowedEntityTypes(entityTypes: EntityTypeModel[]) {
    return TypedHtmlProp<RelationshipEditorElement>("allowedEntityTypes", entityTypes);
}

export function RelationshipTypes(relationshipTypes: RelationshipTypeModel[]) {
    return TypedHtmlProp<RelationshipEditorElement>("relationshipTypes", relationshipTypes);
}

export function RelationshipType(relationshipType: RelationshipTypeModel) {
    return TypedHtmlProp<RelationshipEditorElement>("relationshipType", relationshipType);
}

export function Relationships(relationships: RelationshipModel[]) {
    return TypedHtmlProp<RelationshipEditorElement>("relationships", relationships);
}

type RelationshipEditorEventMap = {
    "itemselected": TypedItemSelectedEvent<RelationshipModel | EntityModel, RelationshipEditorElement>
}

export class RelationshipEditorElement extends TypedHTMLElement<RelationshipEditorEventMap> implements IHasTemplateName, IDisableable {

    static observedAttributes = [
        "disabled",
        "relationshipid",
        "relationshiptypename",
        "readonly"
    ];

    readonly #relationshipTypeSelector: TypedSelectElement<RelationshipTypeModel>;
    readonly #contents: PropertyListElement;
    get relationshipTypes() { return this.#relationshipTypeSelector.data; }
    set relationshipTypes(v) {
        this.#relationshipTypeSelector.data = v;
        if (v.length === 1) {
            this.relationshipType = v[0];
        }
        this.#render();
    }

    #allowedEntityTypes: EntityTypeModel[];
    get allowedEntityTypes() { return this.#allowedEntityTypes; }
    set allowedEntityTypes(v) { this.#allowedEntityTypes = v; }

    readonly #addButton: HTMLButtonElement;
    readonly #entitySelector: SelectEntityDialogElement;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Cedrus::RelationshipEditorElement", () =>
            rule("relationship-editor",

                rule("> div",
                    display("flex"),
                    flexDirection("row"),
                    columnGap(em(.25))
                ),

                rule("> label",
                    fontWeight("bold"),

                    rule("::after",
                        content("':'")
                    )
                ),

                rule(" select[multiple]",
                    height(em(10))
                ),

                rule(" select:not([multiple])",
                    height(px(26))
                ),

                rule(" img",
                    maxHeight(vh(10))
                )
            )
        );

        this.#relationshipTypeSelector = TypedSelect(
            ID(stringRandom(12)),
            ValueField<RelationshipTypeModel>(v => v.id.toFixed()),
            LabelField<RelationshipTypeModel>(v => v.name),
            SortKeyField<RelationshipTypeModel>(v => v.name),
            PlaceHolder("Select type..."),
            TitleAttr("Relationship Type"),
            OnItemSelected<RelationshipTypeModel>((evt) => {
                this.relationshipType = evt.item;
            })
        );

        const relationshipDescription = (rel: RelationshipModel) =>
            rel.child.name.replace(`${rel.parent.name} - `, "");

        this.#relationships = TypedSelect(
            ID(stringRandom(12)),
            Nullable(true),
            Multiple(true),
            ValueField<RelationshipModel>(v => v.id.toFixed()),
            LabelField<RelationshipModel>(relationshipDescription),
            SortKeyField<RelationshipModel>(relationshipDescription)
        );

        this.#addButton = Button(
            OnClick(async () => {
                const entity = await this.#entitySelector.show(this.#allowedEntityTypes);
                if (entity) {
                    this.dispatchEvent(new TypedItemSelectedEvent(entity));
                }
            })
        );

        this.#entitySelector = singleton("Juniper::Cedrus::RelationshipEditorElement::SelectEntityDialog", () => {
            const dialog = SelectEntityDialog();
            document.body.append(dialog);
            return dialog;
        });

        this.#contents = PropertyList(
            PropertyGroup(
                "Relationship Type",
                Label(For(this.#relationshipTypeSelector.id), "Relationship Type"),
                this.#relationshipTypeSelector,
            ),

            PropertyGroup(
                "Default",

                this.#addButton,
                this.#relationships
            )
        );
    }

    #ready = false;

    connectedCallback() {
        if (!this.#ready) {
            this.append(this.#contents);
            this.#render();
        }
    }

    attributeChangedCallback(_name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        this.#render();
    }

    #render() {
        this.#relationshipTypeSelector.selectedItem = this.relationshipType;
        this.#contents.setGroupVisible("Relationship Type", this.relationshipTypes.length > 1);
        elementSetDisplay(this.#addButton, !this.readOnly)
        this.#relationshipTypeSelector.disabled = this.disabled || !this.relationshipType;
        HtmlRender(
            this.#addButton,
            Clear(),
            plus.value,
            this.templateName
        );
    }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(value) { this.toggleAttribute("disabled", value); }

    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(value) { this.toggleAttribute("readonly", value); }


    readonly #relationships: TypedSelectElement<RelationshipModel>;
    get relationships() { return this.#relationships.data; }
    set relationships(v) {
        this.#relationships.data = v;
    }
    get selectedRelationship() {
        return this.#relationships.selectedItem;
    }

    get relationshipId() { return parseFloat(this.getAttribute("relationshipid")); }
    set relationshipId(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("relationshipid");
        }
        else {
            this.setAttribute("relationshipid", v.toFixed());
        }
    }

    get relationshipType() { return arrayScan(this.relationshipTypes, rt => rt.name === this.relationshipTypeName); }
    set relationshipType(v) { this.relationshipTypeName = v?.name; }

    get relationshipTypeName() { return this.getAttribute("relationshiptypename"); }
    set relationshipTypeName(name) {
        if (isNullOrUndefined(name)) {
            this.removeAttribute("relationshiptypename");
        }
        else {
            this.setAttribute("relationshiptypename", name);
        }
    }

    get templateName() { return this.getAttribute("templatename"); }
    set templateName(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("templatename");
        }
        else {
            this.setAttribute("templatename", v);
        }
    }

    static install() {
        return singleton("Juniper::Cedrus::RelationshipEditorElement", () => registerFactory("relationship-editor", RelationshipEditorElement));
    }
}

export function RelationshipEditor(...rest: ElementChild<RelationshipEditorElement>[]) {
    return RelationshipEditorElement.install()(...rest);
}
