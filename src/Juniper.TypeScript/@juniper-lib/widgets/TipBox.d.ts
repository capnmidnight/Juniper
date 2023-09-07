import { IEventTarget } from "@juniper-lib/events/EventTarget";
export declare function TipBox(tipBoxID: string, ...tips: string[]): TipBoxElement;
export declare class TipBoxElement extends HTMLElement implements IEventTarget {
    private readonly eventTarget;
    constructor();
    connectedCallback(): void;
    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: EventTarget): void;
    removeBubbler(bubbler: EventTarget): void;
    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners(type?: string): void;
}
//# sourceMappingURL=TipBox.d.ts.map