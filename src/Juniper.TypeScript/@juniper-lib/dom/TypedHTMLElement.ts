import { HTMLElementBase } from "@juniper-lib/events/EventBase";
import { TypedEventMap, ITypedEventTarget, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/TypedEventBase";

export interface ITypedHTMLElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement {
    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void;
    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void;
    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void;
}

export function TypedHTMLElement<BaseElementT extends CustomElementConstructor>(tagExpr: [string] | [string, string], Base: BaseElementT) {
    const [tagName, extendsTag] = tagExpr;
    return class CustomElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElementBase(Base) implements ITypedHTMLElement<EventMapT> {

        static get TagName() { return tagName; }
        static get ExtendsTag() { return extendsTag; }

        static create() {
            if (extendsTag) {
                return document.createElement(tagName, { is: extendsTag });
            }
            else {
                return document.createElement(tagName);
            }
        }

        override addBubbler(bubbler: ITypedEventTarget<EventMapT>) {
            super.addBubbler(bubbler);
        }

        override removeBubbler(bubbler: ITypedEventTarget<EventMapT>) {
            super.removeBubbler(bubbler);
        }

        override addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
            super.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
        }

        override addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
            super.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
        }

        override removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void {
            super.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
        }

        override clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void {
            return super.clearEventListeners(type as string);
        }
    };
}

export function CustomElement<T extends CustomElementConstructor & { TagName: string; ExtendsTag: string; }>(Type: T) {
    console.log(Type);
    if (Type.ExtendsTag) {
        customElements.define(Type.TagName, Type, { extends: Type.ExtendsTag });
    }
    else {
        customElements.define(Type.TagName, Type);
    }

    return Type;
}
