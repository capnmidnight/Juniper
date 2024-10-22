import { compareCallback } from "@juniper-lib/util";
import { ElementChild } from "@juniper-lib/dom";
import { ArrayElementType, ITypedElementSelector, TypedElementSelector } from "./ArrayElementSelector";
import { makeItemCallback } from "./FieldDef";
export declare class ArrayEditorElement<InputT extends ITypedElementSelector<DataT>, DataT = ArrayElementType<InputT>> extends HTMLElement implements TypedElementSelector<DataT> {
    #private;
    static observedAttributes: string[];
    get selector(): InputT;
    get data(): readonly DataT[];
    set data(v: readonly DataT[]);
    get values(): DataT[];
    set values(v: DataT[]);
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get placeholder(): string;
    set placeholder(v: string);
    get labelField(): makeItemCallback<DataT>;
    set labelField(v: makeItemCallback<DataT>);
    get valueField(): makeItemCallback<DataT>;
    set valueField(v: makeItemCallback<DataT>);
    get sortKeyField(): compareCallback<DataT>;
    set sortKeyField(v: compareCallback<DataT>);
    get allowDuplicates(): boolean;
    set allowDuplicates(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    get readOnly(): boolean;
    set readOnly(v: boolean);
    get selectedItem(): DataT;
    clear(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<ArrayEditorElement<ITypedElementSelector<never>, never>>;
}
export declare function ArrayEditor<InputT extends ITypedElementSelector<DataT>, DataT = ArrayElementType<InputT>>(selector: InputT, ...rest: ElementChild<ArrayEditorElement<InputT, DataT>>[]): ArrayEditorElement<InputT, DataT>;
export declare function ArrayEditor<InputT extends ITypedElementSelector<DataT>, DataT = ArrayElementType<InputT>>(...rest: ElementChild<ArrayEditorElement<InputT, DataT>>[]): ArrayEditorElement<InputT, DataT>;
//# sourceMappingURL=ArrayEditorElement.d.ts.map