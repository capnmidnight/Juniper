import { arrayScan, withErrorAlert } from "@juniper-lib/util";
import { Button, ClassList, Div, FieldSet, For, ID, IconAttr, InputText, Label, Legend, OnClick, PlaceHolder, Required, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { DataColumn, DataTable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, PropertyList, SortKeyField, TypedSelect, TypedSelectElement, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { EntityModel, EntityTypeModel } from "../../models";
import { EntityManager } from "../EntityManager";
import { DataBrowserPane, OnDisabled, OnShown } from "./DataBrowserPane";

export function EntityTab() {

    let entityMgr: EntityManager;
    let props: HTMLDivElement;
    let entityType: TypedSelectElement<EntityTypeModel>;
    let newEntityName: HTMLInputElement;
    

    const dt = DataTable<EntityModel>(
        ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"),
        DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")),
        DataColumn(TypeAttr("text"), FieldAttr("type.name"), TitleAttr("Type")),
        DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")),
        DataColumn(TypeAttr("date"), FieldAttr("createdOn"), TitleAttr("Created")),
        DataColumn(TypeAttr("text"), FieldAttr("user.name"), TitleAttr("By")),
        DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")),
        OnItemSelected<EntityModel>(evt => withErrorAlert(async () => {
            props.innerHTML = "";

            entityType.selectedItem = evt.item?.type;
            newEntityName.value = evt.item?.name ?? "";
            
            if (evt.item) {
                entityMgr = await EntityManager.load(evt.item);
                entityMgr.connectAt(props, true);
            }
        }))
    );


    const pane = DataBrowserPane("Entities",
        OnShown(() => withErrorAlert(async () => {
            const ds = await CedrusDataAPI.dataSourceTask;
            const [entityTypes, entities] = await Promise.all([
                ds.getEntityTypes(),
                ds.getEntities()
            ]);
            entityType.data = entityTypes;
            dt.data = entities;
        })),
        OnDisabled(evt => {
            entityMgr.disabled
                = entityType.disabled
                = newEntityName.disabled
                = evt.disabled;
        }),
        dt,

        FieldSet(
            Legend("Create new Entity"),
            PropertyList(
                Label(For("newEntityName"), "Name"),
                newEntityName = InputText(
                    ID("newEntityName"),
                    Required(true),
                    PlaceHolder("Enter name..."),
                    TitleAttr("Entity Name")
                ),


                Label(For("newEntityEntityType"), "Entity Type"),
                entityType = TypedSelect<EntityTypeModel>(
                    ID("newEntityEntityType"),
                    Required(true),
                    PlaceHolder("Select type..."),
                    TitleAttr("Entity Type"),
                    LabelField<EntityTypeModel>(et => et.name),
                    ValueField<EntityTypeModel>(et => et.name),
                    SortKeyField<EntityTypeModel>(et => et.name)
                )
            ),

            Button("Save", OnClick(() => withErrorAlert(async () => {
                const ds = await CedrusDataAPI.dataSourceTask;
                const entity = await ds.setEntity(
                    newEntityName.value,
                    entityType.selectedItem
                )
                const existing = arrayScan(dt.data, item => item.id === entity.id);
                if (existing) {
                    dt.removeItem(existing);
                }
                dt.addItem(entity);
            }))),

            props = Div()
        )
    );

    dt.on("delete", entity => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteEntity(entity);
        dt.removeItem(entity);
    }));

    return pane;
}