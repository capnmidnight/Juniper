import { mapBuild, TypedEvent, TypedEventBase } from "juniper-tslib";
import { buttonSetEnabled } from "./buttonSetEnabled";
import { borderBottom, borderBottomColor, borderRadius, boxShadow, display, flexDirection, marginBottom, paddingTop, rule, zIndex } from "./css";
import { elementSetClass } from "./elementSetClass";
import { Elements, elementSetDisplay, ErsatzElements, Style } from "./tags";

export class TabControlTabSelectedEvent extends TypedEvent<"tabselected">{
    constructor(public tabname: string) {
        super("tabselected");
    }
}

interface TabControlEvents {
    tabselected: TabControlTabSelectedEvent;
}

Style(
    rule(".tabs",
        display("flex"),
        flexDirection("row"),
        borderBottom("solid 1px #6c757d"),
        paddingTop("5px")
    ),

    rule(".tabs > button",
        borderRadius("5px 5px 0 0"),
        marginBottom("-1px")
    ),

    rule(".tabs > button.btn-secondary",
        zIndex(0)
    ),

    rule(".tabs > button.btn-outline-secondary",
        borderBottomColor("white"),
        boxShadow("#ccc 0 -5px 10px"),
        zIndex(1)
    )
);

export class TabControl
    extends TypedEventBase<TabControlEvents>
    implements ErsatzElements {
    private curTab: string = null;
    private views = new Array<HTMLElement>();
    private selectors = new Map<string, () => void>();
    private tabButtons: HTMLButtonElement[];
    private displayTypes: Map<HTMLElement, string>;
    private _enabled = true;

    public readonly elements: Elements[];

    constructor(tabButtonRoot: HTMLElement, private readonly tabPanelRoot: HTMLElement, private buttonStyle: string = "secondary") {
        super();

        this.elements = [
            tabButtonRoot,
            this.tabPanelRoot
        ];

        this.tabButtons = Array.from(tabButtonRoot.querySelectorAll("button"));
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

        this.displayTypes = mapBuild(this.views, v => v.style.display);

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
                || tabButton.classList.contains(`btn-outline-${this.buttonStyle}`);
        }

        elementSetClass(
            this.tabPanelRoot,
            !v,
            "disabled");
    }

    isSelected(name: string): boolean {
        return this.curTab === name;
    }

    select(name: string): void {
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
                this.buttonStyle);
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
