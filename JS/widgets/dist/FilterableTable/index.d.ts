import { ElementChild, ErsatzElement } from "@juniper-lib/dom/dist/tags";
import "./style.css";
export type HTMLValuedElement = HTMLInputElement | HTMLSelectElement;
export interface FilterableTableColumn<T> {
    header?: ElementChild;
    headerColSpan?: number;
    filter?: HTMLValuedElement;
    filterColSpan?: number;
    getCellValue?: (value: T, row?: HTMLTableRowElement) => ElementChild;
}
export interface FilterableTableOptions<T> {
    resourceName: string;
    pageSizes?: number[];
    columns: FilterableTableColumn<T>[];
}
export declare class FilterableTable<T extends any> implements ErsatzElement<HTMLTableElement> {
    readonly element: HTMLTableElement;
    static create<T>(options: FilterableTableOptions<T>): FilterableTable<T>;
    private readonly update;
    private readonly resourceName;
    private readonly rows;
    private readonly ranges;
    private readonly valueCache;
    private readonly filterElements;
    private readonly colCount;
    private readonly noContentMessageElement;
    private readonly paginator;
    private readonly columnIndices;
    private readonly resetButton;
    private readonly cellMappers;
    private readonly pageSizes;
    private readonly pageIndexKey;
    private readonly pageSizeKey;
    private _pageIndex;
    get pageIndex(): number;
    set pageIndex(v: number);
    private _pageSize;
    get pageSize(): number;
    set pageSize(v: number);
    constructor(element: HTMLTableElement);
    setPageSizes(pageSizes: number[]): void;
    setCellMapper(columnIndex: number, mapper: (value: T, row?: HTMLTableRowElement) => ElementChild): void;
    clear(): void;
    setValues(...values: T[]): void;
    select(sel: HTMLTableRowElement): void;
    get noContentMessage(): string;
    set noContentMessage(v: string);
    private get contentRoot();
    private makeKey;
    private saveValue;
    private deleteValue;
    private restoreValue;
    private isRange;
    private _update;
}
//# sourceMappingURL=index.d.ts.map