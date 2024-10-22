import { singleton } from "@juniper-lib/util";
import { Details, ID, LI, Name, OnToggle, Open, Query, registerFactory, SlotAttr, SlotTag, SummaryTag, Template, TypedHTMLElement, UL } from "@juniper-lib/dom";
const template = Template(Details(SummaryTag("Tips:"), SlotTag(Name("tips-slot"))));
export class TipBoxElement extends TypedHTMLElement {
    connectedCallback() {
        const storageKey = `Juniper:Widgets:TipBox:${this.id}`;
        const shadowRoot = this.attachShadow({ mode: "closed" });
        const instance = template.content.cloneNode(true);
        shadowRoot.appendChild(instance);
        const details = Details(Query(shadowRoot, "details"), Open(localStorage.getItem(storageKey) !== "closed"), OnToggle(() => localStorage.setItem(storageKey, details.open ? "open" : "closed")));
    }
    static install() { return singleton("Juniper::Widgets::TipBoxElement", () => registerFactory("tip-box", TipBoxElement)); }
}
export function TipBox(tipBoxID, ...tips) {
    return TipBoxElement.install()(ID(tipBoxID), UL(SlotAttr("tips-slot"), ...tips.map(tip => LI(tip))));
}
//# sourceMappingURL=TipBox.js.map