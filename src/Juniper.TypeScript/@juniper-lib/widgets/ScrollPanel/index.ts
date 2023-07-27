import { HtmlAttr, ClassList } from "@juniper-lib/dom/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/css";
import { Div, ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";

import "./styles.css";

function isRule(obj: ElementChild): obj is CssElementStyleProp | HtmlAttr {
    return obj instanceof CssElementStyleProp
        || obj instanceof HtmlAttr;
}

function isElem(obj: ElementChild): obj is Exclude<ElementChild, CssElementStyleProp | HtmlAttr> {
    return !isRule(obj);
}


export class ScrollPanel
implements ErsatzElement {
    readonly element: HTMLElement;

    constructor(...rest: ElementChild[]) {

        const rules = rest.filter(isRule);
        const elems = rest.filter(isElem);

        this.element = Div(
            ClassList("scroll-panel"),
            ...rules,
            Div(
                ClassList("scroll-panel-inner"),
                ...elems
            )
        );
    }
}
