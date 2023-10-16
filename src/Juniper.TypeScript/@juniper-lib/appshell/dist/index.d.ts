import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
export declare class AppShell {
    private readonly fetcher;
    constructor(fetcher: IFetcher);
    setMenuHidden(hidden: boolean): Promise<import("@juniper-lib/fetcher/src/IResponse").IResponse<void>>;
    getCanGoBack(): Promise<boolean>;
    getCanGoForward(): Promise<boolean>;
    setMenuUI(mainNav: HTMLElement, button: HTMLButtonElement): void;
    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement): void;
    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement): void;
}
//# sourceMappingURL=index.d.ts.map