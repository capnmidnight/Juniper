import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget"
import { debounce } from "@juniper-lib/events/dist/debounce";
import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import "./index.css";
import "./main.css";
import "./nav.css";

export class AppShellClosingEvent extends TypedEvent<"closing"> {
    constructor() {
        super("closing");
    }
}

type AppShellEventMap = {
    "closing": AppShellClosingEvent;
}

export class AppShell extends TypedEventTarget<AppShellEventMap> {
    constructor(private readonly fetcher: IFetcher) {
        super();
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

    setMenuHidden(hidden: boolean) {
        return this.fetcher.post("/api/appshell/hidemenu")
            .body(hidden)
            .exec();
    }

    getCanGoBack() {
        return this.fetcher.get("/api/appshell/cangoback")
            .object<boolean>()
            .then(unwrapResponse);
    }

    getCanGoForward() {
        return this.fetcher.get("/api/appshell/cangoforward")
            .object<boolean>()
            .then(unwrapResponse)
    }

    setMenuUI(mainNav: HTMLElement, button: HTMLElement) {
        mainNav.classList.toggle("hidden", localStorage.getItem("menuHidden") == "true");

        button.addEventListener("click", () => {
            const hidden = mainNav.classList.toggle("hidden");
            localStorage.setItem("menuHidden", hidden ? "true" : "false");
            this.setMenuHidden(hidden);
        });
    }

    setCloseButton(button: HTMLButtonElement) {
        if (button) {
            button.addEventListener("click", () =>
                this.close()
            );
        }
    }

    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement) {
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

    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement) {
        this.setHistoryUIBackButton(backButton);
        this.setHistoryUIForwardButton(fwdButton);
    }

    private async setHistoryUIBackButton(backButton: HTMLButtonElement) {
        if (backButton) {
            backButton.addEventListener("click", () => history.back());
            backButton.disabled = !(await this.getCanGoBack());
        }
    }

    private async setHistoryUIForwardButton(fwdButton: HTMLButtonElement) {
        if (fwdButton) {
            fwdButton.addEventListener("click", () => history.forward());
            fwdButton.disabled = !(await this.getCanGoForward());
        }
    }
}