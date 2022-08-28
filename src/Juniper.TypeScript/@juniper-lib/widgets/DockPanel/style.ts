import { backgroundColor, cursor, display, gridArea, gridTemplateColumns, gridTemplateRows, margin, opacity, padding, rule } from "@juniper-lib/dom/css";
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
    rule(".dock.row, .dock.column",
        display("grid"),
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
        margin("2px"),
        padding("1px"),
        backgroundColor("#ddd")
    ),
    rule(".dock.sep.targeting",
        backgroundColor("#bbb")
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