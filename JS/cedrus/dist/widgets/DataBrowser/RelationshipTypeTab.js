import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputText, Label, Legend, OnClick, PlaceHolder, Required, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, PropertyList } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function RelationshipTypeTab() {
    let newRelationshipTypeParentRole;
    let newRelationshipTypeChildRole;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("parentRole"), TitleAttr("Parent Role")), DataColumn(TypeAttr("text"), FieldAttr("childRole"), TitleAttr("Child Role")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")));
    const pane = DataBrowserPane("Relationship Types", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        dt.data = await ds.getRelationshipTypes();
    })), OnDisabled(evt => {
        newRelationshipTypeParentRole.disabled
            = newRelationshipTypeChildRole.disabled
                = evt.disabled;
    }), dt, FieldSet(Legend("Create new Relationship Type"), PropertyList(Label(For("newRelationshipTypeParentRole"), "Parent Role"), newRelationshipTypeParentRole = InputText(ID("newRelationshipTypeParentRole"), Required(true), PlaceHolder("Enter parent role..."), TitleAttr("Relationship Parent Role Name")), Label(For("newRelationshipTypeChildRole"), "Child Role"), newRelationshipTypeChildRole = InputText(ID("newRelationshipTypeChildRole"), PlaceHolder("Enter child role..."), TitleAttr("Relationship Child Role Name"))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const relationshipType = await ds.setRelationshipType(newRelationshipTypeParentRole.value, newRelationshipTypeChildRole.value
            || newRelationshipTypeParentRole.value);
        const existing = arrayScan(dt.data, item => item.id === relationshipType.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(relationshipType);
    })))));
    dt.on("delete", relationshipType => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteRelationshipType(relationshipType);
        dt.removeItem(relationshipType);
    }));
    return pane;
}
//# sourceMappingURL=RelationshipTypeTab.js.map