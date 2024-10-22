import { compareCallback } from "@juniper-lib/util";
import { ElementChild } from "@juniper-lib/dom";
export declare class PaginatedEvent<T> extends Event {
    readonly paginator: Paginator<T>;
    /**
     * Creates a new event for indicating the start and end of the pagination range.
    */
    constructor(paginator: Paginator<T>, eventInit?: EventInit);
}
export declare class PageSizeChangedEvent extends Event {
    readonly pageSize: number;
    /**
     * Creates a new event for indicating that a paginator view has changed the number of items per page.
    */
    constructor(pageSize: number);
}
export declare class PageChangedEvent extends Event {
    readonly page: number;
    constructor(page: number);
}
export declare class Paginator<T> extends EventTarget {
    #private;
    constructor(elements: PaginatorViewElement[]);
    get disabled(): boolean;
    set disabled(v: boolean);
    get autoSized(): boolean;
    set autoSized(v: boolean);
    /**
     * Get the current target of results per page.
     */
    get resultsPerPage(): number;
    /**
     * Set the current target of results per page.
     */
    set resultsPerPage(value: number);
    /**
     * Get the current page number.
     */
    get page(): number;
    /**
     * Set the current page number.
     */
    set page(v: number);
    /**
     * Get the data that the paginator is managing.
     */
    get data(): T[];
    /**
     * Set the data that the paginator should manage.
     */
    set data(data: T[]);
    /**
     * Get the data that is being shown on the current page.
     */
    get pageData(): T[];
    /** Get the number of pages the current data is split into. */
    get numberOfPages(): number;
    get start(): number;
    get end(): number;
    get numberOfItems(): number;
    /**
     * Add an item to the data list.
     */
    add(item: T): void;
    /**
     * Remove an item from the data list.
     */
    remove(item: T): void;
    /**
     * Updates an item's position in the data list
     */
    update(item: T): void;
    /**
     * Sort the data to be paginated.
     */
    sort(compareFunc: compareCallback<T>): void;
    resort(): void;
    /**
     * Brings the page that holds the value into view.
     */
    select(value: T): void;
}
export declare class PaginatorViewElement extends HTMLElement {
    #private;
    get maxPages(): number;
    get resultsPerPage(): number[];
    get maxResultsPerPage(): number;
    constructor();
    connectedCallback(): void;
    refresh(numberOfResults: number, resultsPerPage: number, currentPage: number, hideView: boolean, autoSized: boolean): void;
    static install(): import("@juniper-lib/dom").ElementFactory<PaginatorViewElement>;
}
export declare function PaginatorView(...rest: ElementChild<PaginatorViewElement>[]): PaginatorViewElement;
//# sourceMappingURL=Paginator.d.ts.map