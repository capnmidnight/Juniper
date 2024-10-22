import { isNullOrUndefined, singleton } from "@juniper-lib/util";
import { ClassList, ElementChild, FieldSet, H1, Legend, SingletonStyleBlob, TitleAttr, TypeAttr, alignItems, border, display, em, flexDirection, fr, gap, gridTemplateColumns, gridTemplateRows, padding, registerFactory, rule } from "@juniper-lib/dom";
import { BaseDialogElement, DataColumn, DataTable, DataTableElement, FieldAttr, SaveButtonText } from "@juniper-lib/widgets";
import { Cancelable } from "@juniper-lib/widgets/src/Cancelable";
import { CedrusDataAPI } from "../adapters";
import { EntityModel } from "../models/EntityModel";
import { EntityTypeModel } from "../models/EntityTypeModel";

export class SelectEntityDialogElement extends BaseDialogElement<EntityTypeModel[], EntityModel> {

    readonly #dataTable: DataTableElement<EntityModel>;

    constructor() {
        SingletonStyleBlob("Juniper::Cedrus::SelectEntityDialog", () =>
            rule("select-entity",
                display("contents"),

                rule(">standard-dialog",

                    rule(" div[slot='modal-body']",
                        padding(em(1)),
                        display("grid"),
                        gridTemplateColumns(fr(1), "auto", "auto"),
                        gridTemplateRows("auto"),
                        alignItems("center"),
                        gap(em(2)),

                        rule(" fieldset",
                            display("flex"),
                            flexDirection("column")
                        )
                    ),

                    rule("select-entity tr.selected",
                        border("dashed 2px black")
                    )
                )
            )
        );

        const dataTable = DataTable<EntityModel>(
            ClassList("select-entity", "table", "table-striped", "table-borderless", "table-hover"),
            DataColumn(TypeAttr("text"), FieldAttr("type.name"), TitleAttr("Type")),
            DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")),
            DataColumn(TypeAttr("date"), FieldAttr("createdOn"), TitleAttr("Created")),
            DataColumn(TypeAttr("text"), FieldAttr("user.name"), TitleAttr("By"))
        );

        super(
            H1("Entities"),

            FieldSet(
                Legend(
                    "Select existing Entity"
                ),
                dataTable
            ),

            SaveButtonText("Select"),
            Cancelable(true)
        );

        this.#dataTable = dataTable;

        this.dialog.addEventListener("showing", async evt => {
            const ds = await CedrusDataAPI.dataSourceTask;
            this.#dataTable.data = await ds.getEntities(...evt.value);
            this.#dataTable.selectedValue = null;
            evt.resolve();
        });

        this.dialog.addEventListener("validating", evt => {
            if (isNullOrUndefined(this.#dataTable.selectedValue)) {
                evt.preventDefault();
            }
        });

        this.dialog.addEventListener("submit", evt => {
            evt.resolve(this.#dataTable.selectedValue);
        });
    }

    static install() {
        return singleton("Juniper::Cedrus::SelectEntityDialogElement", () =>
            registerFactory("select-entity", SelectEntityDialogElement))
    }
}

export function SelectEntityDialog(...rest: ElementChild<SelectEntityDialogElement>[]) {
    return SelectEntityDialogElement.install()(...rest);
}