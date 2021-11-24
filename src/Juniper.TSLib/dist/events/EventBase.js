import { arrayClear, arrayRemoveAt, isBoolean, isDefined, isFunction, isNullOrUndefined } from "../";
const allListeners = new WeakMap();
export class EventBase {
    listeners = new Map();
    listenerOptions = new Map();
    constructor() {
        allListeners.set(this, this.listeners);
    }
    addEventListener(type, callback, options) {
        if (isFunction(callback)) {
            let listeners = this.listeners.get(type);
            if (!listeners) {
                listeners = new Array();
                this.listeners.set(type, listeners);
            }
            if (!listeners.find(c => c === callback)) {
                listeners.push(callback);
                if (options) {
                    this.listenerOptions.set(callback, options);
                }
            }
        }
    }
    removeEventListener(type, callback) {
        if (isFunction(callback)) {
            const listeners = this.listeners.get(type);
            if (listeners) {
                this.removeListener(listeners, callback);
            }
        }
    }
    removeListener(listeners, callback) {
        const idx = listeners.findIndex(c => c === callback);
        if (idx >= 0) {
            arrayRemoveAt(listeners, idx);
            if (this.listenerOptions.has(callback)) {
                this.listenerOptions.delete(callback);
            }
        }
    }
    dispatchEvent(evt) {
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
export class TypedEvent extends Event {
    get type() {
        return super.type;
    }
    constructor(type) {
        super(type);
    }
}
export class TypedEventBase extends EventBase {
    bubblers = new Set();
    scopes = new WeakMap();
    addBubbler(bubbler) {
        this.bubblers.add(bubbler);
    }
    removeBubbler(bubbler) {
        this.bubblers.delete(bubbler);
    }
    addEventListener(type, callback, options) {
        super.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        super.removeEventListener(type, callback);
    }
    addScopedEventListener(scope, type, callback, options) {
        if (!this.scopes.has(scope)) {
            this.scopes.set(scope, []);
        }
        this.scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback, options);
    }
    removeScope(scope) {
        const listeners = this.scopes.get(scope);
        if (listeners) {
            this.scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }
    clearEventListeners(type) {
        const listeners = allListeners.get(this);
        for (const [evtName, handlers] of listeners) {
            if (isNullOrUndefined(type) || type === evtName) {
                arrayClear(handlers);
                listeners.delete(evtName);
            }
        }
    }
    dispatchEvent(evt) {
        if (!super.dispatchEvent(evt)) {
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
