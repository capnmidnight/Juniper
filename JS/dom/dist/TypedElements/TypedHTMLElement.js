import { EventTargetMixin } from "@juniper-lib/events";
export class TypedHTMLElement extends HTMLElement {
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
    hasEventListeners(evtName) {
        return this.#eventTarget.hasEventListeners(evtName);
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
//# sourceMappingURL=TypedHTMLElement.js.map