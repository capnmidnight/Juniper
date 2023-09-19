import { ID, Name, Open, Query, Slot_attr } from "@juniper-lib/dom/dist/attrs";
import { onToggle } from "@juniper-lib/dom/dist/evts";
import { Details, HtmlRender, LI, Slot, Summary, Template, UL } from "@juniper-lib/dom/dist/tags";
import { EventTargetMixin, IEventTarget } from "@juniper-lib/events/dist/EventTarget";

export function TipBox(tipBoxID: string, ...tips: string[]) {
    return HtmlRender(
        document.createElement("tip-box"),
        ID(tipBoxID),
        UL(
            Slot_attr("tips-slot"),
            ...tips.map(tip => LI(tip))
        )
    ) as TipBoxElement;
}

const template = Template(
    Details(
        Summary("Tips:"),
        Slot(Name("tips-slot"))
    )
);

export class TipBoxElement extends HTMLElement implements IEventTarget {

    private readonly eventTarget: EventTargetMixin;

    constructor() {
        super();

        this.eventTarget = new EventTargetMixin(
            super.addEventListener.bind(this),
            super.removeEventListener.bind(this),
            super.dispatchEvent.bind(this)
        );
    }

    connectedCallback() {

        const storageKey = `Juniper:Widgets:TipBox:${this.id}`;

        const shadowRoot = this.attachShadow({ mode: "closed" });
        const instance = template.content.cloneNode(true) as DocumentFragment;
        shadowRoot.appendChild(instance);

        const details = Details(Query(shadowRoot, "details"),
            Open(localStorage.getItem(storageKey) !== "closed"),
            onToggle(() =>
                localStorage.setItem(
                    storageKey,
                    details.open ? "open" : "closed")
            )
        );
    }

    override addEventListener(type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addEventListener(type, callback, options);
    }

    override removeEventListener(type: string, callback: EventListenerOrEventListenerObject) {
        this.eventTarget.removeEventListener(type, callback);
    }

    override dispatchEvent(evt: Event): boolean {
        return this.eventTarget.dispatchEvent(evt);
    }

    addBubbler(bubbler: EventTarget) {
        this.eventTarget.addBubbler(bubbler);
    }

    removeBubbler(bubbler: EventTarget) {
        this.eventTarget.removeBubbler(bubbler);
    }

    addScopedEventListener(scope: object, type: string, callback: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void {
        this.eventTarget.addScopedEventListener(scope, type, callback, options);
    }

    removeScope(scope: object) {
        this.eventTarget.removeScope(scope);
    }

    clearEventListeners(type?: string): void {
        this.eventTarget.clearEventListeners(type);
    }
}

customElements.define("tip-box", TipBoxElement);