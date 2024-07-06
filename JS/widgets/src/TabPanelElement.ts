import { first, singleton } from "@juniper-lib/util";
import { ClassList, Div, ElementChild, HtmlTag, OnClick, QueryAll, StyleBlob, alignItems, boxShadow, display, flexDirection, flexGrow, overflow, position, rule, zIndex } from "@juniper-lib/dom";
import { ButtonPrimary } from "./widgets";

export function TabPanel(...rest: ElementChild[]): TabPanelElement {
    return HtmlTag("tab-panel", ...rest);
}

export class TabPanelTabShownEvent extends Event {

    #tabName: string;
    get tabName(): string { return this.#tabName; }

    constructor(tabName: string) {
        super("tabshown");
        this.#tabName = tabName;
    }
}

export class TabPanelElement extends HTMLElement {

    #tabNames: string[];
    get tabNames(): readonly string[] { return this.#tabNames; }
    #tabs: Map<string, HTMLElement>;
    #buttons: Map<string, HTMLButtonElement> = new Map();
    #disabledTabs: Map<string, boolean> = new Map();
    constructor() {
        super();
        singleton("Juniper:Widgets:TabPanelElement", () => {
            document.head.append(StyleBlob(
                rule("tab-panel",
                    display("flex"),
                    flexDirection("column"),
                    overflow("hidden"),
                    alignItems("stretch"),
                    flexGrow(1)
                ),
                rule("tab-panel > .tab-panel-controls",
                    display("flex"),
                    flexDirection("row")
                ),
                rule("tab-panel[orientation=horizontal]",
                    flexDirection("row")
                ),
                rule("tab-panel[orientation=horizontal] > .tab-panel-controls",
                    flexDirection("column")
                ),
                rule("tab-panel > [data-tab-name]",
                    flexGrow(1),
                    display("flex"),
                    flexDirection("column"),
                    overflow("auto")
                ),
                rule("tab-panel > .tab-panel-controls > .btn-primary",
                    position("relative")
                ),
                rule("tab-panel > .tab-panel-controls > .btn-primary:not(.selected)",
                    zIndex(0)
                ),
                rule("tab-panel > .tab-panel-controls > .btn-primary.selected",
                    boxShadow("0 0 5px black"),
                    zIndex(1)
                )
            ));

            return true;
        });

        this.#tabs = new Map(QueryAll<HTMLElement>(this, ":scope > [data-tab-name]")
            .map((e) => [e.dataset.tabName, e as HTMLElement]));

        this.#tabNames = Array.from(this.#tabs.keys());

        for (const tabName of this.#tabNames) {
            this.#buttons.set(tabName, ButtonPrimary(
                `Show ${tabName} tab`,
                tabName,
                OnClick(() => this.#show(tabName, true))
            ));

            this.#disabledTabs.set(tabName, false);
        }
    }

    connectedCallback(): void {
        if (!this.querySelector(":scope>.tab-panel-controls")) {
            this.insertAdjacentElement("afterbegin", Div(
                ClassList("tab-panel-controls"),
                ...Array.from(this.#buttons.values())
            ));
        }

        if (!this.disabled) {
            this.#show(first(this.#tabNames), true);
        }
    }

    #show(tabName: string, emitEvent: boolean): void {
        for (const t of this.#tabNames) {
            const button = this.#buttons.get(t);
            const tab = this.#tabs.get(t);
            const isSelected = t === tabName;

            button.disabled = isSelected || this.disabled || this.#disabledTabs.get(t);
            button.classList.toggle("selected", isSelected);
            tab.style.display = isSelected ? "" : "none";
            if (isSelected && emitEvent) {
                this.dispatchEvent(new TabPanelTabShownEvent(tabName));
            }
        }
    }

    set selectedTab(v) {
        this.#show(v, false);
    }

    get selectedTab(): string {
        for (const t of this.#tabNames) {
            const button = this.#buttons.get(t);
            if (button.classList.contains("selected")) {
                return t;
            }
        }

        return null;
    }

    getTab(tabName: string): HTMLElement {
        return this.#tabs.get(tabName);
    }

    get orientation(): string {
        return this.getAttribute("orientation") === "horizontal" ? "horizontal" : "vertical";
    }

    set orientation(v) {
        this.setAttribute("orientation", v === "horizontal" ? "horizontal" : "vertical");
    }

    get disabled(): boolean {
        return this.hasAttribute("disabled");
    }

    set disabled(v) {
        if (this.disabled != v) {
            if (v) {
                this.setAttribute("disabled", "");
            }
            else {
                this.removeAttribute("disabled");
            }

            this.#show(this.selectedTab, !this.selectedTab);
        }
    }

    getTabDisabled(tabName: string): boolean {
        return this.#disabledTabs.get(tabName);
    }

    setTabDisabled(tabName: string, disabled: boolean) {
        this.#disabledTabs.set(tabName, disabled);
        this.#show(this.selectedTab, false);
    }
}

customElements.define("tab-panel", TabPanelElement);