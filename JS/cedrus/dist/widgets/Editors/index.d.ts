import { ArrayEnumerationEditor } from "./ArrayEnumerationEditor";
import { ArrayFileEditor, ArrayFileEditorElement } from "./ArrayFileEditor";
import { IPropertyEditorElement } from "./IPropertyEditorFactory";
import { SingleFileEditor, SingleFileEditorElement } from "./SingleFileEditor";
export * from "./InputWithDateElement";
export declare function isFileEditor(editor: IPropertyEditorElement): editor is (ArrayFileEditorElement | SingleFileEditorElement);
export declare const PropertyEditorFactories: {
    Unknown: {
        Single: () => void;
        Array: () => void;
        TimeSeries: () => void;
    };
    Boolean: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Boolean", "Single">;
        Array: () => void;
        TimeSeries: () => void;
    };
    Integer: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Integer", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Integer", "Array">;
        TimeSeries: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Integer", "TimeSeries">;
    };
    Decimal: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Decimal", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Decimal", "Array">;
        TimeSeries: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Decimal", "TimeSeries">;
    };
    Currency: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Currency", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Currency", "Array">;
        TimeSeries: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Currency", "TimeSeries">;
    };
    String: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"String", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"String", "Array">;
        TimeSeries: import("./IPropertyEditorFactory").IPropertyEditorFactory<"String", "TimeSeries">;
    };
    Enumeration: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Enumeration", "Single">;
        Array: typeof ArrayEnumerationEditor;
        TimeSeries: () => void;
    };
    Date: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Date", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Date", "Array">;
        TimeSeries: () => void;
    };
    Link: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Link", "Single">;
        Array: import("./IPropertyEditorFactory").IPropertyEditorFactory<"Link", "Array">;
        TimeSeries: () => void;
    };
    File: {
        Single: typeof SingleFileEditor;
        Array: typeof ArrayFileEditor;
        TimeSeries: () => void;
    };
    LongText: {
        Single: import("./IPropertyEditorFactory").IPropertyEditorFactory<"LongText", "Single">;
        Array: () => void;
        TimeSeries: () => void;
    };
};
//# sourceMappingURL=index.d.ts.map