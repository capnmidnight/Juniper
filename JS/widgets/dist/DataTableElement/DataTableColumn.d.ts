import { boolCallback, FieldName, stringCallback, unknownCallback } from "@juniper-lib/util";
import { ElementChild, HtmlAttr } from "@juniper-lib/dom";
import { InputDateRangeElement } from "../InputDateRangeElement";
import { BaseColumnDef, ColumnType } from "./types";
export type ColumnDef<T> = BaseColumnDef<T> & {
    typeEnum: string;
    styleType: string | Record<number | string, string>;
    styleField: string;
    group: string;
    title: string;
    event: string;
    icon: string;
    sortable: boolean;
    filterable: boolean;
    exportable: boolean;
    hidden: boolean;
    ellipsis: boolean;
};
export declare class DataTableItemEvent<T> extends Event {
    #private;
    constructor(event: string, item: T);
    get item(): T;
}
export declare function isDataTableItemEvent<T>(evt: Event): evt is DataTableItemEvent<T>;
export declare function DataColumn(...rest: ElementChild[]): HTMLElement;
export declare function DataColumnGroup(...rest: ElementChild[]): HTMLElement;
export declare function NameAttr(name: string): HtmlAttr<string, Node>;
export declare function FieldAttr(field: string): HtmlAttr<string, Node>;
export declare function HeaderAttr(header: string): HtmlAttr<string, Node>;
export declare function EventAttr(event: string): HtmlAttr<string, Node>;
export declare class DataTableColumnElement<T> extends EventTarget {
    #private;
    get header(): HTMLElement;
    get getValue(): unknownCallback<T>;
    get filter(): HTMLInputElement | HTMLSelectElement | InputDateRangeElement;
    get filterActive(): boolean;
    get dateFormatter(): stringCallback<Date>;
    set dateFormatter(v: stringCallback<Date>);
    get type(): ColumnType;
    get field(): FieldName<T>;
    get group(): string;
    get event(): string;
    eventEnabled: boolean;
    get filterable(): boolean;
    get exportable(): boolean;
    get ellipsis(): boolean;
    get hidden(): boolean;
    get longName(): string;
    get shortName(): string;
    constructor(columnDef: ColumnDef<T>, groupStyle: string, form: HTMLFormElement, enumerations: Record<string, Record<string | number, any>>, validators: Record<string, boolCallback<T>>);
    updateFilters(data: T[]): void;
    updateCell(cell: HTMLTableCellElement, item: T): void;
    resetSort(): void;
    get sorter(): import("@juniper-lib/util").compareCallback<unknown>;
    makeSorterCell(): HTMLTableCellElement;
    makeFilterCell(): HTMLTableCellElement;
    setFilterValue(value: string): void;
    get filterer(): boolCallback<T>;
    searchMatch(item: T, value: any): boolean;
    makeItemCell(item: T): HTMLTableCellElement;
}
//# sourceMappingURL=DataTableColumn.d.ts.map