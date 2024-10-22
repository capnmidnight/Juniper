import { arrayScan, identity, isNullOrUndefined, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, IconAttr, ID, InputText, Label, Legend, OnClick, OnInput, PlaceHolder, Required, TextArea, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataAttr, DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, OnItemSelected, PropertyList, SortKeyField, TypedInput, TypedSelect } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataTypeValues, storageTypesByDataType } from "../../models";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function PropertyTypeTab() {
    let name;
    let description;
    let dataTypeInput;
    let storageTypeInput;
    let unitsCategory;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("text"), FieldAttr("type"), TitleAttr("Type")), DataColumn(TypeAttr("text"), FieldAttr("storage"), TitleAttr("Storage")), DataColumn(TypeAttr("text"), FieldAttr("unitsCategory"), TitleAttr("Units")), DataColumn(TypeAttr("text"), FieldAttr("description"), TitleAttr("Description")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")), OnItemSelected(evt => {
        name.value = evt.item?.name ?? "";
        description.value = evt.item?.description ?? "";
        dataTypeInput.selectedItem = evt.item?.type;
        setStorageTypes(evt.item?.type, evt.item?.storage);
        unitsCategory.selectedItem = evt.item?.unitsCategory;
    }));
    const pane = DataBrowserPane("Property Types", OnShown(() => withErrorAlert(async () => {
        storageTypeInput.data = [];
        storageTypeInput.disabled = true;
        const ds = await CedrusDataAPI.dataSourceTask;
        dt.data = await ds.getPropertyTypes();
        unitsCategory.data = Array.from((await ds.getUnitsData()).keys());
    })), OnDisabled(evt => {
        name.disabled
            = description.disabled
                = dataTypeInput.disabled
                    = unitsCategory.disabled
                        = evt.disabled;
    }), dt, FieldSet(Legend("Create new Property Type"), PropertyList(Label(For("newPropertyTypeName"), "Name"), name = InputText(ID("newPropertyTypeName"), Required(true), PlaceHolder("Enter name..."), TitleAttr("Property Type Name")), Label(For("newPropertyTypeDescription"), "Description"), description = TextArea(ID("newPropertyTypeDescription"), Required(true), PlaceHolder("Enter description..."), TitleAttr("Property Type Description")), Label(For("newPropertyTypeDataType"), "Data Type"), dataTypeInput = TypedSelect(ID("newPropertyTypeDataType"), DataAttr(DataTypeValues.filter(v => storageTypesByDataType.has(v) && storageTypesByDataType.get(v).length > 0)), Required(true), PlaceHolder("Select data type..."), TitleAttr("Property Type Data Type"), OnInput(() => setStorageTypes(dataTypeInput.selectedItem, storageTypeInput.selectedItem))), Label(For("newPropertyTypeStorageType"), "Storage Type"), storageTypeInput = TypedSelect(ID("newPropertyTypeStorageType"), Required(true), PlaceHolder("Select storage type..."), TitleAttr("Property Type Storage Type")), Label(For("newPropertyTypeUnitsCategory"), "Units Category"), unitsCategory = TypedInput(ID("newPropertyTypeUnitsCategory"), SortKeyField(identity), Required(true), PlaceHolder("Select units category..."), TitleAttr("Property Type Units Category"))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const propertyType = await ds.setPropertyType(name.value, dataTypeInput.selectedItem, storageTypeInput.selectedItem, unitsCategory.selectedItem, description.value);
        const existing = arrayScan(dt.data, item => item.id === propertyType.id);
        if (existing) {
            dt.removeItem(existing);
        }
        dt.addItem(propertyType);
    })))));
    dt.on("delete", propertyType => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deletePropertyType(propertyType);
        dt.removeItem(propertyType);
    }));
    function setStorageTypes(dataType, storageType) {
        storageTypeInput.data = storageTypesByDataType.get(dataType) ?? [];
        storageTypeInput.disabled = storageTypeInput.data.length <= 1;
        storageTypeInput.selectedItem = null;
        if (storageTypeInput.data.indexOf(storageType) >= 0) {
            storageTypeInput.selectedItem = storageType;
        }
        if (storageTypeInput.data.length === 1 && isNullOrUndefined(storageTypeInput.selectedItem)) {
            storageTypeInput.selectedItem = storageTypeInput.data[0];
        }
    }
    return pane;
}
//# sourceMappingURL=PropertyTypeTab.js.map