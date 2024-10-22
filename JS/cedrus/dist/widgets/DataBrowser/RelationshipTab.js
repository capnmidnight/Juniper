import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, Label, Legend, OnClick, PlaceHolder, Required, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataColumnGroup, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, NameAttr, PropertyList, SortKeyField, TypedInput, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function RelationshipTab() {
    let relationshipType;
    let parentEntity;
    let childEntity;
    let propertyEntity;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumnGroup(NameAttr("Parent"), DataColumn(TypeAttr("text"), FieldAttr("type.parentRole"), TitleAttr("Role")), DataColumn(TypeAttr("text"), FieldAttr("parent.type.name"), TitleAttr("Type")), DataColumn(TypeAttr("text"), FieldAttr("parent.name"), TitleAttr("Name"))), DataColumnGroup(NameAttr("Child"), DataColumn(TypeAttr("text"), FieldAttr("type.childRole"), TitleAttr("Role")), DataColumn(TypeAttr("text"), FieldAttr("child.type.name"), TitleAttr("Type")), DataColumn(TypeAttr("text"), FieldAttr("child.name"), TitleAttr("Name"))), DataColumnGroup(NameAttr("Properties"), DataColumn(TypeAttr("text"), FieldAttr("propertyEntity.type.name"), TitleAttr("Type")), DataColumn(TypeAttr("text"), FieldAttr("propertyEntity.name"), TitleAttr("Name"))), DataColumn(TypeAttr("date"), FieldAttr("createdOn"), TitleAttr("Created")), DataColumn(TypeAttr("text"), FieldAttr("user.name"), TitleAttr("By")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")));
    const props = PropertyList(Label(For("newRelationshipType"), "Relationship Type"), relationshipType = TypedSelect(ID("newRelationshipType"), Required(true), PlaceHolder("Select type..."), TitleAttr("Relationship Type"), ValueField(e => e.name), LabelField(e => e.name), SortKeyField(e => e.name)), Label(For("newRelationshipParentEntity"), "Parent Entity"), parentEntity = TypedInput(ID("newRelationshipParentEntity"), Required(true), PlaceHolder("Select parent..."), TitleAttr("Relationship Parent Entity"), ValueField(e => e.name), LabelField(e => e.name), SortKeyField(e => e.name)), Label(For("newRelationshipChildEntity"), "Child Entity"), childEntity = TypedInput(ID("newRelationshipChildEntity"), Required(true), PlaceHolder("Select child..."), TitleAttr("Relationship Child Entity"), ValueField(e => e.name), LabelField(e => e.name), SortKeyField(e => e.name)), Label(For("newRelationshipPropertyEntity"), "Property Entity (optional)"), propertyEntity = TypedInput(ID("newRelationshipPropertyEntity"), PlaceHolder("Select property detail entity..."), TitleAttr("Relationship Property Entity"), ValueField(e => e.name), LabelField(e => e.name), SortKeyField(e => e.name)));
    const pane = DataBrowserPane("Relationships", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const [relations, entities, relTypes] = await Promise.all([
            ds.getRelationships(),
            ds.getEntities(),
            ds.getRelationshipTypes()
        ]);
        dt.data = relations;
        relationshipType.data = relTypes;
        parentEntity.data = entities;
        childEntity.data = entities;
        propertyEntity.data = entities;
    })), OnDisabled(evt => {
        relationshipType.disabled
            = parentEntity.disabled
                = childEntity.disabled
                    = propertyEntity.disabled
                        = evt.disabled;
    }), dt, FieldSet(Legend("Create new Relationship"), props, Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const relationship = await ds.setRelationship(parentEntity.selectedItem, childEntity.selectedItem, propertyEntity.selectedItem, relationshipType.selectedItem);
        const existing = arrayScan(dt.data, item => item.id === relationship.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(relationship);
    })))));
    dt.on("delete", relationship => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteRelationship(relationship);
        dt.removeItem(relationship);
    }));
    return pane;
}
//# sourceMappingURL=RelationshipTab.js.map