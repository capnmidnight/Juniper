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
export declare class CustomEventTarget implements IEventTarget {
    #private;
    addBubbler(bubbler: EventTarget): void;
    removeBubbler(bubbler: EventTarget): void;
    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void;
    clearEventListeners(type?: string): void;
    dispatchEvent(evt: Event): boolean;
}
export declare class EventTargetMixin implements IEventTarget {
    #private;
    constructor(addEventListener: (type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions) => void, removeEventListener: (type: string, callback: EventListenerOrEventListenerObject) => void, dispatchEvent: (evt: Event) => boolean);
    hasEventListeners(evtName: string): boolean;
    addBubbler(bubbler: EventTarget): void;
    removeBubbler(bubbler: EventTarget): void;
    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void;
    clearEventListeners(type?: string): void;
    dispatchEvent(evt: Event): boolean;
}
/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 * Make sure to decorate your custom element with the `@CustomElement` decorator.
 *
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export declare abstract class CustomHTMLElementExample extends HTMLElement implements IEventTarget {
    #private;
    constructor();
    addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener(type: string, callback: EventListenerOrEventListenerObject): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: EventTarget): void;
    removeBubbler(bubbler: EventTarget): void;
    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners(type?: string): void;
}
//# sourceMappingURL=EventTarget.d.ts.map