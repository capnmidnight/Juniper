import { distinct, singleton } from "@juniper-lib/util";
import { Button, ClassList, Clear, colorScheme, DataList, display, Div, flexDirection, fontSize, gap, HtmlRender, I, InputSearch, ListAttr, OnClick, OnInput, Option, PlaceHolder, pointerEvents, registerFactory, rule, SingletonStyleBlob, TitleAttr, TypedHTMLElement } from "@juniper-lib/dom";
import { upwardsPariedArrows } from "@juniper-lib/emoji";
import { NavTreeItemElement } from "./NavTreeItemElement";
class NavTreeElement extends TypedHTMLElement {
    #controls;
    #autocompleteList;
    #search;
    #observer;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::NavTree", () => rule("nav-tree", display("block"), rule(" .controls", display("flex"), flexDirection("row"), gap(".5em"), rule(">input", colorScheme("light")), rule(">button", fontSize("12pt"), rule(">i", pointerEvents("none"))))));
        this.#controls = Div(ClassList("controls"), this.#search = InputSearch(TitleAttr("Search menu"), PlaceHolder("Search..."), ListAttr(this.#autocompleteList = DataList()), OnInput(() => {
            const term = this.#search.value.toLowerCase();
            const items = this.items;
            const toHighlight = [];
            const toDehighlight = new Set();
            for (const item of items) {
                item.classList.remove("highlighted", "dehighlighted");
                if (term.length > 0) {
                    const value = item.label.toLowerCase();
                    if (value.indexOf(term) >= 0) {
                        toHighlight.push(item);
                    }
                    else {
                        toDehighlight.add(item);
                    }
                }
            }
            while (toHighlight.length > 0) {
                const item = toHighlight.shift();
                item.classList.add("highlighted");
                for (let here = item; here instanceof NavTreeItemElement; here = here.parentElement) {
                    here.open = true;
                    toDehighlight.delete(here);
                }
            }
            for (const item of toDehighlight) {
                item.open = false;
                item.classList.add("dehighlighted");
            }
        })), Button(ClassList("borderless"), TitleAttr("Collapse all..."), I(upwardsPariedArrows.value), OnClick(() => {
            for (const element of this.items) {
                element.open = false;
            }
        })));
        this.addEventListener("itemselected", evt => {
            for (const item of this.items) {
                if (item !== evt.item) {
                    item.selected = false;
                }
            }
        });
        this.#observer = new MutationObserver(mutations => {
            const toRefresh = new Set();
            const queue = mutations.flatMap(m => [
                m.target,
                ...m.addedNodes
            ]);
            while (queue.length > 0) {
                const here = queue.shift();
                if (here instanceof NavTreeItemElement
                    && !toRefresh.has(here)) {
                    toRefresh.add(here);
                    queue.push(here.parentElement);
                }
            }
            for (const element of toRefresh) {
                element._refresh();
            }
        });
    }
    #setup = false;
    connectedCallback() {
        if (!this.#setup) {
            this.#setup = true;
            this.insertBefore(this.#controls, this.children[0]);
            const controls = Array.from(this.children)
                .filter(e => e.slot === "controls");
            this.#controls.append(...controls);
            const labels = distinct(Array
                .from(this.items)
                .map(e => e.label));
            HtmlRender(this.#autocompleteList, Clear(), ...labels.map(l => Option(l)));
            this.#observer.observe(this, { subtree: true, childList: true });
        }
    }
    get items() { return this.querySelectorAll("nav-tree-item"); }
    select(type, value) {
        const item = this.getItem(type, value);
        if (item instanceof NavTreeItemElement) {
            item.selected = true;
        }
    }
    clearSelection() {
        for (const element of this.querySelectorAll("nav-tree-item[selected]")) {
            element.selected = false;
        }
    }
    getItem(type, value) {
        return this.querySelector(`nav-tree-item[type='${type}'][value='${value}']`);
    }
    hasItemByLabel(type, label) {
        label = label.toLowerCase();
        for (const item of this.querySelectorAll(`nav-tree-item[type="${type}"]`)) {
            if (item.label.toLowerCase() === label) {
                return true;
            }
        }
        return false;
    }
    static install() {
        NavTreeItemElement.install();
        return singleton("Juniper::Widgets::NavTreeElement", () => registerFactory("nav-tree", NavTreeElement));
    }
}
export function NavTree(...rest) {
    return NavTreeElement.install()(...rest);
}
//# sourceMappingURL=NavTreeElement.js.map