import { __decorate, __metadata } from "tslib";
import { CustomElement } from "@juniper-lib/dom/CustomElement";
import { ID, Name, Open, Query, Slot_attr } from "@juniper-lib/dom/attrs";
import { onToggle } from "@juniper-lib/dom/evts";
import { Details, HtmlRender, LI, Slot, Summary, Template, UL } from "@juniper-lib/dom/tags";
import { EventTargetMixin } from "@juniper-lib/events/EventTarget";
export function TipBox(tipBoxID, ...tips) {
    return HtmlRender(document.createElement("tip-box"), ID(tipBoxID), UL(Slot_attr("tips-slot"), ...tips.map(tip => LI(tip))));
}
const template = Template(Details(Summary("Tips:"), Slot(Name("tips-slot"))));
let TipBoxElement = class TipBoxElement extends HTMLElement {
    constructor() {
        super();
        this.eventTarget = new EventTargetMixin(super.addEventListener.bind(this), super.removeEventListener.bind(this), super.dispatchEvent.bind(this));
    }
    connectedCallback() {
        const storageKey = `Juniper:Widgets:TipBox:${this.id}`;
        const shadowRoot = this.attachShadow({ mode: "closed" });
        const instance = template.content.cloneNode(true);
        shadowRoot.appendChild(instance);
        const details = Details(Query(shadowRoot, "details"), Open(localStorage.getItem(storageKey) !== "closed"), onToggle(() => localStorage.setItem(storageKey, details.open ? "open" : "closed")));
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
};
TipBoxElement = __decorate([
    CustomElement("tip-box"),
    __metadata("design:paramtypes", [])
], TipBoxElement);
export { TipBoxElement };
//# sourceMappingURL=TipBox.js.map