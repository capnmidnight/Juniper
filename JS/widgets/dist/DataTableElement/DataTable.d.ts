import { boolCallback, compareCallback, FieldName, stringCallback } from "@juniper-lib/util";
import { ElementChild, HtmlProp, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { SheetDef } from "./types";
type Pagination = "on" | "off" | "auto";
export declare function DefaultSort(defaultSort: compareCallback<any>): HtmlProp<"defaultSort", compareCallback<any>, Node & Record<"defaultSort", compareCallback<any>>>;
export declare function Enumerations(enumerations: Record<string, Record<string | number, any>>): HtmlProp<"enumerations", Record<string, Record<string | number, any>>, Node & Record<"enumerations", Record<string, Record<string | number, any>>>>;
export declare function Validators(validators: Record<string, import("@juniper-lib/util").boolCallback<any>>): HtmlProp<"validators", Record<string, boolCallback<any>>, Node & Record<"validators", Record<string, boolCallback<any>>>>;
interface ColumnAlterer {
    setVisible(columnIndex: number, visible: boolean): void;
    setHeader(columnIndex: number, header: string): void;
    setField(columnIndex: number, field: string): void;
    setType(columnIndex: number, type: string): void;
}
export declare class DataTableElement<T> extends TypedHTMLElement<{
    "itemselected": TypedItemSelectedEvent<T>;
}> {
    #private;
    static observedAttributes: string[];
    get data(): T[];
    set data(items: T[]);
    get selectedValue(): T;
    set selectedValue(v: T);
    get paginated(): Pagination;
    set paginated(v: Pagination);
    get autoPaginated(): boolean;
    set autoPaginated(v: boolean);
    get filteredData(): T[];
    get resultsPerPage(): number;
    set resultsPerPage(v: number);
    get defaultSort(): compareCallback<T>;
    set defaultSort(v: compareCallback<T>);
    get dateFormatter(): stringCallback<Date>;
    set dateFormatter(v: stringCallback<Date>);
    get enumerations(): Record<string, Record<string | number, any>>;
    set enumerations(v: Record<string, Record<string | number, any>>);
    get validators(): Record<string, boolCallback<T>>;
    set validators(v: Record<string, boolCallback<T>>);
    constructor();
    get pageSize(): number;
    set pageSize(v: number);
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    on(type: string, handler: (obj: T) => void): void;
    off(type: string, handler: (obj: T) => void): void;
    enableEvent(eventName: string, enabled: boolean): void;
    resize(): void;
    filter(): void;
    getFilterValue(columnName: string): string;
    setFilterValue(columnName: string, filterValue: string): void;
    clearFilters(): void;
    /**
     * Search the given fields (or all filterable fields if no fields are given)
     * of all of the data table items and set the filters to make matches.
     */
    searchAll(value: string, ...fields: FieldName<T>[]): void;
    /**
     * Search the given fields (or all filterable fields if no fields are given)
     * of all of the data table items and set the filters to make matches.
     */
    searchFirstMatch(value: string, ...fields: FieldName<T>[]): void;
    update(item: T): void;
    addItem(item: T): void;
    removeItem(item: T): void;
    getSheetDef(sheetName: string, allData: boolean): SheetDef<any>;
    alterColumns(action: (columns: ColumnAlterer) => void): void;
    static install(): import("@juniper-lib/dom").ElementFactory<DataTableElement<unknown>>;
}
export declare function DataTable<T>(...rest: ElementChild<DataTableElement<T>>[]): DataTableElement<T>;
export {};
//# sourceMappingURL=DataTable.d.ts.map