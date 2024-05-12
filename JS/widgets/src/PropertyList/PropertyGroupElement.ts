import { ElementChild, HtmlTag, Name } from "@juniper-lib/dom";


export class PropertyGroupElement extends HTMLElement {
    get name() { return this.getAttribute("name"); }
    set name(v) { this.setAttribute("name", v); }
}
customElements.define("property-group", PropertyGroupElement);

export function PropertyGroup(name: string, ...rest: ElementChild[]) {
    return HtmlTag(
        "property-group",
        Name(name),
        ...rest
    ) as PropertyGroupElement;
}
