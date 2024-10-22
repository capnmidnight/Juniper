import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, FieldSet, For, ID, IconAttr, InputFile, Label, Legend, Multiple, OnClick, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, PropertyList } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";
export function FileTab() {
    let file;
    const dt = DataTable(ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"), DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("text"), FieldAttr("type"), TitleAttr("Content Type")), DataColumn(TypeAttr("text"), FieldAttr("formattedSize"), TitleAttr("Size")), DataColumn(TypeAttr("link"), FieldAttr("path"), TitleAttr("Path")), DataColumn(TypeAttr("date"), FieldAttr("createdOn"), TitleAttr("Created")), DataColumn(TypeAttr("text"), FieldAttr("user.name"), TitleAttr("By")), DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")));
    const pane = DataBrowserPane("Files", OnShown(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        dt.data = await ds.getFiles();
    })), OnDisabled(evt => {
        file.disabled = evt.disabled;
    }), dt, FieldSet(Legend("Upload new File"), PropertyList(Label(For("newFile"), "File"), file = InputFile(ID("newFile"), Multiple(true))), Button("Save", OnClick(() => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        const fileInfos = await ds.uploadFiles(file.files);
        for (const fileInfo of fileInfos) {
            const existing = arrayScan(dt.data, item => item.id === fileInfo.id);
            if (existing) {
                dt.removeItem(existing);
            }
            dt.addItem(fileInfo);
        }
    })))));
    dt.on("delete", fileInfo => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteFile(fileInfo);
        dt.removeItem(fileInfo);
    }));
    return pane;
}
//# sourceMappingURL=FileTab.js.map