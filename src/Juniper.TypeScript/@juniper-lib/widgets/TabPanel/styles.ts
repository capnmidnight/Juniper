import { border, borderBottom, borderBottomColor, borderRadius, borderTop, boxShadow, display, flexDirection, gridArea, gridTemplateColumns, gridTemplateRows, marginBottom, rule, zIndex } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".tab-panel",
        display("grid"),
        gridTemplateRows("auto 1fr")
    ),

    rule(".tab-panel > .tabs",
        display("flex"),
        flexDirection("row"),
        borderBottom("solid 1px #6c757d")
    ),

    rule(".tab-panel > .tabs > button",
        borderRadius("5px 5px 0 0"),
        marginBottom("-3px")
    ),

    rule(".tab-panel > .tabs > button.btn-secondary",
        zIndex(0)
    ),

    rule(".tab-panel > .tabs > button.btn-outline-secondary",
        borderBottomColor("white"),
        boxShadow("#ccc 0 -5px 10px"),
        zIndex(1)
    ),

    rule(".tab-panel > .panels",
        display("grid"),
        gridTemplateColumns("auto"),
        gridTemplateRows("auto"),
        border("2px outset #ccc"),
        borderTop("none")
    ),

    rule(".tab-panel > .panels > *",
        gridArea(1, 1, -1, -1)
    )
);