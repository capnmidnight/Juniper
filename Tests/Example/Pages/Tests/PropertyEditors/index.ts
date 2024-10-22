import { InputWithDate, InputWithDateElement, PropertyEditorFactories } from "@juniper-lib/cedrus";
import { Article, InputNumber, Label, OnInput, Query } from "@juniper-lib/dom";
import { PropertyList } from "@juniper-lib/widgets";
import "./index.css";

let inputWithDate: InputWithDateElement<number>;

const enumSingle = PropertyEditorFactories.Enumeration.Single();
enumSingle.setValidValues(["A", "B", "C"]);
const enumArray = PropertyEditorFactories.Enumeration.Array();
enumArray.setValidValues(["A", "B", "C"]);

Article(
    Query("article"),
    PropertyList(
        Label("Input With Date"),
        InputWithDate<void>(),
        Label("Input With Date with Input Number"),
        inputWithDate = InputWithDate<number>(
            InputNumber(),
            OnInput(evt => {
                console.log(evt.target);
            })
        ),

        Label("Boolean Single"),
        PropertyEditorFactories.Boolean.Single(),

        Label("Currency Single"),
        PropertyEditorFactories.Currency.Single(),
        Label("Currency Array"),
        PropertyEditorFactories.Currency.Array(),
        Label("Currency TimeSeries"),
        PropertyEditorFactories.Currency.TimeSeries(),

        Label("Date Single"),
        PropertyEditorFactories.Date.Single(),
        Label("Date Array"),
        PropertyEditorFactories.Date.Array(),

        Label("Decimal Single"),
        PropertyEditorFactories.Decimal.Single(),
        Label("Decimal Array"),
        PropertyEditorFactories.Decimal.Array(),
        Label("Decimal TimeSeries"),
        PropertyEditorFactories.Decimal.TimeSeries(),

        Label("Enumeration Single"),
        enumSingle,
        Label("Enumeration Array"),
        enumArray,

        Label("File Single"),
        PropertyEditorFactories.File.Single(),
        Label("File Array"),
        PropertyEditorFactories.File.Array(),

        Label("Integer Single"),
        PropertyEditorFactories.Integer.Single(),
        Label("Integer Array"),
        PropertyEditorFactories.Integer.Array(),
        Label("Integer TimeSeries"),
        PropertyEditorFactories.Integer.TimeSeries(),

        Label("Link Single"),
        PropertyEditorFactories.Link.Single(),

        Label("LongText Single"),
        PropertyEditorFactories.LongText.Single(),

        Label("String Single"),
        PropertyEditorFactories.String.Single(),
        Label("String Array"),
        PropertyEditorFactories.String.Array(),
        Label("String TimeSeries"),
        PropertyEditorFactories.String.TimeSeries()
    )
);

console.log(inputWithDate);