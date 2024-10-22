import { arrayClear, arrayReplace, first, isDefined, isString, singleton } from "@juniper-lib/util";
import { ClassList, CustomData, Div, ElementChild, SingletonStyleBlob, TypedHTMLElement, alignItems, boxShadow, display, elementSetDisplay, flexDirection, flexGrow, flexShrink, minHeight, overflow, position, px, registerFactory, rule, zIndex } from "@juniper-lib/dom";
import { ButtonPrimary } from "./widgets";

export class TabPanelTabShownEvent extends Event {

    #tabName: string;
    get tabName(): string { return this.#tabName; }

    constructor(tabName: string) {
        super("tabshown");
        this.#tabName = tabName;
    }
}

type EventMap = {
    "tabshown": TabPanelTabShownEvent;
}

export class TabPanelElement extends TypedHTMLElement<EventMap> {

    static observedAttributes = [
        "disabled"
    ];

    readonly #tabNames: string[] = [];
    get tabNames(): readonly string[] { return this.#tabNames; }
    readonly #tabs: Map<string, HTMLElement> = new Map();
    readonly #buttons: Map<string, HTMLButtonElement> = new Map();
    readonly #disabledTabs: Map<string, boolean> = new Map();
    #initialized = false;

    constructor() {
        super();
        SingletonStyleBlob(`Juniper:Widgets:TabPanelElement:${this.tagName}`, () => [
            rule(this.tagName,
                display("flex"),
                flexDirection("column"),
                overflow("hidden"),
                alignItems("stretch"),
                flexGrow(1)
            ),
            rule(this.tagName + " > .tab-panel-controls",
                display("flex"),
                flexDirection("row"),
                flexShrink(0),
                minHeight(px(35))
            ),
            rule(this.tagName + "[orientation=horizontal]",
                flexDirection("row")
            ),
            rule(this.tagName + "[orientation=horizontal] > .tab-panel-controls",
                flexDirection("column")
            ),
            rule(this.tagName + " > [data-tab-name]",
                flexGrow(1),
                overflow("auto")
            ),
            rule(this.tagName + " > .tab-panel-controls > .btn-primary",
                position("relative")
            ),
            rule(this.tagName + " > .tab-panel-controls > .btn-primary:not(.selected)",
                zIndex(0)
            ),
            rule(this.tagName + " > .tab-panel-controls > .btn-primary.selected",
                boxShadow("0 0 5px black"),
                zIndex(1)
            )
        ]);
    }

    connectedCallback(): void {
        if (!this.#initialized) {
            for (const e of this.querySelectorAll<HTMLElement>(":scope > [data-tab-name]")) {
                this.#tabs.set(e.dataset.tabName, e);
            }

            arrayReplace(this.#tabNames, Array.from(this.#tabs.keys()));

            this.addEventListener("click", (evt) => {
                if (evt.target instanceof HTMLButtonElement
                    && isString(evt.target.dataset.tabName)) {
                    this.show(evt.target.dataset.tabName, true)
                }
            });

            for (const tabName of this.#tabNames) {
                this.#createButton(tabName);
            }

            this.#renderButtons();

            if (!this.disabled) {
                this.show(first(this.#tabNames), true);
            }

            this.#initialized = true;
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        if (name === "disabled") {
            this.show(this.selectedTab, !this.selectedTab);
        }
    }

    clearTabs() {
        for (const tab of this.#tabs.values()) {
            tab.remove();
        }
        this.#tabs.clear();
        arrayClear(this.#tabNames);
        for (const button of this.#buttons.values()) {
            button.remove();
        }
        this.#buttons.clear();
        this.#disabledTabs.clear();
    }

    addTab(tabName: string, beforeTab?: string): HTMLElement {
        let refTab: HTMLElement = null;
        let refIndex: number = this.#tabNames.length;
        if (isDefined(beforeTab)) {
            refTab = this.getTab(beforeTab);
            if (isDefined(refTab)) {
                refIndex = this.#tabNames.indexOf(beforeTab);
            }
        }

        const newTab = Div(CustomData("tab-name", tabName));
        this.insertBefore(newTab, refTab);
        this.#tabs.set(tabName, newTab);
        this.#tabNames.splice(refIndex, 0, tabName);
        this.#createButton(tabName);
        this.#renderButtons();

        return this.getTab(tabName);
    }

    removeTab(tabName: string) {
        const tab = this.getTab(tabName);
        if (isDefined(tab)) {
            tab.remove();
            this.#tabs.delete(tabName);
            const idx = this.#tabNames.indexOf(tabName);
            this.#tabNames.splice(idx, 1);
            this.#buttons.delete(tabName);
            this.#disabledTabs.delete(tabName);
            this.#renderButtons();
        }
    }

    #createButton(tabName: string) {
        this.#buttons.set(tabName, ButtonPrimary(
            `Show ${tabName} tab`,
            tabName,
            CustomData("tab-name", tabName)
        ));

        this.#disabledTabs.set(tabName, false);
    }

    #renderButtons() {
        let controls = this.querySelector(":scope>.tab-panel-controls");
        if (!controls) {
            this.insertAdjacentElement("afterbegin", controls = Div(
                ClassList("tab-panel-controls"),
            ));
        }

        controls.replaceChildren(...this.#tabNames.map(tabName => this.#buttons.get(tabName)));
    }

    protected show(tabName: string, emitEvent: boolean): void {
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
        this.show(v, false);
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

    set disabled(v) { this.toggleAttribute("disabled", v); }

    getTabDisabled(tabName: string): boolean {
        return this.#disabledTabs.get(tabName);
    }

    setTabDisabled(tabName: string, disabled: boolean) {
        this.#disabledTabs.set(tabName, disabled);
        this.show(this.selectedTab, false);
    }

    getTabVisible(tabName: string): boolean {
        const button = this.#buttons.get(tabName);
        return isDefined(button) && button.style.display !== "none";
    }

    setTabVisible(tabName: string, visible: boolean): void {
        const button = this.#buttons.get(tabName);
        if (isDefined(button)) {
            elementSetDisplay(button, visible);
        }
    }

    static install() {
        return singleton("Juniper::Widgets::TabPanelElement", () => registerFactory("tab-panel", TabPanelElement));
    }
}

export function TabPanel(...rest: ElementChild<TabPanelElement>[]) {
    return TabPanelElement.install()(...rest);
}

export function TabPane(name: string, ...rest: ElementChild[]) {
    return Div(
        CustomData("tab-name", name),
        ...rest
    );
}
