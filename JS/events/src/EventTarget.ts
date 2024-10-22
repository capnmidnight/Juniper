import { arrayClear } from "@juniper-lib/util";
import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/util";

export interface EventMap {
    [type: string]: Event;
}

export interface IEventTarget extends EventTarget {
    clearEventListeners(type?: string): void;
    addBubbler(bubbler: EventTarget): void;
    removeBubbler(bubbler: EventTarget): void;
    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
}

export class CustomEventTarget implements IEventTarget {
    readonly #listeners = new Map<string, EventListenerOrEventListenerObject[]>();
    readonly #listenerOptions = new Map<EventListenerOrEventListenerObject, boolean | AddEventListenerOptions>();
    readonly #bubblers = new Set<EventTarget>();
    readonly #scopes = new WeakMap<object, Array<[any, any]>>();

    addBubbler(bubbler: EventTarget) {
        this.#bubblers.add(bubbler);
    }

    removeBubbler(bubbler: EventTarget) {
        this.#bubblers.delete(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        if (!this.#scopes.has(scope)) {
            this.#scopes.set(scope, []);
        }
        this.#scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback as any, options);
    }

    removeScope(scope: object) {
        const listeners = this.#scopes.get(scope);
        if (listeners) {
            this.#scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }

    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        let listeners = this.#listeners.get(type);
        if (!listeners) {
            listeners = new Array<EventListenerOrEventListenerObject>();
            this.#listeners.set(type, listeners);
        }

        if (!listeners.find((c) => c === callback)) {
            listeners.push(callback);

            if (options) {
                this.#listenerOptions.set(callback, options);
            }
        }
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
        const listeners = this.#listeners.get(type);
        if (listeners) {
            this.#removeListener(listeners, callback);
        }
    }

    clearEventListeners(type?: string) {
        for (const [evtName, handlers] of this.#listeners) {
            if (isNullOrUndefined(type) || type === evtName) {
                for (const handler of handlers) {
                    this.removeEventListener(type, handler);
                }
                arrayClear(handlers);
                this.#listeners.delete(evtName);
            }
        }
    }

    #removeListener(listeners: EventListenerOrEventListenerObject[], callback: EventListenerOrEventListenerObject) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            listeners.splice(idx, 1);
            if (this.#listenerOptions.has(callback)) {
                this.#listenerOptions.delete(callback);
            }
        }
    }

    dispatchEvent(evt: Event): boolean {
        const listeners = this.#listeners.get(evt.type);
        if (listeners) {
            for (const callback of listeners) {
                const options = this.#listenerOptions.get(callback);
                if (isDefined(options)
                    && !isBoolean(options)
                    && options.once) {
                    this.#removeListener(listeners, callback);
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

        for (const bubbler of this.#bubblers) {
            if (!bubbler.dispatchEvent(evt)) {
                return false;
            }
        }

        return true;
    }
}

export class EventTargetMixin implements IEventTarget {
    readonly #listeners = new Map<string, EventListenerOrEventListenerObject[]>();
    readonly #listenerOptions = new Map<EventListenerOrEventListenerObject, boolean | AddEventListenerOptions>();
    readonly #bubblers = new Set<EventTarget>();
    readonly #scopes = new WeakMap<object, Array<[any, any]>>();
    readonly #addEventListener: (type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions) => void;
    readonly #removeEventListener: (type: string, callback: EventListenerOrEventListenerObject) => void;
    readonly #dispatchEvent: (evt: Event) => boolean;

    constructor(
        addEventListener: (type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions) => void,
        removeEventListener: (type: string, callback: EventListenerOrEventListenerObject) => void,
        dispatchEvent: (evt: Event) => boolean
    ) {
        this.#addEventListener = addEventListener;
        this.#removeEventListener = removeEventListener;
        this.#dispatchEvent = dispatchEvent;
    }

    hasEventListeners(evtName: string): boolean {
        return this.#listeners.has(evtName)
            && this.#listeners.get(evtName).length > 0;
    }

    addBubbler(bubbler: EventTarget): void {
        this.#bubblers.add(bubbler);
    }

    removeBubbler(bubbler: EventTarget): void {
        this.#bubblers.delete(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        if (!this.#scopes.has(scope)) {
            this.#scopes.set(scope, []);
        }
        this.#scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback as any, options);
    }

    removeScope(scope: object) {
        const listeners = this.#scopes.get(scope);
        if (listeners) {
            this.#scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }

    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        let listeners = this.#listeners.get(type);
        if (!listeners) {
            listeners = new Array<EventListenerOrEventListenerObject>();
            this.#listeners.set(type, listeners);
        }

        if (!listeners.find((c) => c === callback)) {
            listeners.push(callback);

            if (options) {
                this.#listenerOptions.set(callback, options);
            }
        }

        this.#addEventListener(type, callback, options);
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void {
        const listeners = this.#listeners.get(type);
        if (listeners) {
            this.#removeListener(listeners, callback);
        }

        this.#removeEventListener(type, callback);
    }

    #removeListener(listeners: EventListenerOrEventListenerObject[], callback: EventListenerOrEventListenerObject) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            listeners.splice(idx, 1);
            if (this.#listenerOptions.has(callback)) {
                this.#listenerOptions.delete(callback);
            }
        }
    }

    clearEventListeners(type?: string): void {
        for (const [evtName, handlers] of this.#listeners) {
            if (isNullOrUndefined(type) || type === evtName) {
                for (const handler of handlers) {
                    this.removeEventListener(type, handler);
                }
                arrayClear(handlers);
                this.#listeners.delete(evtName);
            }
        }
    }

    dispatchEvent(evt: Event): boolean {
        const result = this.#dispatchEvent(evt);

        const listeners = this.#listeners.get(evt.type);
        if (listeners) {
            for (const callback of listeners) {
                const options = this.#listenerOptions.get(callback);
                if (isDefined(options)
                    && !isBoolean(options)
                    && options.once) {
                    this.#removeListener(listeners, callback);
                }
            }
        }

        if (!result) {
            return false;
        }

        for (const bubbler of this.#bubblers) {
            if (!bubbler.dispatchEvent(evt)) {
                return false;
            }
        }

        return true;
    }
}

/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 * Make sure to decorate your custom element with the `@CustomElement` decorator.
 * 
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export abstract class CustomHTMLElementExample extends HTMLElement implements IEventTarget {

    readonly #eventTarget: EventTargetMixin;

    constructor() {
        super();
        this.#eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    override addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addEventListener(type, callback, options);
    }

    override removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
        this.#eventTarget.removeEventListener(type, callback);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.#eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: EventTarget) {
        this.#eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: EventTarget) {
        this.#eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.#eventTarget.addScopedEventListener(scope, type, callback, options);
    }

    removeScope(scope: object) {
        this.#eventTarget.removeScope(scope);
    }

    clearEventListeners(type?: string): void {
        this.#eventTarget.clearEventListeners(type);
    }
}


