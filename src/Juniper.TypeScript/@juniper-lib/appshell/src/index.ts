import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import "./header.css";
import "./index.css";
import "./main.css";
import "./nav.css";

export class AppShell {
    constructor(private readonly fetcher: IFetcher) {

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

    setMenuUI(mainNav: HTMLElement, button: HTMLButtonElement) {
        mainNav.classList.toggle("hidden", localStorage.getItem("menuHidden") == "true");
        button.addEventListener("click", () => {
            mainNav.classList.toggle("hidden");
            const hidden = mainNav.classList.contains("hidden");
            localStorage.setItem("menuHidden", hidden ? "true" : "false");
            this.setMenuHidden(hidden);
        });
    }

    setBodyUI(scrollToTop: HTMLButtonElement, scrollToBottom: HTMLButtonElement, article: HTMLElement) {
        function indicateScroll() {
            const scrollTopVisible = article.scrollTop > 25;
            const scrollBottomVisible = (article.scrollTop + article.clientHeight) < article.scrollHeight - 25;

            scrollToTop.style.opacity = scrollTopVisible ? "1" : "0";
            scrollToTop.style.pointerEvents = scrollTopVisible ? "" : "none";
            scrollToBottom.style.opacity = scrollBottomVisible ? "1" : "0";
            scrollToBottom.style.pointerEvents = scrollBottomVisible ? "" : "none";
        }

        scrollToTop.addEventListener("click", () => article.scroll({
            behavior: "smooth",
            top: 0
        }));

        scrollToBottom.addEventListener("click", () => article.scroll({
            behavior: "smooth",
            top: article.scrollHeight
        }));

        article.addEventListener("scroll", indicateScroll);

        indicateScroll();
    }

    setHistoryUI(backButton: HTMLButtonElement, fwdButton: HTMLButtonElement) {
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