import { debounce } from "@juniper-lib/events/dist/debounce";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import "./header.css";
import "./index.css";
import "./main.css";
import "./nav.css";
export class AppShell {
    constructor(fetcher) {
        this.fetcher = fetcher;
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
            mainNav.classList.toggle("hidden");
            const hidden = mainNav.classList.contains("hidden");
            localStorage.setItem("menuHidden", hidden ? "true" : "false");
            this.setMenuHidden(hidden);
        });
    }
    setBodyUI(scrollToTop, scrollToBottom, article) {
        function indicateScroll() {
            const scrollTopVisible = article.scrollTop > 25;
            const scrollBottomVisible = (article.scrollTop + article.clientHeight) < article.scrollHeight - 25;
            scrollToTop.classList.toggle("visible", scrollTopVisible);
            scrollToBottom.classList.toggle("visible", scrollBottomVisible);
        }
        const indScroll = debounce(indicateScroll);
        scrollToTop.addEventListener("click", () => article.scroll({
            behavior: "smooth",
            top: 0
        }));
        scrollToBottom.addEventListener("click", () => article.scroll({
            behavior: "smooth",
            top: article.scrollHeight
        }));
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