import { ClassList } from "@juniper-lib/dom/dist/attrs";
import { fr, gridTemplateColumns } from "@juniper-lib/dom/dist/css";
import { Div, ElementChild, ErsatzElement, isElements } from "@juniper-lib/dom/dist/tags";

import "./style.css";

export class GroupPanel
implements ErsatzElement {

    readonly element: HTMLElement;

    constructor(...rest: ElementChild[]) {

        const elems = rest.filter(isElements);

        const colExpr = elems.map((_, i) => i === 0 ? fr(1) : "auto");

        this.element = Div(
            ClassList("group-panel"),
            gridTemplateColumns(...colExpr),
            ...rest
        );

        Object.seal(this);
    }
}
