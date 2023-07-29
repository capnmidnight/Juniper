import { EventBase, EventMap, HTMLElementBase } from "./EventBase";

export class TypedEvent<EventTypeT extends string> extends Event {
    override get type(): EventTypeT {
        return super.type as EventTypeT;
    }

    constructor(type: EventTypeT, eventInitDict?: EventInit) {
        super(type, eventInitDict);
    }
}

export type TypedEventMap<EventTypeT extends string> =
    EventMap
    | Record<EventTypeT, TypedEvent<EventTypeT>>;

type TypedEventHandler<EventT> =
    (evt: EventT) => void;

export type TypedEventListener<EventMapT, EventTypeT extends keyof EventMapT> =
    TypedEventHandler<EventMapT[EventTypeT]>;

export interface TypedEventListenerObject<EventMapT, EventTypeT extends keyof EventMapT> {
    handleEvent: TypedEventListener<EventMapT, EventTypeT>;
}

export type TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT extends keyof EventMapT> =
    TypedEventListener<EventMapT, EventTypeT>
    | TypedEventListenerObject<EventMapT, EventTypeT>;

export interface TypedEventTarget<EventMapT extends TypedEventMap<string>> extends EventTarget {
    addBubbler(bubbler: TypedEventTarget<EventMapT>): void;

    removeBubbler(bubbler: TypedEventTarget<EventMapT>): void;

    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;

    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;

    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;

    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
}

export class TypedEventBase<EventMapT extends TypedEventMap<string>> extends EventBase implements TypedEventTarget<EventMapT> {
    override addBubbler(bubbler: TypedEventTarget<EventMapT>) {
        super.addBubbler(bubbler);
    }

    override removeBubbler(bubbler: TypedEventTarget<EventMapT>) {
        super.removeBubbler(bubbler);
    }

    override addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        super.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        super.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void {
        super.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void {
        return super.clearEventListeners(type as string);
    }
}

export class TypedHTMLElement<EventMapT extends TypedEventMap<string>> extends HTMLElementBase implements TypedEventTarget<EventMapT> {
    override addBubbler(bubbler: TypedEventTarget<EventMapT>) {
        super.addBubbler(bubbler);
    }

    override removeBubbler(bubbler: TypedEventTarget<EventMapT>) {
        super.removeBubbler(bubbler);
    }

    override addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        super.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        super.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void {
        super.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void {
        return super.clearEventListeners(type as string);
    }
}