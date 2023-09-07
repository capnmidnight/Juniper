import { EventListenerOpts } from "@juniper-lib/dom/evts";
import { ElementChild } from "@juniper-lib/dom/tags";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/TypedEventTarget";
import "./styles.css";
export declare class TabPanelTabSelectedEvent<TabNames> extends TypedEvent<"tabselected"> {
    tabname: TabNames;
    constructor(tabname: TabNames);
}
type TabPanelEvents<TabNames> = {
    tabselected: TabPanelTabSelectedEvent<TabNames>;
};
export declare function TabPanel<TabNames extends string>(...rest: ElementChild[]): TabPanelElement<TabNames>;
export declare function onTabSelected<TabNames>(callback: (evt: TabPanelTabSelectedEvent<TabNames>) => void, opts?: EventListenerOpts): import("@juniper-lib/dom/evts").HtmlEvt<TabPanelTabSelectedEvent<TabNames>>;
export declare class TabPanelElement<TabNames extends string> extends HTMLElement implements ITypedEventTarget<TabPanelEvents<TabNames>> {
    private readonly eventTarget;
    private readonly panels;
    private readonly panelNames;
    private readonly buttons;
    private readonly controls;
    private readonly inner;
    private curTab;
    private _disabled;
    constructor();
    connectedCallback(): void;
    disconnectedCallback(): void;
    get enabled(): boolean;
    set enabled(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    isSelected(name: TabNames): boolean;
    select(name: TabNames): void;
    addEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: ITypedEventTarget<TabPanelEvents<TabNames>>): void;
    removeBubbler(bubbler: ITypedEventTarget<TabPanelEvents<TabNames>>): void;
    addScopedEventListener<EventTypeT extends keyof TabPanelEvents<TabNames>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TabPanelEvents<TabNames>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners<EventTypeT extends keyof TabPanelEvents<TabNames>>(type?: EventTypeT): void;
}
export {};
//# sourceMappingURL=index.d.ts.map