import { className } from "@juniper-lib/dom/attrs";
import { display, gridTemplateColumns, rule } from "@juniper-lib/dom/css";
import { Div, ElementChild, ErsatzElement, isElements, Style } from "@juniper-lib/dom/tags";

Style(rule(".group-panel", display("grid")))

export class GroupPanel
    implements ErsatzElement {

    readonly element: HTMLElement;

    constructor(...rest: ElementChild[]) {

        const elems = rest.filter(isElements);

        const colExpr = elems.map((_, i) => i === 0 ? "1fr" : "auto").join(" ");

        this.element = Div(
            className("group-panel"),
            gridTemplateColumns(colExpr),
            ...rest
        );

        Object.seal(this);
    }
}
