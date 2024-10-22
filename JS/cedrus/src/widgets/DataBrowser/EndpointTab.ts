import { withErrorAlert } from "@juniper-lib/util";
import { ClassList, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, FieldAttr } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnShown } from "./DataBrowserPane";


export function EndpointTab() {
    const dt = DataTable<object>(
        ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"),
        DataColumn(TypeAttr("text"), FieldAttr("methods"), TitleAttr("Method")),
        DataColumn(TypeAttr("text"), FieldAttr("route"), TitleAttr("Route")),
        DataColumn(TypeAttr("text"), FieldAttr("action"), TitleAttr("Action"))
    );

    const pane = DataBrowserPane("Endpoints",
        OnShown(() => withErrorAlert(async () => {
            const ds = await CedrusDataAPI.dataSourceTask;
            dt.data = await ds.getEndpoints();
        })),
        dt
    );

    return pane;
}
