import { Attr, className } from "@juniper-lib/dom/attrs";
import { CssProp } from "@juniper-lib/dom/css";
import { Div, ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";
import "./styles";

function isRule(obj: ElementChild): obj is CssProp | Attr {
    return obj instanceof CssProp
        || obj instanceof Attr;
}

function isElem(obj: ElementChild): obj is Exclude<ElementChild, CssProp | Attr> {
    return !isRule(obj);
}


export class SplitPanel
    implements ErsatzElement {
    readonly element: HTMLElement;

    constructor(...rest: ElementChild[]) {

        const rules = rest.filter(isRule);
        const elems = rest.filter(isElem);

        this.element = Div(
            className("scroll-panel"),
            ...rules,
            Div(
                className("scroll-panel-inner"),
                ...elems
            )
        )
    }
}
