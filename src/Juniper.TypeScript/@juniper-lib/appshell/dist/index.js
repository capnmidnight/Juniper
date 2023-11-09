import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { debounce } from "@juniper-lib/events/dist/debounce";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import "./header.css";
import "./index.css";
import "./main.css";
import "./nav.css";
export class AppShellClosingEvent extends TypedEvent {
    constructor() {
        super("closing");
    }
}
export class AppShell extends TypedEventTarget {
    constructor(fetcher) {
        super();
        this.fetcher = fetcher;
    }
    async close() {
        const evt = new AppShellClosingEvent();
        this.dispatchEvent(evt);
        if (!evt.defaultPrevented) {
            await this.fetcher.post("/api/appshell/close")
                .exec();
        }
    }
    maximize() {
        return this.fetcher.post("/api/appshell/maximize")
            .exec();
    }
    minimize() {
        return this.fetcher.post("/api/appshell/minimize")
            .exec();
    }
    setMenuHidden(hidden) {
        return this.fetcher.post("/api/appshell/hidemenu")
            .body(hidden)
            .exec();
    }
    getCanGoBack() {
        return this.fetcher.get("/api/appshell/cangoback")
            .object()
            .then(unwrapResponse);
    }
    getCanGoForward() {
        return this.fetcher.get("/api/appshell/cangoforward")
            .object()
            .then(unwrapResponse);
    }
    setMenuUI(mainNav, button) {
        mainNav.classList.toggle("hidden", localStorage.getItem("menuHidden") == "true");
        button.addEventListener("click", () => {
            if (!mainNav.classList.contains("hidden")) {
                for (const child of mainNav.children) {
                    if (child instanceof HTMLElement) {
                        const style = getComputedStyle(child);
                        const marginStart = parseFloat(style.marginInlineStart);
                        const marginEnd = parseFloat(style.marginInlineEnd);
                        child.style.width = (mainNav.clientWidth - marginStart - marginEnd) + "px";
                    }
                }
            }
            const hidden = mainNav.classList.toggle("hidden");
            localStorage.setItem("menuHidden", hidden ? "true" : "false");
            this.setMenuHidden(hidden);
        });
    }
    setCloseButton(button) {
        if (button) {
            button.addEventListener("click", () => this.close());
        }
    }
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
    setHistoryUI(backButton, fwdButton) {
        if (backButton) {
            backButton.addEventListener("click", () => history.back());
            this.getCanGoBack().then(yes => backButton.disabled = !yes);
        }
        if (fwdButton) {
            fwdButton.addEventListener("click", () => history.forward());
            this.getCanGoForward().then(yes => fwdButton.disabled = !yes);
        }
    }
}
//# sourceMappingURL=index.js.map