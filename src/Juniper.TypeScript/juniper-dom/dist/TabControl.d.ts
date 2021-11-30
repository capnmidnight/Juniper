import { TypedEvent, TypedEventBase } from "juniper-tslib";
import { ErsatzElements } from "./tags";
export declare class TabControlTabSelectedEvent extends TypedEvent<"tabselected"> {
    tabname: string;
    constructor(tabname: string);
}
interface TabControlEvents {
    tabselected: TabControlTabSelectedEvent;
}
export declare class TabControl extends TypedEventBase<TabControlEvents> implements ErsatzElements {
    private readonly tabPanelRoot;
    private buttonStyle;
    private curTab;
    private views;
    private selectors;
    private tabButtons;
    private displayTypes;
    private _enabled;
    readonly elements: HTMLElement[];
    constructor(tabButtonRoot: HTMLElement, tabPanelRoot: HTMLElement, buttonStyle?: string);
    get enabled(): boolean;
    set enabled(v: boolean);
    isSelected(name: string): boolean;
    select(name: string): void;
    private activate;
}
export {};
