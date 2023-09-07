class EventScope {
    constructor(target, eventName, handler) {
        this.target = target;
        this.eventName = eventName;
        this.handler = handler;
        this.target.addEventListener(this.eventName, this.handler);
    }
    dispose() {
        this.target.removeEventListener(this.eventName, this.handler);
    }
}
export function eventScope(target, eventName, eventHandler) {
    return new EventScope(target, eventName, eventHandler);
}
//# sourceMappingURL=eventScope.js.map