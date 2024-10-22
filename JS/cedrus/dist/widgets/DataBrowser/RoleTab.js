import { arrayScan, compareBy, distinct, isDefined, makeLookup, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputText, Label, Legend, OnClick, PlaceHolder, Required, SlotAttr, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { ArrayEditor, DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, PropertyList, SortKeyField, TypedInput, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnShown } from "./DataBrowserPane";
export function RoleTab() {
    let roleName;
    let users;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected(evt => withErrorAlert(async () => {
        roleName.value = evt.item?.name ?? "";
        const ds = await CedrusDataAPI.dataSourceTask;
        if (isDefined(evt.item)) {
            users.values = (await ds.getRoleUsers(evt?.item))
                .map(ur => ur.user);
        }
        else {
            users.values = [];
        }
    })));
    const pane = DataBrowserPane("Roles", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        dt.data = await ds.getRoles();
        users.data = await ds.getUsers();
    })), dt, FieldSet(Legend("Create new Role"), PropertyList(Label(For("newRoleName"), "Name"), roleName = InputText(ID("newRoleName"), Required(true), PlaceHolder("Enter name..."), TitleAttr("Role Name")), Label(For("newRoleUsers"), "Users"), users = ArrayEditor(TypedInput(ID("newRoleUsers"), SlotAttr("selector"), PlaceHolder("Select user..."), TitleAttr("User in Role")), LabelField(v => v.name), ValueField(v => v.name), SortKeyField(v => v.name))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const role = await ds.createRole(roleName.value);
        const existing = arrayScan(dt.data, item => item.id === role.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(role);
        const current = (await ds.getRoleUsers(role))
            .map(ur => ur.user);
        const oldIDs = current.map(v => v.id);
        const newIDs = users.values.map(v => v.id);
        const allUsers = makeLookup(distinct([
            ...current,
            ...users.values
        ], compareBy(v => v.id)), v => v.id);
        const toRemove = oldIDs
            .filter(id => newIDs.indexOf(id) === -1)
            .map(id => allUsers.get(id));
        const toAdd = newIDs
            .filter(id => oldIDs.indexOf(id) === -1)
            .map(id => allUsers.get(id));
        for (const user of toRemove) {
            await ds.removeUserFromRole(user, role);
        }
        for (const user of toAdd) {
            await ds.addUserToRole(user, role);
        }
    })))));
    dt.on("delete", role => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteRole(role);
        dt.removeItem(role);
    }));
    return pane;
}
//# sourceMappingURL=RoleTab.js.map