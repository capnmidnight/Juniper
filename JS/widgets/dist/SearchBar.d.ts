export declare class SearchBar extends EventTarget {
    #private;
    constructor(searchBar: HTMLInputElement, searchBtn: HTMLButtonElement);
    get searchTerm(): string;
    addTerms(...terms: string[]): void;
    removeTerms(...terms: string[]): void;
}
//# sourceMappingURL=SearchBar.d.ts.map