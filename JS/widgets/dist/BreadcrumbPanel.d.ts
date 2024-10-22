import { ElementChild } from "@juniper-lib/dom";
export declare class BreadcrumbPanelChangedEvent extends Event {
    readonly panel: HTMLElement;
    constructor(panel: HTMLElement);
}
export declare class BreadcrumbPanelElement extends HTMLElement {
    #private;
    constructor();
    connectedCallback(): void;
    getPanel(index: number | string): HTMLElement;
    get currentStep(): number;
    get currentPanel(): HTMLElement;
    next(title: string): void;
    back(): void;
    reset(): void;
    get panelCount(): number;
    static install(): import("@juniper-lib/dom").ElementFactory<BreadcrumbPanelElement>;
}
export declare function BreadcrumbPanel(...rest: ElementChild<BreadcrumbPanelElement>[]): BreadcrumbPanelElement;
//# sourceMappingURL=BreadcrumbPanel.d.ts.map