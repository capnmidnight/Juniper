import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import "./index.css";
import "./main.css";
import "./nav.css";
export declare class AppShellClosingEvent extends TypedEvent<"closing"> {
    constructor();
}
type AppShellEventMap = {
    "closing": AppShellClosingEvent;
};
export declare class AppShell extends TypedEventTarget<AppShellEventMap> {
    private readonly fetcher;
    constructor(fetcher: IFetcher);
    close(): Promise<void>;
    maximize(): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    minimize(): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    setMenuHidden(hidden: boolean): Promise<import("@juniper-lib/fetcher/dist/IResponse").IResponse<void>>;
    getCanGoBack(): Promise<boolean>;
    getCanGoForward(): Promise<boolean>;
    setMenuUI(mainNav: HTMLElement, button: HTMLElement): void;
    setCloseButton(button: HTMLButtonElement): void;
    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement): void;
    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement): void;
    private setHistoryUIBackButton;
    private setHistoryUIForwardButton;
}
export {};
//# sourceMappingURL=index.d.ts.map