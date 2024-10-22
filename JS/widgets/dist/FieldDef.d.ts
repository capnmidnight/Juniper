import { compareCallback } from "@juniper-lib/util";
import { HtmlProp } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";
export type makeItemCallback<T> = (item: T) => string;
export declare function fieldGetter<T>(fieldNameOrGetter: string | makeItemCallback<T>): makeItemCallback<T>;
export declare function LabelField<T>(fieldName: string | makeItemCallback<T>): HtmlProp<"labelField", Function, Node & Record<"labelField", Function>>;
export declare function ValueField<T>(fieldName: string | makeItemCallback<T>): HtmlProp<"valueField", Function, Node & Record<"valueField", Function>>;
export declare function SortKeyField<T>(fieldName: string | makeItemCallback<T>): HtmlProp<"sortKeyField", Function, Node & Record<"sortKeyField", Function>>;
export declare function CompareBy<T>(comparison: compareCallback<T>): HtmlProp<"sortKeyField", Function, Node & Record<"sortKeyField", Function>>;
export declare function DataAttr<T>(values: readonly T[]): HtmlProp<"data", readonly T[], Node & Record<"data", readonly T[]>>;
export declare function SelectedItem<T>(value: T): HtmlProp<"selectedItem", T, Node & Record<"selectedItem", T>>;
export declare function OnItemSelected<DataT, TargetT extends EventTarget = EventTarget>(callback: (evt: TypedItemSelectedEvent<DataT, TargetT>) => void, opts?: EventListenerOptions): import("@juniper-lib/dom").HtmlEvent<string, Event>;
export declare function identityString(item: any): string;
//# sourceMappingURL=FieldDef.d.ts.map