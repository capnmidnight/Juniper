import { CustomEventTarget, EventMap, IEventTarget } from "./EventTarget";

export class TypedEvent<EventTypeT extends string, TargetT extends EventTarget = EventTarget> extends Event {
    override get type(): EventTypeT {
        return super.type as EventTypeT;
    }

    constructor(type: EventTypeT, eventInitDict?: EventInit) {
        super(type, eventInitDict);
    }

    override get target() {
        return super.target as TargetT;
    }
}

export type TypedEventMap<EventTypeT> =
    EventMap
    | Record<string & EventTypeT, TypedEvent<string & EventTypeT>>;

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

export interface ITypedEventTarget<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends IEventTarget {
    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;
}

export class TypedEventTarget<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends CustomEventTarget implements ITypedEventTarget<EventMapT> {
    override addBubbler(bubbler: ITypedEventTarget<EventMapT>) {
        super.addBubbler(bubbler);
    }

    override removeBubbler(bubbler: ITypedEventTarget<EventMapT>) {
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
