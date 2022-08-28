import { backgroundColor, border, borderBottom, borderBottomColor, borderTop, boxShadow, color, display, flexDirection, flexGrow, flexShrink, gridArea, gridTemplateColumns, gridTemplateRows, marginBottom, marginTop, opacity, overflow, paddingLeft, pointerEvents, rule, width, zIndex } from "@juniper-lib/dom/css";
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
        borderBottom("solid 1px #6c757d"),
        overflow("hidden"),
        paddingLeft("2px")
    ),

    rule(".tab-panel > .tabs > button",
        border("solid 1px #888"),
        marginBottom("-3px"),
        pointerEvents("initial"),
        zIndex(0),
        color("#888")
    ),

    rule(".tab-panel > .tabs > button.btn[disabled]",
        borderBottomColor("transparent"),
        boxShadow("#ccc 0 -5px 10px"),
        marginTop("-3px"),
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
        border("2px outset #ccc"),
        borderTop("none")
    ),

    rule(".tab-panel > .panels > *",
        gridArea(1, 1, -1, -1)
    )
);