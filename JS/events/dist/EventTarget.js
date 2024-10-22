import { arrayClear } from "@juniper-lib/util";
import { isBoolean, isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/util";
export class CustomEventTarget {
    #listeners = new Map();
    #listenerOptions = new Map();
    #bubblers = new Set();
    #scopes = new WeakMap();
    addBubbler(bubbler) {
        this.#bubblers.add(bubbler);
    }
    removeBubbler(bubbler) {
        this.#bubblers.delete(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        if (!this.#scopes.has(scope)) {
            this.#scopes.set(scope, []);
        }
        this.#scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback, options);
    }
    removeScope(scope) {
        const listeners = this.#scopes.get(scope);
        if (listeners) {
            this.#scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }
    addEventListener(type, callback, options) {
        let listeners = this.#listeners.get(type);
        if (!listeners) {
            listeners = new Array();
            this.#listeners.set(type, listeners);
        }
        if (!listeners.find((c) => c === callback)) {
            listeners.push(callback);
            if (options) {
                this.#listenerOptions.set(callback, options);
            }
        }
    }
    removeEventListener(type, callback) {
        const listeners = this.#listeners.get(type);
        if (listeners) {
            this.#removeListener(listeners, callback);
        }
    }
    clearEventListeners(type) {
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
    #removeListener(listeners, callback) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            listeners.splice(idx, 1);
            if (this.#listenerOptions.has(callback)) {
                this.#listenerOptions.delete(callback);
            }
        }
    }
    dispatchEvent(evt) {
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
export class EventTargetMixin {
    #listeners = new Map();
    #listenerOptions = new Map();
    #bubblers = new Set();
    #scopes = new WeakMap();
    #addEventListener;
    #removeEventListener;
    #dispatchEvent;
    constructor(addEventListener, removeEventListener, dispatchEvent) {
        this.#addEventListener = addEventListener;
        this.#removeEventListener = removeEventListener;
        this.#dispatchEvent = dispatchEvent;
    }
    hasEventListeners(evtName) {
        return this.#listeners.has(evtName)
            && this.#listeners.get(evtName).length > 0;
    }
    addBubbler(bubbler) {
        this.#bubblers.add(bubbler);
    }
    removeBubbler(bubbler) {
        this.#bubblers.delete(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        if (!this.#scopes.has(scope)) {
            this.#scopes.set(scope, []);
        }
        this.#scopes.get(scope).push([type, callback]);
        this.addEventListener(type, callback, options);
    }
    removeScope(scope) {
        const listeners = this.#scopes.get(scope);
        if (listeners) {
            this.#scopes.delete(scope);
            for (const [type, listener] of listeners) {
                this.removeEventListener(type, listener);
            }
        }
    }
    addEventListener(type, callback, options) {
        let listeners = this.#listeners.get(type);
        if (!listeners) {
            listeners = new Array();
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
    removeEventListener(type, callback) {
        const listeners = this.#listeners.get(type);
        if (listeners) {
            this.#removeListener(listeners, callback);
        }
        this.#removeEventListener(type, callback);
    }
    #removeListener(listeners, callback) {
        const idx = listeners.findIndex((c) => c === callback);
        if (idx >= 0) {
            listeners.splice(idx, 1);
            if (this.#listenerOptions.has(callback)) {
                this.#listenerOptions.delete(callback);
            }
        }
    }
    clearEventListeners(type) {
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
    dispatchEvent(evt) {
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
export class CustomHTMLElementExample extends HTMLElement {
    #eventTarget;
    constructor() {
        super();
        this.#eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
    }
    addEventListener(type, callback, options) {
        this.#eventTarget.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        this.#eventTarget.removeEventListener(type, callback);
    }
    dispatchEvent(evt) {
        return this.#eventTarget.dispatchEvent(evt);
    }
    addBubbler(bubbler) {
        this.#eventTarget.addBubbler(bubbler);
    }
    removeBubbler(bubbler) {
        this.#eventTarget.removeBubbler(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        this.#eventTarget.addScopedEventListener(scope, type, callback, options);
    }
    removeScope(scope) {
        this.#eventTarget.removeScope(scope);
    }
    clearEventListeners(type) {
        this.#eventTarget.clearEventListeners(type);
    }
}
//# sourceMappingURL=EventTarget.js.map