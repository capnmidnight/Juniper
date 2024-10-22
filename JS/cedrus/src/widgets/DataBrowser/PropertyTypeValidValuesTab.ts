import { ClassList, IconAttr, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { PropertyTypeValidValueModel } from "../../models";
import { DataBrowserPane, OnShown } from "./DataBrowserPane";
import { withErrorAlert } from "@juniper-lib/util";


export function PropertyValidValuesTab() {
    const dt = DataTable<PropertyTypeValidValueModel>(
        ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"),
        DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")),
        DataColumn(TypeAttr("text"), FieldAttr("propertyType.name"), TitleAttr("Name")),
        DataColumn(TypeAttr("text"), FieldAttr("propertyType.dataType"), TitleAttr("Type")),
        DataColumn(TypeAttr("text"), FieldAttr("propertyType.unitsCategory"), TitleAttr("Units")),
        DataColumn(TypeAttr("text"), FieldAttr("value"), TitleAttr("Value")),
        DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete"))
    );

    const pane = DataBrowserPane("Property Type Valid Values",
        OnShown(() => withErrorAlert(async () => {
            const ds = await CedrusDataAPI.dataSourceTask;
            dt.data = await ds.getPropertyTypeValidValues();
        })),
        dt
    );

    dt.on("delete", validValue => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deletePropertyTypeValidValue(validValue);
        dt.removeItem(validValue);
    }));

    return pane;
}
