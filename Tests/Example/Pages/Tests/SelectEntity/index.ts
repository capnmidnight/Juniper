import { EntityManager, SelectEntityDialog } from "@juniper-lib/cedrus";
import { Button, Clear, Disabled, Div, ID, OnClick } from "@juniper-lib/dom";
import { DataAPI } from "../../../Models/DataAPI";
import "./index.css";

const ds = new DataAPI();
const dialog = new SelectEntityDialog();
const entityTypes = (await ds.getEntityTypes())
    .filter(et => et.name === "Carrier"
        || et.name === "Destroyer");

Button(
    ID("selectEntity", true),
    Disabled(false),
    OnClick(async () => {
        const entity = await dialog.show(entityTypes);

        const editors = Div(
            ID("editors"),
            Clear()
        );

        if (entity) {
            const mgr = await EntityManager.load(entity);
            mgr.connectAt(editors);
        }
    })
)