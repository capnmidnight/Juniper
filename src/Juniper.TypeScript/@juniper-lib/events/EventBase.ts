import { arrayClear, arrayRemoveAt } from "@juniper-lib/collections/arrays";
import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

export interface EventMap {
    [type: string]: Event;
}

export class EventBase implements EventTarget {
    private readonly listeners = new Map<string, EventListenerOrEventListenerObject[]>();
    private readonly listenerOptions = new Map<EventListenerOrEventListenerObject, boolean | AddEventListenerOptions>();
    private readonly bubblers = new Set<EventTarget>();
    private readonly scopes = new WeakMap<object, Array<[any, any]>>();

    addBubbler(bubbler: EventTarget) {
        this.bubblers.add(bubbler);
    }

    removeBubbler(bubbler: EventTarget) {
        this.bubblers.delete(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        if (!this.scopes.has(scope)) {
            this.scopes.set(scope, []);
        }
        this.scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback as any, options);
    }

    removeScope(scope: object) {
        const listeners = this.scopes.get(scope);
        if (listeners) {
            this.scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }

    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        let listeners = this.listeners.get(type);
        if (!listeners) {
            listeners = new Array<EventListenerOrEventListenerObject>();
            this.listeners.set(type, listeners);
        }

        if (!listeners.find((c) => c === callback)) {
            listeners.push(callback);

            if (options) {
                this.listenerOptions.set(callback, options);
            }
        }
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
        const listeners = this.listeners.get(type);
        if (listeners) {
            this.removeListener(listeners, callback);
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

    private removeListener(listeners: EventListenerOrEventListenerObject[], callback: EventListenerOrEventListenerObject) {
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

                if (isFunction(callback)) {
                    callback.call(this, evt);
                }
                else {
                    callback.handleEvent(evt);
                }
            }
        }

        if (evt.defaultPrevented) {
            return false;
        }

        for (const bubbler of this.bubblers) {
            if (!bubbler.dispatchEvent(evt)) {
                return false;
            }
        }

        return true;
    }
}
export class HTMLElementBase extends HTMLElement {
    private readonly listeners = new Map<string, EventListenerOrEventListenerObject[]>();
    private readonly listenerOptions = new Map<EventListenerOrEventListenerObject, boolean | AddEventListenerOptions>();
    private readonly bubblers = new Set<EventTarget>();
    private readonly scopes = new WeakMap<object, Array<[any, any]>>();

    addBubbler(bubbler: EventTarget) {
        this.bubblers.add(bubbler);
    }

    removeBubbler(bubbler: EventTarget) {
        this.bubblers.delete(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        if (!this.scopes.has(scope)) {
            this.scopes.set(scope, []);
        }
        this.scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback as any, options);
    }

    removeScope(scope: object) {
        const listeners = this.scopes.get(scope);
        if (listeners) {
            this.scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }

    override addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        let listeners = this.listeners.get(type);
        if (!listeners) {
            listeners = new Array<EventListenerOrEventListenerObject>();
            this.listeners.set(type, listeners);
        }

        if (!listeners.find((c) => c === callback)) {
            listeners.push(callback);

            if (options) {
                this.listenerOptions.set(callback, options);
            }
        }

        super.addEventListener(type, callback, options);
    }

    override removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
        const listeners = this.listeners.get(type);
        if (listeners) {
            this.removeListener(listeners, callback);
        }

        super.removeEventListener(type, callback);
    }

    private removeListener(listeners: EventListenerOrEventListenerObject[], callback: EventListenerOrEventListenerObject) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            arrayRemoveAt(listeners, idx);
            if (this.listenerOptions.has(callback)) {
                this.listenerOptions.delete(callback);
            }
        }
    }

    clearEventListeners(type?: string): void {
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

    override dispatchEvent(evt: Event): boolean {
        const result = super.dispatchEvent(evt);

        const listeners = this.listeners.get(evt.type);
        if (listeners) {
            for (const callback of listeners) {
                const options = this.listenerOptions.get(callback);
                if (isDefined(options)
                    && !isBoolean(options)
                    && options.once) {
                    this.removeListener(listeners, callback);
                }
            }
        }

        if (!result) {
            return false;
        }

        for (const bubbler of this.bubblers) {
            if (!bubbler.dispatchEvent(evt)) {
                return false;
            }
        }

        return true;
    }
}