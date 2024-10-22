import { ID, Input, OnInput, QueryAll } from "@juniper-lib/dom";
import { DataTableElement } from "@juniper-lib/widgets";

interface record {
    name: string;
    A: string;
    B: string;
    C: string;
}

const data: record[] = [
    { name: "xyz", A: "green", B: "green", C: "green" },
    { name: "xyz", A: "green", B: "green", C: "yellow" },
    { name: "xyz", A: "green", B: "green", C: "red" },
    { name: "xyz", A: "green", B: "yellow", C: "green" },
    { name: "xyz", A: "green", B: "yellow", C: "yellow" },
    { name: "xyz", A: "green", B: "yellow", C: "red" },
    { name: "xyz", A: "green", B: "red", C: "green" },
    { name: "xyz", A: "green", B: "red", C: "yellow" },
    { name: "xyz", A: "green", B: "red", C: "red" },

    { name: "xyz", A: "yellow", B: "green", C: "green" },
    { name: "xyz", A: "yellow", B: "green", C: "yellow" },
    { name: "xyz", A: "yellow", B: "green", C: "red" },
    { name: "xyz", A: "yellow", B: "yellow", C: "green" },
    { name: "xyz", A: "yellow", B: "yellow", C: "yellow" },
    { name: "xyz", A: "yellow", B: "yellow", C: "red" },
    { name: "xyz", A: "yellow", B: "red", C: "green" },
    { name: "xyz", A: "yellow", B: "red", C: "yellow" },
    { name: "xyz", A: "yellow", B: "red", C: "red" },

    { name: "xyz", A: "red", B: "green", C: "green" },
    { name: "xyz", A: "red", B: "green", C: "yellow" },
    { name: "xyz", A: "red", B: "green", C: "red" },
    { name: "xyz", A: "red", B: "yellow", C: "green" },
    { name: "xyz", A: "red", B: "yellow", C: "yellow" },
    { name: "xyz", A: "red", B: "yellow", C: "red" },
    { name: "xyz", A: "red", B: "red", C: "green" },
    { name: "xyz", A: "red", B: "red", C: "yellow" },
    { name: "xyz", A: "red", B: "red", C: "red" }
];

const tables = QueryAll<DataTableElement<record>>("data-table")
    .map((table: DataTableElement<record>, i, l) => {
        table.enumerations = {
            Rankings: {
                "green": "good",
                "yellow": "warning",
                "red": "error"
            }
        };
        table.data = i < l.length - 1 ? data : data.slice(0, 5);
        return table;
    });

const search = Input(
    ID("search"),
    OnInput(() =>
        tables[0].searchFirstMatch(search.value)
    )
);