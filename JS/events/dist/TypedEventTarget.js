import { CustomEventTarget } from "./EventTarget";
export class TypedEvent extends Event {
    get type() {
        return super.type;
    }
    constructor(type, eventInitDict) {
        super(type, eventInitDict);
    }
    get target() {
        return super.target;
    }
}
export class TypedEventTarget extends CustomEventTarget {
    addBubbler(bubbler) {
        super.addBubbler(bubbler);
    }
    removeBubbler(bubbler) {
        super.removeBubbler(bubbler);
    }
    addScopedEventListener(scope, type, callback, options) {
        super.addScopedEventListener(scope, type, callback, options);
    }
    addEventListener(type, callback, options) {
        super.addEventListener(type, callback, options);
    }
    removeEventListener(type, callback) {
        super.removeEventListener(type, callback);
    }
    clearEventListeners(type) {
        return super.clearEventListeners(type);
    }
}
//# sourceMappingURL=TypedEventTarget.js.map