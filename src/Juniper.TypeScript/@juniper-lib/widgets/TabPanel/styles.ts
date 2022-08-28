import { backgroundColor, border, borderBottom, borderBottomColor, borderTop, boxShadow, color, display, flexDirection, gridArea, gridTemplateColumns, gridTemplateRows, marginBottom, marginTop, opacity, overflow, paddingLeft, pointerEvents, rule, zIndex } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".tab-panel",
        display("grid"),
        gridTemplateRows("auto", "1fr")
    ),

    rule(".tab-panel > .tabs",
        display("flex"),
        flexDirection("row"),
        borderBottom("solid 1px #6c757d"),
        pointerEvents("none"),
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