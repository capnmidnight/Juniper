import { CustomEventTarget, EventMap, IEventTarget } from "./EventTarget";
export declare class TypedEvent<EventTypeT extends string, TargetT extends EventTarget = EventTarget> extends Event {
    get type(): EventTypeT;
    constructor(type: EventTypeT, eventInitDict?: EventInit);
    get target(): TargetT;
}
export type TypedEventMap<EventTypeT> = EventMap | Record<string & EventTypeT, TypedEvent<string & EventTypeT>>;
type TypedEventHandler<EventT> = (evt: EventT) => void;
export type TypedEventListener<EventMapT, EventTypeT extends keyof EventMapT> = TypedEventHandler<EventMapT[EventTypeT]>;
export interface TypedEventListenerObject<EventMapT, EventTypeT extends keyof EventMapT> {
    handleEvent: TypedEventListener<EventMapT, EventTypeT>;
}
export type TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT extends keyof EventMapT> = TypedEventListener<EventMapT, EventTypeT> | TypedEventListenerObject<EventMapT, EventTypeT>;
export interface ITypedEventTarget<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends IEventTarget {
    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;
}
export declare class TypedEventTarget<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends CustomEventTarget implements ITypedEventTarget<EventMapT> {
    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;
    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
}
export {};
//# sourceMappingURL=TypedEventTarget.d.ts.map