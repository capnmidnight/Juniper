import { isNullOrUndefined } from "@juniper-lib/util";
import { EntityManager, EntityTypeModel } from "@juniper-lib/cedrus";
import { Article, Button, Disabled, Label, OnClick, PlaceHolder, Query } from "@juniper-lib/dom";
import { DataAttr, LabelField, Nullable, OnItemSelected, PropertyList, SortKeyField, TypedSelect, TypedSelectElement, ValueField } from "@juniper-lib/widgets";
import { DataAPI } from "../../../Models/DataAPI";
import "./index.css";

const ds = new DataAPI();

const getEntity = ds.getEntity("CVN Nimitz");

const nimitz = await EntityManager.load(getEntity);

Object.assign(window, { ds, nimitz });

let createButton: HTMLButtonElement;
let entityTypesSelector: TypedSelectElement<EntityTypeModel>;


const article = Article(
    Query("article"),
    PropertyList(
        Label(
            entityTypesSelector = TypedSelect(
                ValueField<EntityTypeModel>(et => et.id.toFixed()),
                LabelField<EntityTypeModel>(et => et.name),
                SortKeyField<EntityTypeModel>(et => et.name),
                PlaceHolder("Select entity type"),
                Nullable(true),
                DataAttr(await ds.getEntityTypes()),
                OnItemSelected((evt) => {
                    createButton.disabled = isNullOrUndefined(evt.item);
                })
            )
        ),
        createButton = Button(
            "Create new",
            Disabled(true),
            OnClick(async () => {
                const entityName = prompt(`Please provide a name for the new ${entityTypesSelector.selectedItem.name}.`);
                if (entityName) {
                    (await EntityManager.load(ds.setEntity(
                        entityName,
                        entityTypesSelector.selectedItem,
                    ))).connectAt(article);
                }
            })
        )
    )
);

nimitz.connectAt(article);