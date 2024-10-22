import { isGoodNumber, toString } from "@juniper-lib/util";
import { ENTITY_TYPE, EntityManager, EntityModel, RelationshipEditorElement, isRelationshipModel } from "@juniper-lib/cedrus";
import { ClassName, Div, HistoryManager, ID, Img, Label, Query, Type, Value } from "@juniper-lib/dom";
import { NavTree, NavTreeItem, NavTreeItemElement, OnItemSelected, PropertyListElement, Selectable, TabPanel } from "@juniper-lib/widgets";
import { getCurrentUserInfo } from "../../getCurrentUserInfo";
import { DataAPI } from "../../Models/DataAPI";
import "./index.css";

const ds = new DataAPI();

const currentUser = getCurrentUserInfo();
const isContributor = currentUser.roles.has("Contributor");
const defaultImage = Img(ID("entityImage", true));
const tabs = TabPanel(ID("objectPropertiesTabs"));
const dimensionsTab = tabs.getTab("Dimensions/Capabilities");
const otherTab = tabs.getTab("Other");
const history = new HistoryManager<number>({
    pathPattern: /entities\/(\d+)/,
    stateDeserializer: parseFloat,
    stateSerializer: toString
});
const navTree = NavTree(
    Query("nav-tree"),
    OnItemSelected<NavTreeItemElement, NavTreeItemElement | HTMLButtonElement>(async evt => {
        if(evt.target instanceof HTMLButtonElement) {
            navTree.clearSelection();
            let retry = true;
            while(retry) {
                const entityName = prompt("Please provide a name for the new Entity:");
                if(!entityName) {
                    retry = false;
                }
                else if(navTree.hasItemByLabel("entity", entityName)) {
                    alert(`Entity "${entityName}" already exists. Please enter another.`);
                }
                else {
                    retry = false;
                    const closestItem = evt.target.closest<NavTreeItemElement>("nav-tree-item[type='entityType']");
                    if(closestItem) {
                        const entityType = ENTITY_TYPE(parseFloat(closestItem.value));
                        const entity = await ds.setEntity(entityName, entityType);
                        addEntity(entity);
                    }
                }
            }
        }
        else if(evt.item?.type === "entity") {
            const entityId = parseFloat(evt.item.value);
            loadEntity(entityId);
        }
    })
);

let lastLoadEntity = Promise.resolve(0);
let lastImage: HTMLElement = defaultImage;

tabs.disabled = true;

history.addEventListener("statechanged", evt => {
    if (isGoodNumber(evt.state)) {
        history.pause();
        loadEntity(evt.state);
    }
});

history.start();

function addEntity(entity: EntityModel) {
    const element = NavTreeItem(
        Type("entity"),
        Value(entity.id),
        Selectable(true),
        Label(entity.name)
    );

    const parentElement = navTree.getItem("entityType", entity.type.id);
    parentElement.open = true;
    parentElement.append(element);

    navTree.select("entity", entity.id);
    element.scrollIntoView();
}

function resetView() {

    tabs.disabled = true;

    for (const propList of document.querySelectorAll<PropertyListElement>("property-list")) {
        propList.innerHTML = "";
    }

    for (const element of document.querySelectorAll(".selector.selected")) {
        element.classList.remove("selected");
    }

    if (lastImage !== defaultImage) {
        lastImage.replaceWith(defaultImage);
        lastImage = defaultImage;
    }

    for (const tabName of tabs.tabNames) {
        const tab = tabs.getTab(tabName);
        const propLists = tab.querySelectorAll("property-list[id]");
        tab.innerHTML = "";
        tab.append(...propLists);
        tabs.setTabVisible(tabName, false);
    }
}


function loadEntity(entityId: number) {
    lastLoadEntity = lastLoadEntity.then(async lastEntityId => {
        if (entityId !== lastEntityId && isGoodNumber(entityId)) {
            resetView();

            navTree.select("entity", entityId);
            history.push(entityId);
            history.resume();

            const entityMgr = await EntityManager.load(entityId, {
                readOnly: !isContributor
            });

            const consumedTemplates = new Set<string>();
            if (entityMgr.hasPropertyGroup("Other")) {
                otherTab.append(entityMgr.createPropertyListing("Other"));
                consumedTemplates.add("Other");
            }

            for (const propList of document.querySelectorAll<PropertyListElement>("property-list[data-template-name]")) {
                const templateName = propList.dataset.templateName;
                if (entityMgr.hasPropertyGroup(templateName)) {
                    tabs.setTabVisible(templateName, true);
                    consumedTemplates.add(templateName);
                    const group = entityMgr.createPropertyGroup(templateName);
                    propList.append(group);
                }
            }

            if (entityMgr.hasPropertyGroup("Info")) {
                const img = entityMgr.getPropertyGroupElement("Info", "Image");
                if (img) {
                    img.showLabel = false;
                    lastImage.replaceWith(img);
                    lastImage = img;
                }
            }

            dimensionsTab.replaceChildren(
                ...entityMgr.propertyGroups
                    .filter(groupName => !consumedTemplates.has(groupName))
                    .map(group => entityMgr.createPropertyListing(group))
            );


            const extraRelGroups = [];

            for (const groupName of entityMgr.relationshipGroups) {
                const tab = tabs.getTab(groupName);
                if (!tab) {
                    extraRelGroups.push(groupName);
                }
                else {
                    tabs.setTabVisible(groupName, true);
                    loadRelationshipGroups(entityMgr, tab, groupName);
                }
            }

            if (extraRelGroups.length > 0) {
                loadRelationshipGroups(entityMgr, otherTab, ...extraRelGroups);
            }

            if (!tabs.getTabVisible(tabs.selectedTab)) {
                tabs.selectedTab = "Dimensions/Capabilities";
            }

            tabs.setTabVisible("Dimensions/Capabilities", true);
            tabs.setTabVisible("References", true);
            tabs.disabled = false;
        }

        return entityId;
    });
}

function loadRelationshipGroups(entityMgr: EntityManager, tab: HTMLElement, ...groups: string[]) {
    const subEntity = Div(ClassName("sub-entity"));

    const relations = groups.map(groupName => {

        const relations = entityMgr.createRelationshipFieldSet(groupName);
        const relEditor = relations.querySelector<RelationshipEditorElement>("relationship-editor");

        relEditor.addEventListener("itemselected", async (evt) => {
            const relationship = evt.item;

            subEntity.innerHTML = "";

            if (isRelationshipModel(relationship)) {
                const tasks = new Array<Promise<EntityManager>>();

                tasks.push(EntityManager.load(relationship.child, {
                    hideOthers: true,
                    readOnly: true,
                    deletable: isContributor
                }));

                if (relationship.propertyEntity) {
                    tasks.push(EntityManager.load(relationship.propertyEntity, {
                        hideOthers: true,
                        readOnly: !isContributor
                    }));
                }

                const subs = await Promise.all(tasks);

                for (const sub of subs) {
                    sub.connectAt(subEntity, sub.entity.type.isPrimary);
                    if (sub.entity.id !== relationship.propertyEntity?.id) {
                        sub.addEventListener("removing", async () => {
                            await entityMgr.deleteRelationship(relEditor, relationship);
                            subEntity.innerHTML = "";
                        });

                        sub.addEventListener("itemselected", async (evt) => {
                            const subEntity = evt.item;
                            await loadEntity(subEntity.id);
                            history.push(subEntity.id);
                        });
                    }
                }
            }
        });

        return relations;
    });

    tab.append(
        ...relations,
        subEntity
    );
}