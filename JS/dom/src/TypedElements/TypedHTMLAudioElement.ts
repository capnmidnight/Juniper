import { EventTargetMixin, ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events";


export abstract class TypedHTMLAudioElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLAudioElement implements ITypedEventTarget<EventMapT & HTMLMediaElementEventMap> {

    readonly #eventTarget: EventTargetMixin;

    constructor() {
        super();

        this.#eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    override addEventListener<EventTypeT extends keyof (EventMapT & HTMLMediaElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLMediaElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof (EventMapT & HTMLMediaElementEventMap)>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLMediaElementEventMap, EventTypeT>): void {
        this.#eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.#eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLMediaElementEventMap>): void {
        this.#eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<EventMapT & HTMLMediaElementEventMap>): void {
        this.#eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof (EventMapT & HTMLMediaElementEventMap)>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT & HTMLMediaElementEventMap, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.#eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof (EventMapT & HTMLMediaElementEventMap)>(type?: EventTypeT): void {
        this.#eventTarget.clearEventListeners(type as string);
    }
}
