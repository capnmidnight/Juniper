import { mapBuild, TypedEvent, TypedEventBase } from "juniper-tslib";
import { buttonSetEnabled } from "./buttonSetEnabled";
import { borderBottom, borderBottomColor, borderRadius, boxShadow, display, flexDirection, marginBottom, paddingTop, rule, zIndex } from "./css";
import { elementSetClass } from "./elementSetClass";
import { elementSetDisplay, Style } from "./tags";
export class TabControlTabSelectedEvent extends TypedEvent {
    tabname;
    constructor(tabname) {
        super("tabselected");
        this.tabname = tabname;
    }
}
Style(rule(".tabs", display("flex"), flexDirection("row"), borderBottom("solid 1px #6c757d"), paddingTop("5px")), rule(".tabs > button", borderRadius("5px 5px 0 0"), marginBottom("-1px")), rule(".tabs > button.btn-secondary", zIndex(0)), rule(".tabs > button.btn-outline-secondary", borderBottomColor("white"), boxShadow("#ccc 0 -5px 10px"), zIndex(1)));
export class TabControl extends TypedEventBase {
    tabPanelRoot;
    buttonStyle;
    curTab = null;
    views = new Array();
    selectors = new Map();
    tabButtons;
    displayTypes;
    _enabled = true;
    elements;
    constructor(tabButtonRoot, tabPanelRoot, buttonStyle = "secondary") {
        super();
        this.tabPanelRoot = tabPanelRoot;
        this.buttonStyle = buttonStyle;
        this.elements = [
            tabButtonRoot,
            this.tabPanelRoot
        ];
        this.tabButtons = Array.from(tabButtonRoot.querySelectorAll("button"));
        let firstViewSelector = null;
        for (const tabButton of this.tabButtons) {
            const name = tabButton.dataset.tabname;
            const view = this.tabPanelRoot.querySelector(`.${name}`);
            const selector = this.activate.bind(this, tabButton, name);
            tabButton.addEventListener("click", selector);
            this.views.push(view);
            this.selectors.set(name, selector);
            if (firstViewSelector === null
                || tabButton.disabled) {
                firstViewSelector = selector;
            }
        }
        this.displayTypes = mapBuild(this.views, v => v.style.display);
        if (firstViewSelector) {
            firstViewSelector();
        }
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        this._enabled = v;
        for (const tabButton of this.tabButtons) {
            tabButton.disabled = !v
                || tabButton.classList.contains(`btn-outline-${this.buttonStyle}`);
        }
        elementSetClass(this.tabPanelRoot, !v, "disabled");
    }
    isSelected(name) {
        return this.curTab === name;
    }
    select(name) {
        const wasEnabled = this.enabled;
        const selector = this.selectors.get(name);
        if (selector) {
            selector();
        }
        this.enabled = wasEnabled;
    }
    activate(tabButton, name) {
        this.curTab = name;
        for (const otherTabButton of this.tabButtons) {
            buttonSetEnabled(otherTabButton, otherTabButton !== tabButton, this.buttonStyle);
        }
        for (const view of this.views) {
            const visible = view.classList.contains(name);
            elementSetDisplay(view, visible, this.displayTypes.get(view));
        }
        this.dispatchEvent(new TabControlTabSelectedEvent(name));
    }
}
