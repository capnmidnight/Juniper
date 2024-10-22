import {
    arrayInsert,
    arrayRemove,
    binarySearch,
    compareCallback,
    debounce,
    generate,
    singleton
} from "@juniper-lib/util";
import {
    A,
    AriaLabel,
    ClassList,
    Clear,
    CustomData,
    Div,
    ElementChild,
    HtmlRender,
    I,
    LI,
    Nav,
    OnClick,
    SingletonStyleBlob,
    UL,
    background,
    backgroundColor,
    border,
    borderRadius,
    color,
    cursor,
    hide,
    justifyContent,
    px,
    registerFactory,
    rule,
    show
} from "@juniper-lib/dom";


export class PaginatedEvent<T> extends Event {
    /**
     * Creates a new event for indicating the start and end of the pagination range.
    */

    constructor(public readonly paginator: Paginator<T>, eventInit?: EventInit) {
        super("paginated", eventInit);
        Object.freeze(this);
    }
}

export class PageSizeChangedEvent extends Event {
    /**
     * Creates a new event for indicating that a paginator view has changed the number of items per page. 
    */

    constructor(public readonly pageSize: number) {
        super("pagesizechanged");
        Object.freeze(this);
    }
}

export class PageChangedEvent extends Event {
    constructor(public readonly page: number) {
        super("pagechanged");
        Object.freeze(this);
    }
}

export class Paginator<T> extends EventTarget {

    #data: T[] = [];
    #resultsPerPage = 25;
    #page = 1;
    #enabled = true;
    #autoSized = false;
    #curSort: compareCallback<T>;
    #views;
    #refresh;

    constructor(elements: PaginatorViewElement[]) {
        super();
        this.#refresh = debounce(this.#_refresh.bind(this));
        this.#views = elements.map(view => {
            view.addEventListener("pagesizechanged", (evt: any) => this.resultsPerPage = evt.pageSize);
            view.addEventListener("pagechanged", (evt: any) => this.page = evt.page);
            view.addEventListener("decrementpage", () => --this.page);
            view.addEventListener("incrementpage", () => ++this.page);
            return view;
        });

        Object.freeze(this);
    }

    get disabled(): boolean { return !this.#enabled; }
    set disabled(v) {
        this.#enabled = !v;
        this.#refresh();
    }

    get autoSized(): boolean { return this.#autoSized; }
    set autoSized(v) {
        this.#autoSized = v;
        this.#refresh();
    }

    /** 
     * Get the current target of results per page.
     */
    get resultsPerPage(): number {
        if (this.#resultsPerPage <= 0 || this.disabled) {
            return this.#data.length || 1;
        }

        return this.#resultsPerPage;
    }

    /** 
     * Set the current target of results per page. 
     */
    set resultsPerPage(value) {
        if (value !== this.#resultsPerPage) {
            this.#resultsPerPage = value;
            this.#refresh();
        }
    }

    /**
     * Get the current page number.
     */
    get page(): number {
        return this.#page = Math.max(1, Math.min(this.#page, this.numberOfPages));
    }

    /**
     * Set the current page number.
     */
    set page(v) {
        if (v !== this.#page) {
            this.#page = v;
            this.#refresh();
        }
    }

    /**
     * Get the data that the paginator is managing.
     */
    get data() {
        return this.#data;
    }

    /**
     * Set the data that the paginator should manage.
     */
    set data(data) {
        this.#data.splice(0, this.#data.length, ...data);
        this.resort();
    }

    /**
     * Get the data that is being shown on the current page.
     */
    get pageData() {
        return this.#data.slice(this.start, this.end);
    }

    /** Get the number of pages the current data is split into. */
    get numberOfPages(): number { return Math.ceil(this.#data.length / this.resultsPerPage); }

    get start(): number { return (this.#page - 1) * this.resultsPerPage; }

    get end(): number { return Math.min(this.#data.length, this.start + this.resultsPerPage); }

    get numberOfItems(): number { return this.end - this.start; }

    /**
     * Add an item to the data list.
     */
    add(item: T) {
        const index = this.#curSort
            ? binarySearch(this.#data, item, this.#curSort, "append")
            : this.#data.length;
        arrayInsert(this.#data, item, index);
        this.#refresh();
    }

    /**
     * Remove an item from the data list.
     */
    remove(item: T) {
        arrayRemove(this.data, item);
        this.#refresh();
    }

    /**
     * Updates an item's position in the data list
     */
    update(item: T) {
        this.remove(item);
        this.add(item);
    }

    /**
     * Sort the data to be paginated.
     */
    sort(compareFunc: compareCallback<T>) {
        this.#curSort = compareFunc;
        this.resort();
    }

    resort() {
        if (this.#curSort) {
            this.#data.sort(this.#curSort);
            this.#refresh();
        }
    }

    /**
     * Brings the page that holds the value into view.
     */
    select(value: T) {
        const index = this.#data.indexOf(value);
        if (index >= 0) {
            const page = 1 + Math.floor(index / this.resultsPerPage);
            this.page = page;
        }
    }

    #_refresh() {
        for (const view of this.#views) {
            view.refresh(this.#data.length, this.resultsPerPage, this.page, this.disabled, this.autoSized);
        }

        this.dispatchEvent(new PaginatedEvent(this));
    }
}

export class PaginatorViewElement extends HTMLElement {

    #maxPages = 9;
    get maxPages(): number { return this.#maxPages; }

    #resultsPerPage = [5, 25, 50, 100, -1];
    get resultsPerPage(): number[] { return this.#resultsPerPage; }

    get maxResultsPerPage() { return Math.max(...this.resultsPerPage); }

    #numResultsLinks: HTMLElement[];
    #numResultsLabel: HTMLElement;
    #pageNumbersContainer: HTMLElement;
    #pageNumbersList: HTMLElement;
    #previousArrow: HTMLElement;
    #nextArrow: HTMLElement;
    #contents: HTMLElement;

    constructor() {

        SingletonStyleBlob("Juniper::Widgets::PaginatorView", () => [
            rule("paginator-view",
                justifyContent("center")
            ),

            rule("paginator-view .page-link",
                background("none"),
                border("none"),
                color("inherit")
            ),

            rule("paginator-view .page-link:hover",
                cursor("pointer")
            ),

            rule("paginator-view page-link, paginator-view .page-link.active",
                borderRadius(px(50)),
                color("white !important"),
                backgroundColor("orange")
            ),

            rule("paginator-view .page-item:first-child .page-link, paginator-view .page-item:last-child .page-link",
                borderRadius(px(50))
            )
        ]);

        super();

        const prevEvt = new Event("decrementpage");
        this.#previousArrow = LI(
            ClassList("page-item", "arrow_left"),
            A(
                ClassList("page-link"),
                OnClick(() => this.dispatchEvent(prevEvt)),
                I(ClassList("fa-solid", "fa-backward"))
            )
        );

        const nextEvt = new Event("incrementpage");
        this.#nextArrow = LI(
            ClassList("page-item", "arrow_right"),
            A(
                ClassList("page-link"),
                OnClick(() => this.dispatchEvent(nextEvt)),
                I(ClassList("fa-solid", "fa-forward"))
            )
        );

        this.#pageNumbersList = UL(
            ClassList("pagination")
        );

        this.#numResultsLabel = Div(
            ClassList("col", "text-center")
        );

        this.#pageNumbersContainer = Div(
            ClassList("col"),
            Nav(
                AriaLabel("Results per Page"),
                Div(
                    ClassList("alight-self-center", "text-center"),
                    "Results per Page:"
                ),
                UL(
                    ClassList("pagination"),
                    ...(this.#numResultsLinks = this.resultsPerPage
                        .map(i => {
                            const pageSizeChangedEvent = new PageSizeChangedEvent(i);
                            return A(
                                ClassList(
                                    "page-link",
                                    "resultsPerPage"
                                ),
                                CustomData("pagecount", i),
                                OnClick(() => this.dispatchEvent(pageSizeChangedEvent)),
                                i === -1 ? "All" : i.toFixed()
                            );
                        })).map(link =>
                            LI(ClassList("page-item"), link))
                )
            )
        );

        this.#contents = Div(
            ClassList("row"),
            Div(
                ClassList("col"),
                Nav(
                    AriaLabel("Page Number"),
                    Div(
                        ClassList("align-self-center", "text-center"),
                        "Page Number:"
                    ),
                    this.#pageNumbersList
                )
            ),
            this.#numResultsLabel,
            this.#pageNumbersContainer
        );
    }

    connectedCallback() {
        if (this.#contents.parentElement !== this) {
            this.append(this.#contents);
        }
    }

    refresh(numberOfResults: number, resultsPerPage: number, currentPage: number, hideView: boolean, autoSized: boolean) {
        if (hideView) {
            hide(this);
        }
        else {
            show(this);
        }

        const numberOfPages = Math.ceil(numberOfResults / resultsPerPage);
        const start = (currentPage - 1) * resultsPerPage;
        const end = Math.min(numberOfResults, start + resultsPerPage);

        for (const link of this.#numResultsLinks) {
            const count = parseFloat(link.dataset["pagecount"]);
            const active = count === resultsPerPage || count === -1 && resultsPerPage > this.maxResultsPerPage;
            link.classList.toggle("active", active);
            link.classList.toggle("disabled", active);
        }

        let midPage = currentPage;
        let minPage = midPage - (this.maxPages - 1) / 2;
        let maxPage = midPage + (this.maxPages - 1) / 2;
        if (minPage < 1) {
            minPage = 1;
            maxPage = minPage + (this.maxPages - 1);
            if (maxPage > numberOfPages) {
                maxPage = numberOfPages;
            }
        } else if (maxPage > numberOfPages) {
            maxPage = numberOfPages;
            minPage = maxPage - (this.maxPages - 1);
            if (minPage < 1) {
                minPage = 1;
            }
        }

        HtmlRender(
            this.#pageNumbersList,
            Clear(),
            this.#previousArrow,
            ...generate(minPage, maxPage + 1)
                .map(page => LI(
                    ClassList("page-item"),
                    A(
                        ClassList(
                            "page-link",
                            "pageNumber",
                            page === currentPage && "active",
                            page === currentPage && "disabled"),
                        OnClick(() => this.dispatchEvent(new PageChangedEvent(page))),
                        I(
                            ClassList("page-item"),
                            page
                        )
                    )
                )),
            this.#nextArrow
        );

        this.#previousArrow.classList.toggle("disabled", currentPage === 1);
        this.#previousArrow.style.opacity = currentPage === 1 ? "0.5" : "1";
        this.#nextArrow.classList.toggle("disabled", currentPage === numberOfPages);
        this.#nextArrow.style.opacity = currentPage === numberOfPages ? "0.5" : "1";

        this.#numResultsLabel.textContent =
            numberOfResults === 0
                ? "No items found"
                : `Displaying ${start + 1} - ${end} of ${numberOfResults} results`;

        if (autoSized) {
            hide(this.#pageNumbersContainer);
        }
        else {
            show(this.#pageNumbersContainer);
        }
    }

    static install() {
        return singleton("Juniper::Widgets::PaginatorViewElement", () => registerFactory("paginator-view", PaginatorViewElement));
    }
}

export function PaginatorView(...rest: ElementChild<PaginatorViewElement>[]) {
    return PaginatorViewElement.install()(...rest);
}
