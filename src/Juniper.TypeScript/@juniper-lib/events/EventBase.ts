import { arrayClear, arrayRemoveAt } from "@juniper-lib/collections/arrays";
import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

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

export class EventBaseMixin implements IEventTarget {
    private readonly listeners = new Map<string, EventListenerOrEventListenerObject[]>();
    private readonly listenerOptions = new Map<EventListenerOrEventListenerObject, boolean | AddEventListenerOptions>();
    private readonly bubblers = new Set<EventTarget>();
    private readonly scopes = new WeakMap<object, Array<[any, any]>>();

    constructor(
        private readonly _addEventListener: (type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions) => void,
        private readonly _removeEventListener: (type: string, callback: EventListenerOrEventListenerObject) => void,
        private readonly _dispatchEvent: (evt: Event) => boolean
    ) {
    }

    addBubbler(bubbler: EventTarget): void {
        this.bubblers.add(bubbler);
    }

    removeBubbler(bubbler: EventTarget): void {
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

        this._addEventListener(type, callback, options);
    }

    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void {
        const listeners = this.listeners.get(type);
        if (listeners) {
            this.removeListener(listeners, callback);
        }

        this._removeEventListener(type, callback);
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

    dispatchEvent(evt: Event): boolean {
        const result = this._dispatchEvent(evt);

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

export function HTMLElementBase<BaseElementT extends CustomElementConstructor>(Base: BaseElementT) {
    return class extends Base implements IEventTarget {
        readonly eventTarget: EventBaseMixin;
        constructor(..._rest: any[]) {
            super();
            this.eventTarget = new EventBaseMixin(
                super.addEventListener.bind(this),
                super.removeEventListener.bind(this),
                super.dispatchEvent.bind(this)
            );
        }

        override addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
            this.eventTarget.addEventListener(type, callback, options);
        }

        override removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
            this.eventTarget.removeEventListener(type, callback);
        }

        override dispatchEvent(evt: Event): boolean {
            return this.eventTarget.dispatchEvent(evt);
        }

        addBubbler(bubbler: EventTarget) {
            this.eventTarget.addBubbler(bubbler);
        }

        removeBubbler(bubbler: EventTarget) {
            this.eventTarget.removeBubbler(bubbler);
        }

        addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
            this.eventTarget.addScopedEventListener(scope, type, callback, options);
        }

        removeScope(scope: object) {
            this.eventTarget.removeScope(scope);
        }

        clearEventListeners(type?: string): void {
            this.eventTarget.clearEventListeners(type);
        }
    }
}