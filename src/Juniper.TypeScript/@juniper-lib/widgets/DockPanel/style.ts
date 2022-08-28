import { backgroundColor, cursor, display, gridArea, gridTemplateColumns, gridTemplateRows, margin, minHeight, minWidth, opacity, rule } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".dock",
        margin(0)
    ),
    rule(".dock.panel",
        display("grid"),
        gridTemplateRows("auto"),
        gridTemplateColumns("auto")
    ),
    rule(".dock.group",
        display("grid")
    ),
    rule(".dock.cell",
        display("grid"),
        gridTemplateRows("auto", "1fr"),
        gridTemplateColumns("1fr", "auto")
    ),
    rule(".dock.panel.rearrangeable .dock.cell [draggable]",
        cursor("move")
    ),
    rule(".dock.cell.dragging",
        opacity(.5)
    ),
    rule(".dock.cell > .header",
        margin("auto", "7px"),
        gridArea(1, 1)
    ),
    rule(".dock.cell > .closer",
        gridArea(1, -2)
    ),
    rule(".dock.cell > .content",
        gridArea(2, 1, 3, 3),
        display("grid"),
        gridTemplateRows("auto"),
        gridTemplateColumns("auto")
    ),
    rule(".dock.sep",
        minWidth("4px"),
        minHeight("4px")
    ),
    rule(".dock.sep.targeting, .dock.sep.dragging",
        backgroundColor("#bbb")
    ),
    rule(".dock.sep.column:not(.edge)",
        cursor("ns-resize")
    ),
    rule(".dock.sep.row:not(.edge)",
        cursor("ew-resize")
    )
);