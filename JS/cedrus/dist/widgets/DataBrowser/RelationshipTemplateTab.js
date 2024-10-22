import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputText, Label, Legend, OnClick, PlaceHolder, Required, SlotAttr, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { AllowDuplicates, ArrayEditor, DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, PropertyList, SortKeyField, TypedInput, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function RelationshipTemplateTab() {
    let name;
    let parentEntityType;
    let propertyEntityType;
    let relationshipType;
    let childEntityTypes;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("entityType.name"), TitleAttr("Entity Type")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("text"), FieldAttr("relationshipType.name"), TitleAttr("Relationship Type")), DataColumn(TypeAttr("text"), FieldAttr("propertyEntityType.name"), TitleAttr("Property Entity Type")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected((evt) => {
        name.value = evt.item?.name ?? "";
        parentEntityType.selectedItem = evt.item?.entityType;
        propertyEntityType.selectedItem = evt.item?.propertyEntityType;
        relationshipType.selectedItem = evt.item?.relationshipType;
        childEntityTypes.values = evt.item?.allowedEntityTypes;
    }));
    const pane = DataBrowserPane("Relationship Templates", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const [templates, ets, rts] = await Promise.all([
            ds.getRelationshipTemplates(),
            ds.getEntityTypes(),
            ds.getRelationshipTypes()
        ]);
        dt.data = templates;
        parentEntityType.data = ets;
        propertyEntityType.data = ets;
        childEntityTypes.data = ets;
        relationshipType.data = rts;
    })), OnDisabled(evt => {
        name.disabled
            = parentEntityType.disabled
                = propertyEntityType.disabled
                    = relationshipType.disabled
                        = childEntityTypes.disabled
                            = evt.disabled;
    }), dt, FieldSet(Legend("Create new Relationship Template"), PropertyList(Label(For("newRelationshipTemplateName"), "Name"), name = InputText(ID("newRelationshipTemplateName"), Required(true), PlaceHolder("Enter name..."), TitleAttr("Relationship Template Name")), Label(For("newRelationshipTemplateEntityType"), "Entity Type"), parentEntityType = TypedInput(ID("newRelationshipTemplateEntityType"), Required(true), PlaceHolder("Select entity type..."), TitleAttr("Relationship Template Entity Type"), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name)), Label(For("newRelationshipTemplateRelationshipType"), "Relationship Type"), relationshipType = TypedSelect(ID("newRelationshipTemplateRelationshipType"), Required(true), PlaceHolder("Select type..."), TitleAttr("Relationship Template Relationship Type"), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name)), Label(For("newRelationshipTemplatePropertyEntityType"), "Property Entity Type (Optional)"), propertyEntityType = TypedInput(ID("newRelationshipTemplatePropertyEntityType"), PlaceHolder("Select property detail entity type..."), TitleAttr("Relationship Template Property Detail Entity Type"), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name)), Label(For("newRelationshipTemplateChildEntityTypes"), "Allowed Entity Types"), childEntityTypes = ArrayEditor(TypedInput(ID("newRelationshipTemplateChildEntityTypes"), SlotAttr("selector"), PlaceHolder("Select allowed entity type..."), TitleAttr("Relationship Template Allowed Entity Types")), AllowDuplicates(false), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const template = await ds.setRelationshipTemplate(name.value, parentEntityType.selectedItem, relationshipType.selectedItem, propertyEntityType.selectedItem, ...childEntityTypes.values);
        const existing = arrayScan(dt.data, item => item.id === template.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(template);
    })))));
    dt.on("delete", template => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteRelationshipTemplate(template);
        dt.removeItem(template);
    }));
    return pane;
}
//# sourceMappingURL=RelationshipTemplateTab.js.map