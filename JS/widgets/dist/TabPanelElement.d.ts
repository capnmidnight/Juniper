import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
export declare class TabPanelTabShownEvent extends Event {
    #private;
    get tabName(): string;
    constructor(tabName: string);
}
type EventMap = {
    "tabshown": TabPanelTabShownEvent;
};
export declare class TabPanelElement extends TypedHTMLElement<EventMap> {
    #private;
    static observedAttributes: string[];
    get tabNames(): readonly string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    clearTabs(): void;
    addTab(tabName: string, beforeTab?: string): HTMLElement;
    removeTab(tabName: string): void;
    protected show(tabName: string, emitEvent: boolean): void;
    set selectedTab(v: string);
    get selectedTab(): string;
    getTab(tabName: string): HTMLElement;
    get orientation(): string;
    set orientation(v: string);
    get disabled(): boolean;
    set disabled(v: boolean);
    getTabDisabled(tabName: string): boolean;
    setTabDisabled(tabName: string, disabled: boolean): void;
    getTabVisible(tabName: string): boolean;
    setTabVisible(tabName: string, visible: boolean): void;
    static install(): import("@juniper-lib/dom").ElementFactory<TabPanelElement>;
}
export declare function TabPanel(...rest: ElementChild<TabPanelElement>[]): TabPanelElement;
export declare function TabPane(name: string, ...rest: ElementChild[]): HTMLDivElement;
export {};
//# sourceMappingURL=TabPanelElement.d.ts.map