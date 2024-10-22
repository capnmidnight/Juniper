import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { IFetcher } from "@juniper-lib/fetcher";
declare const CLOSING = "closing";
/**
 * And event that fires when something has requested the AppShell
 * window to close.
 */
export declare class AppShellClosingEvent extends TypedEvent<typeof CLOSING> {
    constructor();
}
type AppShellEventMap = {
    [CLOSING]: AppShellClosingEvent;
};
/**
 * An REST API wrapper for working with Juniper's AppShell
 * desktop window wrapper for Microsoft's Edge WebView2.
 * @see https://developer.microsoft.com/en-us/microsoft-edge/webview2
 */
export declare class AppShell extends TypedEventTarget<AppShellEventMap> {
    #private;
    /**
     * Creates a REST API wrapper for working with Juniper's APpShell
     * desktop window wrapper for Microsoft's Edge WebView2.
     * @param fetcher (Optional) an instance of the Fetcher used to
     * perform the requests with the backend service. If one is not
     * provided, one will be created.
     */
    constructor(fetcher?: IFetcher);
    /**
     * Tell the AppShell to close the window.
     *
     * Before the window is closed, an {AppShellClosingEvent} will be
     * fired that you can listen for and optionally set `evt.defaultPrevented`
     * to prevent actually sending the close request.
     *
     * @returns a promise that *techincally* resolves when the window has closed,
     * but you probably won't be able to await it because your script environment
     * will be destroyed when the application window has closed.
     */
    close(): Promise<void>;
    /**
     * Tell the AppShell to maximize the window.
     * @returns a promise that resolves when the window has been maximized.
     */
    maximize(): Promise<import("@juniper-lib/util").IResponse<void>>;
    /**
     * Tell the AppShell to minimize the window.
     * @returns a promise that resolves when the window has been minimized.
     */
    minimize(): Promise<import("@juniper-lib/util").IResponse<void>>;
    /**
     * When working with AppShell in the backend, you have the option of
     * including a side-drawer Menu UI in your ASP.NET code that can stay open
     * between requests or stay collapsed between requests. To be able to have that
     * UI stay maintain its state between page requests, you need to tell AppShell
     * when the user has triggered the UI event for collapsing/expanding the menu.
     * @param hidden whether or not the menu AI should be in the collapsed/hidden
     * state.
     * @returns
     */
    setMenuHidden(hidden: boolean): Promise<import("@juniper-lib/util").IResponse<void>>;
    /**
     * Ask the AppShell if there is enough navigation history that the user
     * could click a "back" button.
     * @returns a promise that resolves to a boolean indicating "back" is available.
     */
    getCanGoBack(): Promise<boolean>;
    /**
     * Ask the AppShell if there is enough navigation history that the user
     * could click a "foward" button.
     * @returns a promise that resolves to a boolean indicating "forward" is available.
     */
    getCanGoForward(): Promise<boolean>;
    /**
     * Wire up events for the collapsable side-drawer menu
     * @param mainNav the element that contains the menu.
     * @param button the button that triggers showing/hiding the menu.
     */
    setMenuUI(mainNav: HTMLElement, button: HTMLElement): void;
    /**
     * Wire up events for a scrollable article with buttons that jump to the beginning/end
     * of the article.
     * @param scrollToTop the button that triggers scrolling to the top of the article.
     * @param scrollToBottom the button that triggers scrolling to the top of the article.
     * @param article the article that scrolls.
     */
    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement): void;
    /**
     * Wire up events for the button that will close the window.
     */
    setCloseButton(button: HTMLButtonElement): void;
    /**
     * Wire up events for the navigation back/forward buttons.
     * @param backButton the button that triggers going back.
     * @param fwdButton the button that triggers going foward.
     */
    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement): void;
}
export {};
//# sourceMappingURL=index.d.ts.map