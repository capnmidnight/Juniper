import { generate } from "@juniper-lib/util";
import { Button, ID, Input, OnChange, OnClick, OnInput } from "@juniper-lib/dom";
import { DataAttr, InclusionList, LabelField, OnItemSelected, ValueField } from "@juniper-lib/widgets";

const filter = Input(
    ID("filter"),
    OnInput(() => {
        staticList.filter = filter.value;
    })
);

const staticList = InclusionList(
    ID("staticList"),
    OnInput(evt => {
        console.log("input", evt);
    }),
    OnChange(evt => {
        console.log("change", evt);
    })
);

class Obj {
    ID: number;
    Title: string;
    constructor(ID: number, Title: string){
        this.ID = ID;
        this.Title = Title;
    }
}

const dynamicList = InclusionList(
    ID("dynamicList"),
    ValueField("ID"),
    LabelField("Title"),
    DataAttr(generate(0, 10)
        .map(i => new Obj(10 * i, `Option ${i + 1}`))),
    OnItemSelected(() => {
        console.log(dynamicList.selectedItems);
    })
);

dynamicList.selectedItems = dynamicList.data;

Button(
    ID("disabler"),
    OnClick(() => dynamicList.disabled = !dynamicList.disabled)
);