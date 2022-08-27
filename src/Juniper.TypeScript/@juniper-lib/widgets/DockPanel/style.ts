import { backgroundColor, border, cursor, display, float, gridGap, gridTemplate, margin, opacity, padding, rule } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".dock.panel",
        display("grid"),
        gridTemplate("auto/auto")
    ),
    rule(".dock.row, .dock.column",
        display("grid"),
        gridGap("2px")
    ),
    rule(".dock.cell",
        padding("5px"),
        border("1px solid black")
    ),
    rule(".dock.cell[draggable]",
        cursor("move")
    ),
    rule(".dock.cell[draggable].dragging",
        opacity(.5)
    ),
    rule(".dock.cell > .closer",
        float("right")
    ),
    rule(".dock.cell > .header",
        margin(0)
    ),
    rule(".dock.sep",
        padding("2px")
    ),
    rule(".dock.sep.targeting",
        backgroundColor("lightgrey")
    ),
    rule(".dock.sep.edge",
        margin("-2px")
    ),
    rule(".dock.sep.c:not(.edge)",
        cursor("ns-resize")
    ),
    rule(".dock.sep.r:not(.edge)",
        cursor("ew-resize")
    )
);