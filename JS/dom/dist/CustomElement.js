import { EventTargetMixin } from "@juniper-lib/events/dist/EventTarget";
/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 *
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export class TypedHTMLElementExample extends HTMLElement {
    constructor() {
        super();
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
    }
    addEventListener(type, callback, options) {
        this.eventTarget.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        this.eventTarget.removeEventListener(type, callback);
    }
    dispatchEvent(evt) {
        return this.eventTarget.dispatchEvent(evt);
    }
    addBubbler(bubbler) {
        this.eventTarget.addBubbler(bubbler);
    }
    removeBubbler(bubbler) {
        this.eventTarget.removeBubbler(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        this.eventTarget.addScopedEventListener(scope, type, callback, options);
    }
    removeScope(scope) {
        this.eventTarget.removeScope(scope);
    }
    clearEventListeners(type) {
        this.eventTarget.clearEventListeners(type);
    }
}
//# sourceMappingURL=CustomElement.js.map