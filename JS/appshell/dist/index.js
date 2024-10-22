import { TypedEvent, TypedEventTarget, debounce } from "@juniper-lib/events";
import { createFetcher, unwrapResponse } from "@juniper-lib/fetcher";
const CLOSING = "closing";
/**
 * And event that fires when something has requested the AppShell
 * window to close.
 */
export class AppShellClosingEvent extends TypedEvent {
    constructor() {
        super(CLOSING);
    }
}
/**
 * An REST API wrapper for working with Juniper's AppShell
 * desktop window wrapper for Microsoft's Edge WebView2.
 * @see https://developer.microsoft.com/en-us/microsoft-edge/webview2
 */
export class AppShell extends TypedEventTarget {
    #fetcher;
    /**
     * Creates a REST API wrapper for working with Juniper's APpShell
     * desktop window wrapper for Microsoft's Edge WebView2.
     * @param fetcher (Optional) an instance of the Fetcher used to
     * perform the requests with the backend service. If one is not
     * provided, one will be created.
     */
    constructor(fetcher) {
        super();
        this.#fetcher = fetcher ?? createFetcher();
    }
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
    async close() {
        const evt = new AppShellClosingEvent();
        this.dispatchEvent(evt);
        if (!evt.defaultPrevented) {
            await this.#fetcher.post("/api/appshell/close")
                .exec();
        }
    }
    /**
     * Tell the AppShell to maximize the window.
     * @returns a promise that resolves when the window has been maximized.
     */
    maximize() {
        return this.#fetcher.post("/api/appshell/maximize")
            .exec();
    }
    /**
     * Tell the AppShell to minimize the window.
     * @returns a promise that resolves when the window has been minimized.
     */
    minimize() {
        return this.#fetcher.post("/api/appshell/minimize")
            .exec();
    }
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
    setMenuHidden(hidden) {
        return this.#fetcher.post("/api/menuhider/hidemenu")
            .body(hidden)
            .exec();
    }
    /**
     * Ask the AppShell if there is enough navigation history that the user
     * could click a "back" button.
     * @returns a promise that resolves to a boolean indicating "back" is available.
     */
    getCanGoBack() {
        return this.#fetcher.get("/api/appshell/cangoback")
            .object()
            .then(unwrapResponse);
    }
    /**
     * Ask the AppShell if there is enough navigation history that the user
     * could click a "foward" button.
     * @returns a promise that resolves to a boolean indicating "forward" is available.
     */
    getCanGoForward() {
        return this.#fetcher.get("/api/appshell/cangoforward")
            .object()
            .then(unwrapResponse);
    }
    /**
     * Wire up events for the collapsable side-drawer menu
     * @param mainNav the element that contains the menu.
     * @param button the button that triggers showing/hiding the menu.
     */
    setMenuUI(mainNav, button) {
        const hidden = localStorage.getItem("menuHidden") == "true";
        const isHidden = mainNav.classList.contains("hidden");
        mainNav.classList.toggle("hidden", hidden);
        if (hidden !== isHidden) {
            this.setMenuHidden(hidden);
        }
        button.addEventListener("click", () => {
            const hidden = mainNav.classList.toggle("hidden");
            localStorage.setItem("menuHidden", hidden ? "true" : "false");
            this.setMenuHidden(hidden);
        });
    }
    /**
     * Wire up events for a scrollable article with buttons that jump to the beginning/end
     * of the article.
     * @param scrollToTop the button that triggers scrolling to the top of the article.
     * @param scrollToBottom the button that triggers scrolling to the top of the article.
     * @param article the article that scrolls.
     */
    setBodyUI(scrollToTop, scrollToBottom, article) {
        if (scrollToTop || scrollToBottom) {
            function indicateScroll() {
                const scrollTopVisible = article.scrollTop > 25;
                const scrollBottomVisible = (article.scrollTop + article.clientHeight) < article.scrollHeight - 25;
                if (scrollToTop) {
                    scrollToTop.classList.toggle("visible", scrollTopVisible);
                }
                if (scrollToBottom) {
                    scrollToBottom.classList.toggle("visible", scrollBottomVisible);
                }
            }
            const indScroll = debounce(indicateScroll);
            if (scrollToTop) {
                scrollToTop.addEventListener("click", () => article.scroll({
                    behavior: "smooth",
                    top: 0
                }));
            }
            if (scrollToBottom) {
                scrollToBottom.addEventListener("click", () => article.scroll({
                    behavior: "smooth",
                    top: article.scrollHeight
                }));
            }
            article.addEventListener("scroll", indScroll);
            const resizer = new ResizeObserver((evts) => {
                for (const evt of evts) {
                    if (evt.target === document.body) {
                        indScroll();
                    }
                }
            });
            resizer.observe(document.body);
            indicateScroll();
        }
    }
    /**
     * Wire up events for the button that will close the window.
     */
    setCloseButton(button) {
        this.#setButton(button, () => Promise.resolve(true), () => this.close());
    }
    /**
     * Wire up events for the navigation back/forward buttons.
     * @param backButton the button that triggers going back.
     * @param fwdButton the button that triggers going foward.
     */
    setHistoryUI(backButton, fwdButton) {
        this.#setButton(backButton, () => this.getCanGoBack(), () => history.back());
        this.#setButton(fwdButton, () => this.getCanGoForward(), () => history.forward());
    }
    async #setButton(button, getEnabled, onClick) {
        if (button) {
            button.addEventListener("click", onClick);
            button.disabled = !(await getEnabled());
        }
    }
}
//# sourceMappingURL=index.js.map