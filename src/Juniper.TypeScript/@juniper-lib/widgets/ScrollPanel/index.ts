import { Attr, className } from "@juniper-lib/dom/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/css";
import { Div, ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";
import "./styles";

function isRule(obj: ElementChild): obj is CssElementStyleProp | Attr {
    return obj instanceof CssElementStyleProp
        || obj instanceof Attr;
}

function isElem(obj: ElementChild): obj is Exclude<ElementChild, CssElementStyleProp | Attr> {
    return !isRule(obj);
}


export class ScrollPanel
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
