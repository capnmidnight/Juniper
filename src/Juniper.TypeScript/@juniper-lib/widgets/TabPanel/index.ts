import { Attr, ClassList, CustomData, ID } from "@juniper-lib/dom/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/css";
import { ButtonSmall, Div, Elements, ErsatzElement, elementApply, elementSetClass, elementSetDisplay, getElements, isDisableable, resolveElement } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/EventBase";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

import "./styles.css";


export class TabPanelTabSelectedEvent<TabNames> extends TypedEvent<"tabselected">{
    constructor(public tabname: TabNames) {
        super("tabselected");
    }
}

interface TabPanelEvents<TabNames> {
    tabselected: TabPanelTabSelectedEvent<TabNames>;
}

type TabPanelEntry<TabNames> = [TabNames, string, Elements<HTMLElement>];

interface TabPanelView {
    button: HTMLButtonElement;
    panel: Elements<HTMLElement>;
    displayType: CSSGlobalValue | CSSDisplayValue;
}

function isRule<TabNames>(obj: TabPanelEntry<TabNames> | CssElementStyleProp | Attr): obj is CssElementStyleProp | Attr {
    return obj instanceof CssElementStyleProp
        || obj instanceof Attr;
}

function isViewDef<TabNames>(obj: TabPanelEntry<TabNames> | CssElementStyleProp | Attr): obj is TabPanelEntry<TabNames> {
    return !isRule(obj);
}

export class TabPanel<TabNames extends string>
    extends TypedEventBase<TabPanelEvents<TabNames>>
    implements ErsatzElement<HTMLElement> {
    private readonly views = new Map<TabNames, TabPanelView>();

    private curTab: TabNames = null;
    private _disabled = false;

    public static find() {
        return Array.from(TabPanel._find());
    }

    private static *_find() {
        for (const elem of getElements(".tab-panel")) {
            yield new TabPanel(elem);
        }
    }

    public static create<TabNames extends string>(...entries: (TabPanelEntry<TabNames> | CssElementStyleProp | Attr)[]): TabPanel<TabNames> {
        const rules = entries.filter(isRule);
        const viewDefs = entries.filter(isViewDef);
        const viewsByName = new Map<TabNames, TabPanelView>();

        let firstName: TabNames = null;
        for (const viewDef of viewDefs) {
            const [name, label, panel] = viewDef;
            const panelName = CustomData("panelname", name);
            if (isNullOrUndefined(firstName)) {
                firstName = name;
            }
            const elem = resolveElement<HTMLElement>(panel);
            const displayType = elem.style.display as CSSDisplayValue;
            elementApply(panel, ID(name));
            viewsByName.set(name, {
                panel,
                displayType,
                button: ButtonSmall(
                    label,
                    panelName
                )
            });
        }

        const views = Array.from(viewsByName.values());

        return new TabPanel<TabNames>(Div(
            ClassList("tab-panel"),
            ...rules,
            Div(
                ClassList("tabs"),
                ...views.map(p => p.button)
            ),
            Div(
                ClassList("panels"),
                ...views.map(p => p.panel)
            )
        ));
    }

    constructor(public readonly element: HTMLElement) {
        super();

        let counter = 0;
        const btns: [string, HTMLButtonElement][] = [...element.querySelectorAll<HTMLButtonElement>(".tabs > button")]
            .map(btn => [btn.dataset.panelname || `tab${++counter}`, btn]);
        const buttons = new Map<string, HTMLButtonElement>(btns);

        counter = 0;
        const panels = new Map<string, HTMLElement>(
            [...element.querySelectorAll<HTMLElement>(".panels > *")]
                .map(panel => [panel.id || `tab${++counter}`, panel])
        );

        for (const [name, button] of buttons) {
            const panel = panels.get(name);
            button.addEventListener("click", () => {
                this.select(name as TabNames);
                this.dispatchEvent(new TabPanelTabSelectedEvent(name));
            });
            let displayType = panel.style.display as CSSDisplayValue;
            if (displayType = "none") {
                displayType = null;
            }
            this.views.set(name as TabNames, {
                button,
                displayType,
                panel
            });
        }

        if (btns.length > 0) {
            this.select(btns[0][0] as TabNames);
        }
    }

    get enabled(): boolean {
        return !this.disabled;
    }

    set enabled(v) {
        this.disabled = !v;
    }

    get disabled() {
        return this._disabled;
    }

    set disabled(v: boolean) {
        this._disabled = v;

        for (const [name, view] of this.views) {
            view.button.disabled = v || name === this.curTab;
            elementSetClass(view.panel, v, "disabled");
            if (isDisableable(view.panel)) {
                view.panel.disabled = v;
            }
            else {
                const elem = resolveElement(view.panel);
                if (isDisableable(elem)) {
                    elem.disabled = v;
                }
            }
        }
    }

    isSelected(name: TabNames): boolean {
        return this.curTab === name;
    }

    select(name: TabNames): void {
        if (this.views.has(name)) {
            this.curTab = name;
            for (const [name, view] of this.views) {
                const visible = name === this.curTab;
                view.button.disabled = visible || this.disabled;
                elementSetDisplay(view.panel, visible, view.displayType);
            }
        }
    }
}
