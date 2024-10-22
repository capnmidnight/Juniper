import { identity } from "@juniper-lib/util";
import { A, HRef, InputCheckbox, InputDate, InputNumber, InputText, Pattern, Step, Target, TextArea } from "@juniper-lib/dom";
import { InputCurrency, TypedSelect } from "@juniper-lib/widgets";
import { ArrayEnumerationEditor } from "./ArrayEnumerationEditor";
import { ArrayFileEditor, ArrayFileEditorElement } from "./ArrayFileEditor";
import { editorNotImplemented, editorNotSupported } from "./editorNotImplemented";
import { makeSingleInputElement } from "./makeSingleInputElement";
import { SingleFileEditor, SingleFileEditorElement } from "./SingleFileEditor";
export * from "./InputWithDateElement";
const SingleBooleanEditor = makeSingleInputElement("Boolean", InputCheckbox, input => input.value.toString(), identity, input => input.checked, (input, v) => input.checked = v);
const SingleIntegerEditor = makeSingleInputElement("Integer", () => InputNumber(Step(1), Pattern("-?\\d+")), input => input.value.toString(), identity, input => input.valueAsNumber, (input, value) => input.valueAsNumber = value);
const SingleDecimalEditor = makeSingleInputElement("Decimal", () => InputNumber(Step(0.01)), input => input.value.toString(), identity, input => input.valueAsNumber, (input, value) => input.valueAsNumber = value);
const SingleCurrencyEditor = makeSingleInputElement("Currency", () => InputCurrency(Step(0.01)), input => input.value.toString(), identity, input => input.valueAsNumber, (input, value) => input.valueAsNumber = value);
const SingleStringEditor = makeSingleInputElement("String", InputText, input => input.value, identity, input => input.value, (input, value) => input.value = value);
const SingleDateEditor = makeSingleInputElement("Date", InputDate, input => input.value.toString(), identity, input => input.valueAsDate, (input, value) => input.valueAsDate = value);
const SingleLinkEditor = makeSingleInputElement("Link", InputText, input => A(HRef(input.value), Target("_blank"), input.value), identity, input => input.value, (input, value) => input.value = value);
const SingleLongTextEditor = makeSingleInputElement("LongText", TextArea, input => input.value, identity, input => input.value, (input, value) => input.value = value);
const SingleEnumerationEditor = makeSingleInputElement("Enumeration", (TypedSelect), property => property.value, identity, input => input.value, (input, value) => input.value = value, (input, values) => input.data = values);
export function isFileEditor(editor) {
    return editor instanceof ArrayFileEditorElement
        || editor instanceof SingleFileEditorElement;
}
export const PropertyEditorFactories = {
    Unknown: {
        Single: editorNotSupported("Unknown", "Single"),
        Array: editorNotSupported("Unknown", "Array"),
        TimeSeries: editorNotSupported("Unknown", "TimeSeries")
    },
    Boolean: {
        Single: SingleBooleanEditor,
        Array: editorNotSupported("Boolean", "Array"),
        TimeSeries: editorNotSupported("Boolean", "TimeSeries")
    },
    Integer: {
        Single: SingleIntegerEditor,
        Array: editorNotImplemented("Integer", "Array"),
        TimeSeries: editorNotImplemented("Integer", "TimeSeries")
    },
    Decimal: {
        Single: SingleDecimalEditor,
        Array: editorNotImplemented("Decimal", "Array"),
        TimeSeries: editorNotImplemented("Decimal", "TimeSeries")
    },
    Currency: {
        Single: SingleCurrencyEditor,
        Array: editorNotImplemented("Currency", "Array"),
        TimeSeries: editorNotImplemented("Currency", "TimeSeries")
    },
    String: {
        Single: SingleStringEditor,
        Array: editorNotImplemented("String", "Array"),
        TimeSeries: editorNotImplemented("String", "TimeSeries")
    },
    Enumeration: {
        Single: SingleEnumerationEditor,
        Array: ArrayEnumerationEditor,
        TimeSeries: editorNotSupported("Enumeration", "TimeSeries")
    },
    Date: {
        Single: SingleDateEditor,
        Array: editorNotImplemented("Date", "Array"),
        TimeSeries: editorNotSupported("Date", "TimeSeries")
    },
    Link: {
        Single: SingleLinkEditor,
        Array: editorNotImplemented("Link", "Array"),
        TimeSeries: editorNotSupported("Link", "TimeSeries")
    },
    File: {
        Single: SingleFileEditor,
        Array: ArrayFileEditor,
        TimeSeries: editorNotSupported("File", "TimeSeries")
    },
    LongText: {
        Single: SingleLongTextEditor,
        Array: editorNotSupported("LongText", "Array"),
        TimeSeries: editorNotSupported("LongText", "TimeSeries")
    }
};
//# sourceMappingURL=index.js.map