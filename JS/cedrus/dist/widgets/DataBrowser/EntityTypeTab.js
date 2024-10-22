import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputCheckbox, InputText, Label, Legend, OnClick, PlaceHolder, Required, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, Nullable, OnItemSelected, PropertyList, SortKeyField, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function EntityTypeTab() {
    let parentEntityType;
    let isPrimary;
    let newEntityTypeName;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("boolean"), FieldAttr("isPrimary"), TitleAttr("Is Primary")), DataColumn(TypeAttr("text"), FieldAttr("parent.name"), TitleAttr("Parent")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected(evt => {
        parentEntityType.selectedItem = evt.item?.parent;
        isPrimary.checked = evt.item?.isPrimary;
        newEntityTypeName.value = evt.item?.name ?? "";
    }));
    const pane = DataBrowserPane("Entity Types", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        parentEntityType.data
            = dt.data
                = await ds.getEntityTypes();
    })), OnDisabled(evt => {
        newEntityTypeName.disabled
            = parentEntityType.disabled
                = isPrimary.disabled
                    = evt.disabled;
    }), dt, FieldSet(Legend("Create new Entity Type"), PropertyList(Label(For("newEntityTypeName"), "Name"), newEntityTypeName = InputText(ID("newEntityTypeName"), Required(true), PlaceHolder("Enter name..."), TitleAttr("Entity Type Name")), Label(For("newEntityTypeIsPrimary"), "Is Primary"), isPrimary = InputCheckbox(ID("newEntityTypeIsPrimary"), TitleAttr("Show Entity Type in Main Menu")), Label(For("newEntityTypeParent"), "Parent Entity Type"), parentEntityType = TypedSelect(ID("newEntityTypeParent"), PlaceHolder("Select parent type..."), TitleAttr("Entity Type Parent"), Nullable(true), LabelField(et => et.name), ValueField(et => et.name), SortKeyField(et => et.name))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const entityType = await ds.setEntityType({
            name: newEntityTypeName.value,
            isPrimary: isPrimary.checked,
            parentEntityType: parentEntityType.selectedItem
        });
        const existing = arrayScan(dt.data, item => item.id === entityType.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(entityType);
    })))));
    dt.on("delete", entityType => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteEntityType(entityType);
        dt.removeItem(entityType);
    }));
    return pane;
}
//# sourceMappingURL=EntityTypeTab.js.map