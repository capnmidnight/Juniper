import { arrayClear } from "../collections/arrayClear";
import { arrayRemoveAt } from "../collections/arrayRemoveAt";
import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "../typeChecks";

type EventCallback = (evt: Event) => any;

export class EventBase implements EventTarget {
    private readonly listeners = new Map<string, EventCallback[]>();
    private readonly listenerOptions = new Map<EventCallback, boolean | AddEventListenerOptions>();

    addEventListener(type: string, callback: (evt: Event) => any, options?: boolean | AddEventListenerOptions): void {
        if (isFunction(callback)) {
            let listeners = this.listeners.get(type);
            if (!listeners) {
                listeners = new Array<EventCallback>();
                this.listeners.set(type, listeners);
            }

            if (!listeners.find((c) => c === callback)) {
                listeners.push(callback);

                if (options) {
                    this.listenerOptions.set(callback, options);
                }
            }
        }
    }

    removeEventListener(type: string, callback: (evt: Event) => any) {
        if (isFunction(callback)) {
            const listeners = this.listeners.get(type);
            if (listeners) {
                this.removeListener(listeners, callback);
            }
        }
    }

    clearEventListeners(type?: string) {
        for (const [evtName, handlers] of this.listeners) {
            if (isNullOrUndefined(type) || type === evtName) {
                for (const handler of handlers) {
                    this.removeEventListener(type, handler);
                }
                arrayClear(handlers);
                this.listeners.delete(evtName);
            }
        }
    }

    private removeListener(listeners: EventCallback[], callback: EventCallback) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            arrayRemoveAt(listeners, idx);
            if (this.listenerOptions.has(callback)) {
                this.listenerOptions.delete(callback);
            }
        }
    }

    dispatchEvent(evt: Event): boolean {
        const listeners = this.listeners.get(evt.type);
        if (listeners) {
            for (const callback of listeners) {
                const options = this.listenerOptions.get(callback);
                if (isDefined(options)
                    && !isBoolean(options)
                    && options.once) {
                    this.removeListener(listeners, callback);
                }

                callback.call(this, evt);
            }
        }
        return !evt.defaultPrevented;
    }
}

export class TypedEvent<T extends string> extends Event {

    override get type(): T {
        return super.type as T;
    }

    constructor(type: T, eventInitDict?: EventInit) {
        super(type, eventInitDict);
    }
}

export class TypedEventBase<EventsT> extends EventBase {
    private readonly bubblers = new Set<TypedEventBase<EventsT>>();
    private readonly scopes = new WeakMap<object, Array<[any, any]>>();

    addBubbler(bubbler: TypedEventBase<EventsT>) {
        this.bubblers.add(bubbler);
    }

    removeBubbler(bubbler: TypedEventBase<EventsT>) {
        this.bubblers.delete(bubbler);
    }

    override addEventListener<K extends keyof EventsT & string>(type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any, options?: boolean | AddEventListenerOptions): void {
        super.addEventListener(type, callback as any, options);
    }

    override removeEventListener<K extends keyof EventsT & string>(type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any) {
        super.removeEventListener(type, callback as any);
    }

    override clearEventListeners<K extends keyof EventsT & string>(type?: K): void {
        return super.clearEventListeners(type);
    }

    addScopedEventListener<K extends keyof EventsT & string>(scope: object, type: K, callback: (evt: TypedEvent<K> & EventsT[K]) => any, options?: boolean | AddEventListenerOptions): void {
        if (!this.scopes.has(scope)) {
            this.scopes.set(scope, []);
        }
        this.scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback as any, options);
    }

    removeScope<K extends keyof EventsT & string>(scope: object) {
        const listeners = this.scopes.get(scope);
        if (listeners) {
            this.scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type as K, listener);
            }
        }
    }

    override dispatchEvent<T extends Event>(evt: T): boolean {
        if (!super.dispatchEvent(evt)) {
            return false;
        }

        if (evt.bubbles && !evt.cancelBubble) {
            for (const bubbler of this.bubblers) {
                if (!bubbler.dispatchEvent(evt)) {
                    return false;
                }
            }
        }

        return true;
    }
}