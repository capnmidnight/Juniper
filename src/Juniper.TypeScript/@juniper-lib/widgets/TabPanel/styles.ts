import { backgroundColor, border, borderBottom, borderBottomColor, borderTop, boxShadow, color, display, flexDirection, flexGrow, flexShrink, gridArea, gridTemplateColumns, gridTemplateRows, marginBottom, marginTop, opacity, overflow, paddingLeft, pointerEvents, px, rule, width, zIndex } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".tab-panel",
        display("flex"),
        flexDirection("column")
    ),

    rule(".tab-panel > .tabs",
        display("flex"),
        flexShrink(0),
        width("fit-content"),
        flexDirection("row"),
        borderBottom(`solid ${px(1)} #6c757d`),
        overflow("hidden"),
        paddingLeft(px(2))
    ),

    rule(".tab-panel > .tabs > button",
        border(`solid ${px(1)} #888`),
        marginBottom(px(-3)),
        pointerEvents("initial"),
        zIndex(0),
        color("#888")
    ),

    rule(".tab-panel > .tabs > button.btn[disabled]",
        borderBottomColor("transparent"),
        boxShadow(`#ccc 0 ${px(-5)} ${px(10)}`),
        marginTop(px(-3)),
        backgroundColor("white"),
        color("black"),
        opacity(1),
        zIndex(1)
    ),

    rule(".tab-panel > .panels",
        flexGrow(1),
        display("grid"),
        gridTemplateColumns("auto"),
        gridTemplateRows("auto"),
        border(`${px(2)} outset #ccc`),
        borderTop("none")
    ),

    rule(".tab-panel > .panels > *",
        gridArea(1, 1, -1, -1)
    )
);