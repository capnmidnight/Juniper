import { CustomElement } from "@juniper-lib/dom/CustomElement";
import { ClassList, QueryAll } from "@juniper-lib/dom/attrs";
import { EventListenerOpts, onClick, onEvent } from "@juniper-lib/dom/evts";
import { ButtonSmall, Div, ElementChild, HtmlTag, elementSetClass, elementSetDisplay, isDisableable, resolveElement } from "@juniper-lib/dom/tags";
import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/TypedEventTarget";
import "./styles.css";


export class TabPanelTabSelectedEvent<TabNames> extends TypedEvent<"tabselected">{
    constructor(public tabname: TabNames) {
        super("tabselected");
    }
}

type TabPanelEvents<TabNames> = {
    tabselected: TabPanelTabSelectedEvent<TabNames>;
}

export function TabPanel<TabNames extends string>(...rest: ElementChild[]) {
    return HtmlTag<{
        "tab-panel": TabPanelElement<TabNames>
    }>("tab-panel", ...rest);
}

export function onTabSelected<TabNames>(callback: (evt: TabPanelTabSelectedEvent<TabNames>) => void, opts?: EventListenerOpts) { return onEvent("tabselected", callback, opts); }


@CustomElement("tab-panel")
export class TabPanelElement<TabNames extends string>
    extends HTMLElement
    implements ITypedEventTarget<TabPanelEvents<TabNames>> {
    private readonly eventTarget: EventTargetMixin;
    private readonly panels: Map<TabNames, HTMLElement>;
    private readonly panelNames: TabNames[];
    private readonly buttons = new Map<TabNames, HTMLButtonElement>();
    private readonly controls: HTMLElement;
    private readonly inner: HTMLElement;
    private curTab: TabNames = null;
    private _disabled = false;
    constructor() {
        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );

        this.panels = new Map(
            QueryAll<HTMLElement>(this, ":scope > [data-tab-name]")
                .map(d => [d.dataset.tabName as TabNames, d])
        );

        this.panelNames = Array.from(this.panels.keys());

        this.buttons = new Map(this.panelNames
            .map(name => {
                const evt = new TabPanelTabSelectedEvent(name);
                return [name, ButtonSmall(
                    name,
                    onClick(() => {
                        this.select(name);
                        this.dispatchEvent(evt);
                    })
                )];
            }));

        this.controls = Div(
            ClassList("tabs"),
            ...Array.from(this.buttons.values())
        );

        this.inner = Div(
            ClassList("panels")
        );
    }

    connectedCallback() {
        this.append(
            this.controls,
            this.inner
        );

        this.inner.append(
            ...Array.from(this.panels.values())
        );

        if (this.panelNames.length > 0) {
            this.select(this.panelNames[0]);
        }
    }

    disconnectedCallback() {
        this.controls.remove();
        this.inner.replaceWith(
            ...Array.from(this.panels.values())
        );
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

        for (const [name, panel] of this.panels) {
            const button = this.buttons.get(name);
            button.disabled = v || name === this.curTab;
            elementSetClass(panel, v, "disabled");
            if (isDisableable(panel)) {
                panel.disabled = v;
            }
            else {
                const elem = resolveElement(panel);
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
        if (this.panels.has(name)) {
            this.curTab = name;
            for (const [name, panel] of this.panels) {
                const visible = name === this.curTab;
                const button = this.buttons.get(name);
                button.disabled = visible || this.disabled;
                elementSetDisplay(panel, visible);
            }
        }
    }

    override addEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>): void {
        this.eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<TabPanelEvents<TabNames>>): void {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<TabPanelEvents<TabNames>>): void {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof TabPanelEvents<TabNames>>(type?: EventTypeT): void {
        this.eventTarget.clearEventListeners(type as string);
    }
}
