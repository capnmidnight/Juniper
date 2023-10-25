import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import "./header.css";
import "./index.css";
import "./main.css";
import "./nav.css";
export declare class AppShell {
    private readonly fetcher;
    constructor(fetcher: IFetcher);
    maximize(): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    minimize(): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    setMenuHidden(hidden: boolean): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    getCanGoBack(): Promise<boolean>;
    getCanGoForward(): Promise<boolean>;
    setMenuUI(mainNav: HTMLElement, button: HTMLButtonElement): void;
    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement): void;
    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement): void;
}
//# sourceMappingURL=index.d.ts.map