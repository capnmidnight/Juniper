import { className, customData } from "@juniper-lib/dom/attrs";
import { ButtonSecondary, buttonSetEnabled, Div, Elements, elementSetClass, elementSetDisplay, ErsatzElement, ErsatzElements } from "@juniper-lib/dom/tags";
import { isString, mapBuild, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";

import "./styles";

export class TabControlTabSelectedEvent extends TypedEvent<"tabselected">{
    constructor(public tabname: string) {
        super("tabselected");
    }
}

interface TabControlEvents {
    tabselected: TabControlTabSelectedEvent;
}

export interface ITabPanel extends ErsatzElement {
    tabName: string;
    buttonLabel: string;
}

export function TabControl(...tabPanels: ITabPanel[]): TabControlElement {
    const tabButtons = tabPanels.map((panel) => {
        return ButtonSecondary(
            customData("tabname", panel.tabName),
            panel.buttonLabel
        );
    });

    for (const tabPanel of tabPanels) {
        tabPanel.element.classList.add("tab-content");
        tabPanel.element.classList.add(tabPanel.tabName);
    }

    tabButtons[0].className = "btn btn-outline-secondary";
    tabButtons[0].disabled = true;

    return new TabControlElement(
        Div(className("tabs"), ...tabButtons),
        Div(className("tab-container"), ...tabPanels)
    );
}

export class TabControlElement
    extends TypedEventBase<TabControlEvents>
    implements ErsatzElements {
    private curTab: string = null;
    private views = new Array<HTMLElement>();
    private selectors = new Map<string, () => void>();
    private tabButtons: HTMLButtonElement[];
    private displayTypes: Map<HTMLElement, string>;
    private _enabled = true;

    public readonly elements: Elements[];

    constructor(public readonly tabButtonRoot: HTMLElement, public readonly tabPanelRoot: HTMLElement) {
        super();

        this.elements = [
            this.tabButtonRoot,
            this.tabPanelRoot
        ];

        this.tabButtons = Array.from(this.tabButtonRoot.querySelectorAll("button"));
        let firstViewSelector: () => void = null;
        for (const tabButton of this.tabButtons) {
            const name = tabButton.dataset.tabname;
            const view = this.tabPanelRoot.querySelector<HTMLElement>(`.${name}`);
            const selector = this.activate.bind(this, tabButton, name);

            tabButton.addEventListener("click", selector);

            this.views.push(view);
            this.selectors.set(name, selector);

            if (firstViewSelector === null
                || tabButton.disabled) {
                firstViewSelector = selector;
            }
        }

        this.displayTypes = mapBuild(this.views, (v) => v.style.display);

        if (firstViewSelector) {
            firstViewSelector();
        }
    }

    get enabled(): boolean {
        return this._enabled;
    }

    set enabled(v: boolean) {
        this._enabled = v;
        for (const tabButton of this.tabButtons) {
            tabButton.disabled = !v
                || tabButton.classList.contains("btn-outline-secondary");
        }

        elementSetClass(
            this.tabPanelRoot,
            !v,
            "disabled");
    }

    isSelected(name: string | ITabPanel): boolean {
        if (!isString(name)) {
            name = name.tabName;
        }
        return this.curTab === name;
    }

    select(name: string | ITabPanel): void {
        if (!isString(name)) {
            name = name.tabName;
        }
        const wasEnabled = this.enabled;
        const selector = this.selectors.get(name);
        if (selector) {
            selector();
        }
        this.enabled = wasEnabled;
    }

    private activate(tabButton: HTMLButtonElement, name: string): void {
        this.curTab = name;

        for (const otherTabButton of this.tabButtons) {
            buttonSetEnabled(
                otherTabButton,
                otherTabButton !== tabButton,
                "secondary");
        }

        for (const view of this.views) {
            const visible = view.classList.contains(name);

            elementSetDisplay(
                view,
                visible,
                this.displayTypes.get(view));
        }

        this.dispatchEvent(new TabControlTabSelectedEvent(name));
    }
}
