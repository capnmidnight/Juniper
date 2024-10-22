import { arrayScan, groupBy, isDefined, makeLookup, withErrorAlert } from "@juniper-lib/util";
import { ClassList, For, ID, IconAttr, Label, Open, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { Cancelable, DataColumn, DataTable, Deletable, EventAttr, FieldAttr, HeaderAttr, LabelField, OnItemSelected, OnUpdated, PropertyList, PropertyListElement, SortKeyField, TypedInput, TypedInputElement, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { EntityModel, PropertyModel } from "../../models";
import { AllowedPropertyTypes, LockPropertyType, PropertyEditor, PropertyEditorElement, PropertyTypes, UnitAbbreviations, Units, ValidValues } from "../PropertyEditorElement";
import { DataBrowserPane, OnShown } from "./DataBrowserPane";


export function PropertyTab() {
    let entity: TypedInputElement<EntityModel>;
    let propertyList: PropertyListElement;
    let propertyEditor: PropertyEditorElement;

    const dt = DataTable<PropertyModel>(
        ClassList("table", "table-striped", "border", "table-borderless", "border-dark", "table-hover"),
        DataColumn(TypeAttr("integer"), FieldAttr("id"), TitleAttr("ID")),
        DataColumn(TypeAttr("text"), FieldAttr("entity.name"), TitleAttr("Entity")),
        DataColumn(TypeAttr("text"), FieldAttr("entity.type.name"), TitleAttr("Entity Type")),
        DataColumn(TypeAttr("text"), FieldAttr("type.name"), TitleAttr("Name")),
        DataColumn(TypeAttr("text"), FieldAttr("type.type"), TitleAttr("Type")),
        DataColumn(TypeAttr("text"), FieldAttr("type.storage"), TitleAttr("Storage")),
        DataColumn(TypeAttr("text"), FieldAttr("type.unitsCategory"), TitleAttr("Units")),
        DataColumn(TypeAttr("text"), FieldAttr("value"), TitleAttr("Value")),
        DataColumn(TypeAttr("date"), FieldAttr("createdOn"), TitleAttr("Created")),
        DataColumn(TypeAttr("text"), FieldAttr("user.name"), TitleAttr("By")),
        DataColumn(TypeAttr("text"), FieldAttr("value"), TitleAttr("Value")),
        DataColumn(TypeAttr("button"), IconAttr("trash"), EventAttr("delete"), HeaderAttr(""), TitleAttr("Delete")),
        OnItemSelected<PropertyModel>(evt => {
            entity.selectedItem = evt.item?.entity;

            propertyEditor.open = !evt.item;
            propertyEditor.property = evt.item;
            propertyEditor.lockPropertyType
                = propertyEditor.cancelable
                = isDefined(evt.item);
        })
    );

    const pane = DataBrowserPane("Properties",
        OnShown(() => withErrorAlert(async () => {
            const ds = await CedrusDataAPI.dataSourceTask;
            dt.data = await ds.getProperties();

            const [propertyTypes, validValues, units, abbreviations, entities] = await Promise.all([
                ds.getPropertyTypes(),
                ds.getPropertyTypeValidValues(),
                ds.getUnitsData(),
                ds.getUnitAbbreviations(),
                ds.getEntities()
            ]);

            entity.data = entities;

            const propertyTypesByName = makeLookup(propertyTypes, pt => pt.name);
            const validValuesByPropertyTypeID = groupBy(validValues, vv => vv.propertyType.id, vv => vv.value);

            propertyEditor?.remove();
            propertyList.append(propertyEditor = PropertyEditor(
                Open(true),
                LockPropertyType(false),
                Cancelable(false),
                Deletable(false),
                PropertyTypes(propertyTypesByName),
                ValidValues(validValuesByPropertyTypeID),
                Units(units),
                UnitAbbreviations(abbreviations),
                AllowedPropertyTypes(propertyTypes),
                OnUpdated<PropertyEditorElement>(async evt => {
                    const editor = evt.target;
                    editor.saveProperty(ds, entity.selectedItem, false);
                    const existing = arrayScan(dt.data, item => editor.property && item.id === editor.property.id);
                    if (existing) {
                        dt.removeItem(existing);
                    }
                    dt.addItem(editor.property);
                })
            ));
        })),
        dt,
        propertyList = PropertyList(
            Label(For("newPropertyEntity"), "Entity"),
            entity = TypedInput<EntityModel>(
                ID("newPropertyEntity"),
                ValueField<EntityModel>(v => v.name),
                LabelField<EntityModel>(v => v.name),
                SortKeyField<EntityModel>(v => v.name)
            )
        )
    );

    dt.on("delete", property => withErrorAlert(async () => {
        const ds = await CedrusDataAPI.dataSourceTask;
        await ds.deleteProperty(property);
        dt.removeItem(property);
    }));

    return pane;
}
