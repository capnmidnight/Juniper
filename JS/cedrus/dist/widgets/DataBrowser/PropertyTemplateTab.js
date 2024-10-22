import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputText, Label, Legend, OnClick, PlaceHolder, Required, SlotAttr, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { AllowDuplicates, ArrayEditor, DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, PropertyList, SortKeyField, TypedInput, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function PropertyTemplateTab() {
    let name;
    let entityType;
    let propertyTypes;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("entityType.name"), TitleAttr("Entity Type")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected((evt) => {
        name.value = evt.item?.name ?? "";
        entityType.selectedItem = evt.item?.entityType;
        propertyTypes.values = evt.item?.propertyTypes;
    }));
    const pane = DataBrowserPane("Property Templates", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const [templates, ets, pts] = await Promise.all([
            ds.getPropertyTemplates(),
            ds.getEntityTypes(),
            ds.getPropertyTypes()
        ]);
        dt.data = templates;
        entityType.data = ets;
        propertyTypes.data = pts;
    })), OnDisabled(evt => {
        name.disabled
            = entityType.disabled
                = propertyTypes.disabled
                    = evt.disabled;
    }), dt, FieldSet(Legend("Create new Property Template"), PropertyList(Label(For("newPropertyTemplateName"), "Name"), name = InputText(ID("newPropertyTemplateName"), Required(true), PlaceHolder("Enter name..."), TitleAttr("Property Template Name")), Label(For("newPropertyTemplateEntityType"), "Entity Type"), entityType = TypedInput(ID("newPropertyTemplateEntityType"), Required(true), PlaceHolder("Select entity type..."), TitleAttr("Property Template Entity Type"), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name)), Label(For("newPropertyTemplatePropertyTypes"), "Property Types"), propertyTypes = ArrayEditor(TypedInput(ID("newPropertyTemplatePropertyTypes"), SlotAttr("selector"), PlaceHolder("Select property type..."), TitleAttr("Property Template Allowed Property Type")), AllowDuplicates(false), LabelField(pt => pt.name), ValueField(pt => pt.name), SortKeyField(pt => pt.name))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const template = await ds.setPropertyTemplate(name.value, entityType.selectedItem, ...propertyTypes.values);
        const existing = arrayScan(dt.data, item => item.id === template.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(template);
    })))));
    dt.on("delete", template => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deletePropertyTemplate(template);
        dt.removeItem(template);
    }));
    return pane;
}
//# sourceMappingURL=PropertyTemplateTab.js.map