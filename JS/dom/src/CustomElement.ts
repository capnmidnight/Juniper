import { EventTargetMixin } from "@juniper-lib/events/dist/EventTarget";
import { ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";

/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 * 
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export abstract class TypedHTMLElementExample<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement implements ITypedEventTarget<EventMapT> {

    private readonly eventTarget: EventTargetMixin;
    constructor() {
        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    override addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void {
        this.eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void {
        this.eventTarget.clearEventListeners(type as string);
    }
}