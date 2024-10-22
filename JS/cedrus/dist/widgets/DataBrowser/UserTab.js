import { makeLookup, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, Disabled, FieldSet, For, ID, IconAttr, InputEmail, InputText, Label, Legend, OnClick, PlaceHolder, Required, SlotAttr, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { ArrayEditor, DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, PropertyList, SortKeyField, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnShown } from "./DataBrowserPane";
export function UserTab() {
    let userNameInput;
    let emailInput;
    let rolesSelection;
    let saveButton;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("text"), FieldAttr("email"), TitleAttr("Email")), DataColumn(TypeAttr("csv"), FieldAttr("roles"), TitleAttr("Roles")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected(evt => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        if (!evt.item) {
            userNameInput.value = "";
            emailInput.value = "";
            rolesSelection.values = [];
        }
        else {
            userNameInput.value = evt?.item.name;
            emailInput.value = evt?.item.email;
            rolesSelection.values = (await ds.getUserRoles(evt?.item))
                .map(ur => ur.role);
        }
    })));
    const props = PropertyList(Label(For("newUserName"), "Name"), userNameInput = InputText(ID("newUserName"), PlaceHolder("Enter name..."), TitleAttr("User Name"), Required(true)), Label(For("newUserEmail"), "Email"), emailInput = InputEmail(ID("newUserEmail"), PlaceHolder("user@domain.com"), TitleAttr("User Email Address"), Required(true)), Label(For("newUserRoles"), "Roles"), rolesSelection = ArrayEditor(TypedSelect(ID("newUserRoles"), SlotAttr("selector")), LabelField(v => v.name), ValueField(v => v.name), SortKeyField(v => v.name)));
    const pane = DataBrowserPane("Users", OnShown(() => withErrorAlert(async () => {
        saveButton.disabled = true;
        const ds = await CedrusDataAPI.dataSourceTask;
        const [users, roles] = await Promise.all([
            ds.getUsers(),
            ds.getRoles()
        ]);
        dt.data = users;
        rolesSelection.data = roles;
        rolesSelection.values = [];
        saveButton.disabled = false;
    })), dt, FieldSet(Legend("Manage User"), props, saveButton = Button("Save", Disabled(true), OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        let user = dt.selectedValue;
        let needsUpdate = false;
        if (!user) {
            user = await ds.setUser(userNameInput.value, emailInput.value);
            dt.addItem(user);
        }
        else if (emailInput.value !== user.email
            || userNameInput.value !== user.name) {
            await ds.setUser(userNameInput.value, emailInput.value);
            user.email = emailInput.value;
            user.name = userNameInput.value;
            needsUpdate = true;
        }
        const currentRoles = (await ds.getUserRoles(user))
            .map(ur => ur.role);
        const allRoles = makeLookup(rolesSelection.data, v => v.id);
        const oldIDs = currentRoles.map(v => v.id);
        const newIDs = rolesSelection.values.map(v => v.id);
        const toRemoveIDs = oldIDs.filter(id => newIDs.indexOf(id) === -1);
        const toAddIDs = newIDs.filter(id => oldIDs.indexOf(id) === -1);
        const toRemove = toRemoveIDs.map(id => allRoles.get(id));
        const toAdd = toAddIDs.map(id => allRoles.get(id));
        const finalSet = newIDs.map(id => allRoles.get(id));
        if (toRemove.length + toAdd.length > 0) {
            for (const role of toRemove) {
                await ds.removeUserFromRole(user, role);
            }
            for (const role of toAdd) {
                await ds.addUserToRole(user, role);
            }
            rolesSelection.values = finalSet;
            user.roles = finalSet.map(r => r.name);
            needsUpdate = true;
        }
        if (needsUpdate) {
            dt.update(user);
        }
    })))));
    dt.on("delete", user => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteUser(user);
        dt.removeItem(user);
    }));
    return pane;
}
//# sourceMappingURL=UserTab.js.map