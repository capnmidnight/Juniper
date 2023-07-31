import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
import { ITypedEventTarget, TypedEventListenerOrEventListenerObject, TypedEventMap } from "@juniper-lib/events/TypedEventTarget";

type CustomElementConstructor<T extends HTMLElement> = new () => T;

const mayAttachShadowRoot = [
    "article",
    "aside",
    "blockquote",
    "body",
    "div",
    "footer",
    "h1",
    "h2",
    "h3",
    "h4",
    "h5",
    "h6",
    "header",
    "main",
    "nav",
    "p",
    "section",
    "span"
]

export function CustomElement<ElementT extends HTMLElement = HTMLElement>(tagName: string, extendsTag?: string) {
    return function <CustomElementT extends CustomElementConstructor<ElementT>>(Type: CustomElementT) {
        if (extendsTag) {
            if (mayAttachShadowRoot.indexOf(extendsTag) < 0 && /\.attachShadow\b/.test(Type.toString())) {
                console.warn(`The <${extendsTag}> tag may not have a shadowRoot attached`);
            }
            customElements.define(tagName, Type, { extends: extendsTag });
        }
        else {
            customElements.define(tagName, Type);
        }
    }
}

/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 * Make sure to decorate your custom element with the `@CustomElement` decorator.
 * 
 * I know this sucks, but I can't get the type system to do what I want right now.
 */
export abstract class TypedHTMLElementExample<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends HTMLElement implements ITypedEventTarget<EventMapT> {

    private readonly eventTarget: EventTargetMixin;
    constructor() {
        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    override addEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type as string, callback as EventListenerOrEventListenerObject, options);
    }

    override removeEventListener<EventTypeT extends keyof EventMapT>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>): void {
        this.eventTarget.removeEventListener(type as string, callback as EventListenerOrEventListenerObject);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: ITypedEventTarget<EventMapT>): void {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: ITypedEventTarget<EventMapT>): void {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener<EventTypeT extends keyof EventMapT>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<EventMapT, EventTypeT>, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type as string, callback as EventListenerOrEventListenerObject, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners<EventTypeT extends keyof EventMapT>(type?: EventTypeT): void {
        this.eventTarget.clearEventListeners(type as string);
    }
}