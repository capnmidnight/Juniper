export declare class EventBase implements EventTarget {
    private readonly listeners;
    private readonly listenerOptions;
    constructor();
    addEventListener(type: string, callback: (evt: Event) => any, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: (evt: Event) => any): void;
    private removeListener;
    dispatchEvent(evt: Event): boolean;
}
export declare class TypedEvent<T extends string> extends Event {
    get type(): T;
    constructor(type: T);
}
export declare class TypedEventBase<EventsT> extends EventBase {
    private readonly bubblers;
    private readonly scopes;
    addBubbler(bubbler: TypedEventBase<EventsT>): void;
    removeBubbler(bubbler: TypedEventBase<EventsT>): void;
    addEventListener<K extends keyof EventsT & string>(type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<K extends keyof EventsT & string>(type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any): void;
    addScopedEventListener<K extends keyof EventsT & string>(scope: object, type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any, options?: boolean | AddEventListenerOptions): void;
    removeScope<K extends keyof EventsT & string>(scope: object): void;
    clearEventListeners<K extends keyof EventsT & string>(type?: K): void;
    dispatchEvent<T extends Event>(evt: T): boolean;
}
