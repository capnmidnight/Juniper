import { Attr, className } from "@juniper-lib/dom/attrs";
import { CssProp } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { ButtonSecondarySmall, Div, Elements, elementSetClass, elementSetDisplay, ErsatzElement, isDisableable, resolveElement } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import "./styles";


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
    displayType: string;
}

function isRule<TabNames>(obj: TabPanelEntry<TabNames> | CssProp | Attr): obj is CssProp | Attr {
    return obj instanceof CssProp
        || obj instanceof Attr;
}

function isViewDef<TabNames>(obj: TabPanelEntry<TabNames> | CssProp | Attr): obj is TabPanelEntry<TabNames> {
    return !isRule(obj);
}

export class TabPanel<TabNames>
    extends TypedEventBase<TabPanelEvents<TabNames>>
    implements ErsatzElement<HTMLElement> {
    private readonly views = new Map<TabNames, TabPanelView>();

    private curTab: TabNames = null;
    private _disabled = false;

    readonly element: HTMLElement;

    constructor(...entries: (TabPanelEntry<TabNames> | CssProp | Attr)[]) {
        super();

        const rules = entries.filter(isRule);
        const viewDefs = entries.filter(isViewDef);

        let firstName: TabNames = null;
        for (const viewDef of viewDefs) {
            const [name, label, panel] = viewDef;
            if (isNullOrUndefined(firstName)) {
                firstName = name;
            }
            const elem = resolveElement<HTMLElement>(panel);
            const displayType = elem.style.display;
            this.views.set(name, {
                panel,
                displayType,
                button: ButtonSecondarySmall(
                    label,
                    onClick(() => {
                        this.select(name);
                        this.dispatchEvent(new TabPanelTabSelectedEvent(name));
                    })
                )
            });
        }

        const views = Array.from(this.views.values());

        this.element = Div(
            className("tab-panel"),
            ...rules,
            Div(
                className("tabs"),
                ...views.map(p => p.button)
            ),
            Div(
                className("panels"),
                ...views.map(p => p.panel)
            )
        );

        if (firstName) {
            this.select(firstName);
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
