import { isGoodNumber, toString } from "@juniper-lib/util";
import { ENTITY_TYPE, EntityTypeModel, PropertyTemplateModel, PropertyTypeModel, RelationshipTemplateModel } from "@juniper-lib/cedrus";
import { Button, HistoryManager, I, ID, InputCheckbox, Label, OnClick, Open, Query, SlotAttr, SpanTag, TitleAttr, Type, Value } from "@juniper-lib/dom";
import { DataTable, LabelField, NavTree, NavTreeItem, NavTreeItemElement, OnItemSelected, Selectable, SortKeyField, TypedSelect, ValueField } from "@juniper-lib/widgets";
import { DataAPI } from "../../Models/DataAPI";

import "./index.css";

const ds = new DataAPI();


const history = new HistoryManager<number>({
    pathPattern: /templates\/(\d+)/,
    stateDeserializer: parseFloat,
    stateSerializer: toString
});

const navTree = NavTree(
    Query("nav-tree"),
    OnItemSelected<NavTreeItemElement, NavTreeItemElement | HTMLButtonElement>(async evt => {
        if (evt.target instanceof HTMLButtonElement) {
            navTree.clearSelection();
            let retry = true;
            while (retry) {
                const entityTypeName = prompt("Please provide a name for the new Entity Type:");
                if (!entityTypeName) {
                    retry = false;
                }
                else if (navTree.hasItemByLabel("entityType", entityTypeName)) {
                    alert(`Entity type "${entityTypeName}"" already exists. Please enter another.`)
                }
                else {
                    retry = false;
                    let parentEntityType: ENTITY_TYPE = null;
                    const closestItem = evt.target.closest<NavTreeItemElement>("nav-tree-item[type='entityType']");
                    if (closestItem) {
                        parentEntityType = ENTITY_TYPE(parseFloat(closestItem.value));
                    }

                    const entityType = await ds.setEntityType({
                        name: entityTypeName,
                        isPrimary: true,
                        parentEntityType
                    });

                    addEntityType(entityType);
                }
            }
        }
        else {
            const entityTypeId = parseFloat(evt.item.value);
            loadEntityType(entityTypeId);
        }
    })
);

const entityTypeName = SpanTag(ID("entityTypeName", true));
const entityTypeIsPrimary = InputCheckbox(ID("entityTypeIsPrimary", true));
const entityTypeParent = TypedSelect<EntityTypeModel>(
    ID("entityTypeParent", true),
    ValueField<EntityTypeModel>(x => x.id.toFixed()),
    LabelField<EntityTypeModel>(x => x.name),
    SortKeyField<EntityTypeModel>(x => x.name)
);

const propertyTemplatesList = DataTable<PropertyTemplateModel>(ID("propertyTemplatesList", true),
    OnItemSelected<PropertyTemplateModel>(evt => propertiesList.data = evt.item?.propertyTypes ?? [])
);

const propertiesList = DataTable<PropertyTypeModel>(ID("propertiesList", true));

const relationshipTemplatesList = DataTable<RelationshipTemplateModel>(
    ID("relationshipTemplatesList", true),
    OnItemSelected<RelationshipTemplateModel>(evt => relationshipsList.data = evt.item?.allowedEntityTypes ?? [])
);
const relationshipsList = DataTable<EntityTypeModel>(ID("relationshipsList", true));


const saveEntityType = Button(
    ID("saveEntityType", true),
    OnClick(async () => {
        if (currentEntityType) {
            const updatedEntityType = await ds.setEntityType({
                name: currentEntityType.name,
                isPrimary: entityTypeIsPrimary.checked,
                parentEntityType: entityTypeParent.selectedItem
            });

            addEntityType(updatedEntityType);
        }
    })
);

const deleteEntityType = Button(
    ID("deleteEntityType", true),
    OnClick(async () => {
        if (currentEntityType) {
            const curItem = navTree.getItem("entityType", currentEntityType.id);
            if (curItem) {
                curItem.remove();
            }

            await ds.deleteEntityType(currentEntityType);
            navTree.clearSelection();
            saveEntityType.disabled = true;
            deleteEntityType.disabled = true;
        }
    })
);

let lastLoadEntityType = Promise.resolve(0);
let currentEntityType: EntityTypeModel = null;

history.addEventListener("statechanged", async evt => {
    history.pause();
    loadEntityType(evt.state);
});

history.start();

entityTypeParent.data = await ds.getEntityTypes();

function addEntityType(entityType: EntityTypeModel) {
    const curItem = navTree.getItem("entityType", entityType.id);
    const curChildren = !curItem
        ? []
        : Array.from(curItem.children)
            .filter(child => child instanceof NavTreeItemElement);
    if (curItem) {
        curItem.remove();
    }

    let parentElement: HTMLElement = navTree;
    if (entityType.parent) {
        const navTreeItem = navTree.getItem("entityType", entityType.parent.id);
        navTreeItem.open = true;
        parentElement = navTreeItem;
    }

    const element = NavTreeItem(
        Type("entityType"),
        Value(entityType.id),
        Selectable(true),
        Open(true),
        Label(entityType.name),
        Button(
            SlotAttr("controls"),
            I("+"),
            TitleAttr(`Add entity type derived from ${entityType.name}...`)
        ),
        ...curChildren
    );
    parentElement.append(element);
    navTree.clearSelection();
    loadEntityType(null);
    navTree.select("entityType", entityType.id);
    element.scrollIntoView();
}

function loadEntityType(entityTypeId: number) {
    lastLoadEntityType = lastLoadEntityType.then(async lastEntityTypeId => {
        if (entityTypeId !== lastEntityTypeId
            && isGoodNumber(entityTypeId)) {
            navTree.select("entityType", entityTypeId);
            history.push(entityTypeId);
            history.resume();

            currentEntityType = await ds.getEntityType(entityTypeId);
            if (currentEntityType) {
                const [propertyTemplates, relationshipTemplates] = await Promise.all([
                    ds.getPropertyTemplates(currentEntityType),
                    ds.getRelationshipTemplates(currentEntityType)
                ]);

                saveEntityType.disabled = false;
                deleteEntityType.disabled = false;

                entityTypeName.replaceChildren(currentEntityType.name);
                entityTypeIsPrimary.checked = currentEntityType.isPrimary;
                entityTypeParent.selectedItem = currentEntityType.parent;

                propertyTemplatesList.data = propertyTemplates;
                propertiesList.data = [];

                relationshipTemplatesList.data = relationshipTemplates;

                console.log({
                    currentEntityType,
                    propertyTemplates,
                    relationshipTemplates
                })
            }
        }
        return entityTypeId;
    });
}