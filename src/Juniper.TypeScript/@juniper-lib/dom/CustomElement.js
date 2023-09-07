import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
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
];
export function CustomElement(tagName, extendsTag) {
    return function (Type) {
        if (extendsTag) {
            if (mayAttachShadowRoot.indexOf(extendsTag) < 0 && /\.attachShadow\b/.test(Type.toString())) {
                console.warn(`The <${extendsTag}> tag may not have a shadowRoot attached`);
            }
            customElements.define(tagName, Type, { extends: extendsTag });
        }
        else {
            customElements.define(tagName, Type);
        }
    };
}
/**
 * Don't actually instantiate or subclass this class. Use it as template to create new classes.
 * Make sure to decorate your custom element with the `@CustomElement` decorator.
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