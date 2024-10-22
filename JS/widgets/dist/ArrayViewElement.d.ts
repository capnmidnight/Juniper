import { compareCallback, eventHandler } from "@juniper-lib/util";
import { ElementChild, HtmlEvent, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
import { makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
export declare function OnRemoving<DataT, TargetT extends EventTarget = EventTarget>(callback: eventHandler<RemovingEvent<DataT, TargetT>>, opts?: boolean | AddEventListenerOptions): HtmlEvent<"removing", RemovingEvent<DataT, TargetT>>;
export declare class RemovingEvent<DataT, TargetT extends EventTarget = EventTarget> extends TypedEvent<"removing", TargetT> {
    #private;
    get item(): DataT;
    constructor(item: DataT);
}
type ArrayViewEventMap<DataT> = {
    "removing": RemovingEvent<DataT, ArrayViewElement<DataT>>;
    "itemselected": TypedItemSelectedEvent<DataT, ArrayViewElement<DataT>>;
};
export declare class ArrayViewElement<DataT> extends TypedHTMLElement<ArrayViewEventMap<DataT>> implements TypedHTMLElement<ArrayViewEventMap<DataT>> {
    #private;
    static observedAttributes: string[];
    constructor();
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get labelField(): makeItemCallback<DataT>;
    set labelField(v: makeItemCallback<DataT>);
    get sortKeyField(): compareCallback<DataT>;
    set sortKeyField(v: compareCallback<DataT>);
    get values(): DataT[];
    set values(v: DataT[]);
    get idvalues(): string[];
    set idvalues(v: string[]);
    get selectedItem(): DataT;
    set selectedItem(v: DataT);
    addItem(item: DataT): void;
    removeItem(item: DataT): void;
    clear(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<ArrayViewElement<unknown>>;
}
export declare function ArrayView<DataT>(...rest: ElementChild<ArrayViewElement<DataT>>[]): ArrayViewElement<DataT>;
export {};
//# sourceMappingURL=ArrayViewElement.d.ts.map