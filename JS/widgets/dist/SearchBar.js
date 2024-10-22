import { debounce } from "@juniper-lib/util";
import { DataList, ID, Option, Value, onEnterKeyCallback } from "@juniper-lib/dom";
export class SearchBar extends EventTarget {
    #searchBar;
    #searchBtn;
    #terms = null;
    #search;
    constructor(searchBar, searchBtn) {
        super();
        this.#searchBar = searchBar;
        this.#searchBtn = searchBtn;
        const searchEvt = new Event("search");
        this.#search = debounce(() => this.dispatchEvent(searchEvt));
        this.#searchBtn.addEventListener("click", this.#search);
        this.#searchBar.addEventListener("search", this.#search);
        this.#searchBar.addEventListener("input", this.#search);
        this.#searchBar.addEventListener("keyup", onEnterKeyCallback(this.#search));
        Object.seal(this);
    }
    get searchTerm() {
        return this.#searchBar.value;
    }
    addTerms(...terms) {
        if (!this.#terms) {
            this.#terms = DataList(ID(this.#searchBar.id + "-datalist"));
            this.#searchBar.parentElement?.append(this.#terms);
            this.#searchBar.setAttribute("list", this.#terms.id);
        }
        this.#terms.append(...terms.map(term => Option(Value(term))));
        this.#search();
    }
    removeTerms(...terms) {
        if (this.#terms) {
            for (const term of terms) {
                const option = this.#terms.querySelector(`option[value='${term}']`);
                if (option) {
                    option.remove();
                }
            }
            if (this.#terms.options.length === 0) {
                this.#terms.remove();
                this.#terms = null;
            }
        }
        this.#search();
    }
}
//# sourceMappingURL=SearchBar.js.map