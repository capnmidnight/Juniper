import { arrayScan, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { Button, Clear, For, HtmlRender, ID, Label, Multiple, OnClick, PlaceHolder, SingletonStyleBlob, TitleAttr, TypedHTMLElement, TypedHtmlProp, columnGap, content, display, elementSetDisplay, em, flexDirection, fontWeight, height, maxHeight, px, registerFactory, rule, vh } from "@juniper-lib/dom";
import { plus } from "@juniper-lib/emoji";
import { LabelField, Nullable, OnItemSelected, PropertyGroup, PropertyList, SortKeyField, TypedItemSelectedEvent, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { SelectEntityDialog } from "./SelectEntityDialog";
export function AllowedEntityTypes(entityTypes) {
    return TypedHtmlProp("allowedEntityTypes", entityTypes);
}
export function RelationshipTypes(relationshipTypes) {
    return TypedHtmlProp("relationshipTypes", relationshipTypes);
}
export function RelationshipType(relationshipType) {
    return TypedHtmlProp("relationshipType", relationshipType);
}
export function Relationships(relationships) {
    return TypedHtmlProp("relationships", relationships);
}
export class RelationshipEditorElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "disabled",
        "relationshipid",
        "relationshiptypename",
        "readonly"
    ]; }
    #relationshipTypeSelector;
    #contents;
    get relationshipTypes() { return this.#relationshipTypeSelector.data; }
    set relationshipTypes(v) {
        this.#relationshipTypeSelector.data = v;
        if (v.length === 1) {
            this.relationshipType = v[0];
        }
        this.#render();
    }
    #allowedEntityTypes;
    get allowedEntityTypes() { return this.#allowedEntityTypes; }
    set allowedEntityTypes(v) { this.#allowedEntityTypes = v; }
    #addButton;
    #entitySelector;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Cedrus::RelationshipEditorElement", () => rule("relationship-editor", rule("> div", display("flex"), flexDirection("row"), columnGap(em(.25))), rule("> label", fontWeight("bold"), rule("::after", content("':'"))), rule(" select[multiple]", height(em(10))), rule(" select:not([multiple])", height(px(26))), rule(" img", maxHeight(vh(10)))));
        this.#relationshipTypeSelector = TypedSelect(ID(stringRandom(12)), ValueField(v => v.id.toFixed()), LabelField(v => v.name), SortKeyField(v => v.name), PlaceHolder("Select type..."), TitleAttr("Relationship Type"), OnItemSelected((evt) => {
            this.relationshipType = evt.item;
        }));
        const relationshipDescription = (rel) => rel.child.name.replace(`${rel.parent.name} - `, "");
        this.#relationships = TypedSelect(ID(stringRandom(12)), Nullable(true), Multiple(true), ValueField(v => v.id.toFixed()), LabelField(relationshipDescription), SortKeyField(relationshipDescription));
        this.#addButton = Button(OnClick(async () => {
            const entity = await this.#entitySelector.show(this.#allowedEntityTypes);
            if (entity) {
                this.dispatchEvent(new TypedItemSelectedEvent(entity));
            }
        }));
        this.#entitySelector = singleton("Juniper::Cedrus::RelationshipEditorElement::SelectEntityDialog", () => {
            const dialog = SelectEntityDialog();
            document.body.append(dialog);
            return dialog;
        });
        this.#contents = PropertyList(PropertyGroup("Relationship Type", Label(For(this.#relationshipTypeSelector.id), "Relationship Type"), this.#relationshipTypeSelector), PropertyGroup("Default", this.#addButton, this.#relationships));
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.append(this.#contents);
            this.#render();
        }
    }
    attributeChangedCallback(_name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        this.#render();
    }
    #render() {
        this.#relationshipTypeSelector.selectedItem = this.relationshipType;
        this.#contents.setGroupVisible("Relationship Type", this.relationshipTypes.length > 1);
        elementSetDisplay(this.#addButton, !this.readOnly);
        this.#relationshipTypeSelector.disabled = this.disabled || !this.relationshipType;
        HtmlRender(this.#addButton, Clear(), plus.value, this.templateName);
    }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(value) { this.toggleAttribute("disabled", value); }
    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(value) { this.toggleAttribute("readonly", value); }
    #relationships;
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
export function RelationshipEditor(...rest) {
    return RelationshipEditorElement.install()(...rest);
}
//# sourceMappingURL=RelationshipEditorElement.js.map