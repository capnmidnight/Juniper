import { CedrusOrgChart, OrgChartLayout, PROPERTY_TYPE } from '@juniper-lib/cedrus';
import { Button, Div, ID, InputText, OnClick, OnInput, Value } from '@juniper-lib/dom';
import { debounce } from '@juniper-lib/events';
import { DataAttr, LabelField, TypedSelect, ValueField, identityString } from '@juniper-lib/widgets';
import { DataAPI } from '../../../Models/DataAPI';
import "./index.css";

const chart = new CedrusOrgChart(Div(ID("orgChart")));

chart.addEventListener("itemselected", (evt) => {
    document.location = `/entities/${evt.item.id}`;
});

const chartDirection = TypedSelect<OrgChartLayout>(
    ID("chartDirection"),
    ValueField(identityString),
    LabelField(identityString),
    DataAttr(["top", "left", "bottom", "right"]),
    Value("top"),
    OnInput(() => {
        chart.layout = chartDirection.selectedItem;
    }));

Button(ID("expandAll"), OnClick(() => chart.expandAll().fit()));
Button(ID("collapseAll"), OnClick(() => chart.collapseAll().fit()));
Button(ID("fitNodes"), OnClick(() => chart.fit()));

const input = InputText(
    ID("searchbar"),
    OnInput(debounce(500, () =>
        chart.filter(input.value)
    ))
);

const ds = new DataAPI();

const tree = await ds.getTree(PROPERTY_TYPE("Title"));

chart.setTree(tree, true);
