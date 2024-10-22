import { ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events";
export declare abstract class TypedHTMLElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement implements ITypedEventTarget<EventMapT & HTMLElementEventMap>, EventTarget {
    #private;
    constructor();
    addEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>): void;
    dispatchEvent(evt: Event): boolean;
    hasEventListeners(evtName: string): boolean;
    addBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLElementEventMap>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLElementEventMap>): void;
    addScopedEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type?: EventTypeT): void;
}
//# sourceMappingURL=TypedHTMLElement.d.ts.map