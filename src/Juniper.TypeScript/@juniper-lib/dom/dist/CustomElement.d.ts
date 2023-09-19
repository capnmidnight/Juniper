import { ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 *
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export declare abstract class TypedHTMLElementExample<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement implements ITypedEventTarget<EventMapT> {
    private readonly eventTarget;
    constructor();
    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
}
//# sourceMappingURL=CustomElement.d.ts.map