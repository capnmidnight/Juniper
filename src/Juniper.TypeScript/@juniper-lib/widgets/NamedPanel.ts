import { className } from "@juniper-lib/dom/attrs";
import { backgroundColor, border, borderRadius, display, gridTemplateRows, margin, padding, rule } from "@juniper-lib/dom/css";
import { Div, ElementChild, ErsatzElement, H2, Style } from "@juniper-lib/dom/tags";

Style(
    rule(".named-panel",
        display("grid"),
        gridTemplateRows("auto 1fr"),
        border("2px outset #ccc"),
        borderRadius("5px")
    ),

    rule(".named-panel > H2",
        margin(0),
        padding("3px", "6px"),
        backgroundColor("#ccc")
    )
);

export class NamedPanel
    implements ErsatzElement {

    readonly element: HTMLElement;

    constructor(name: string, ...rest: ElementChild[]) {

        this.element = Div(
            className("named-panel"),
            H2(name),
            ...rest
        );

        Object.seal(this);
    }
}
