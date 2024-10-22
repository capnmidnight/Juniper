import { EventTargetMixin, ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events";

export abstract class TypedHTMLElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement implements ITypedEventTarget<EventMapT & HTMLElementEventMap>, EventTarget {

    readonly #eventTarget: EventTargetMixin;

    constructor() {
        super();

        this.#eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    override addEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>): void {
        this.#eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.#eventTarget.dispatchEvent(evt);
    }

    hasEventListeners(evtName: string): boolean {
        return this.#eventTarget.hasEventListeners(evtName);
    }

    addBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLElementEventMap>): void {
        this.#eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLElementEventMap>): void {
        this.#eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.#eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof (EventMapT & HTMLElementEventMap)>(type?: EventTypeT): void {
        this.#eventTarget.clearEventListeners(type as string);
    }
}
