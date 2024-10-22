import { AppShell } from "@juniper-lib/appshell";
import { Article, Button, ID, Query } from "@juniper-lib/dom";
import "./index.css";
import "./main.css";
import "./nav.css";
import "./tabs.css";

const appShell = new AppShell();
const [scrollToTop, scrollToBottom] = document.querySelectorAll<HTMLButtonElement>(".scrollIndicator");

appShell.setMenuUI(
    document.querySelector<HTMLElement>("main > nav"),
    Button(ID("mainNavVisibilityControlButton"))
);

appShell.setHistoryUI(
    document.querySelector<HTMLButtonElement>("#backButton"),
    document.querySelector<HTMLButtonElement>("#fwdButton")
);

appShell.setBodyUI(
    scrollToTop,
    scrollToBottom,
    Article(Query("#content > article"))
);