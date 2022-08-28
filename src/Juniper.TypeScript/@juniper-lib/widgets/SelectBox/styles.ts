import { backgroundColor, border, color, cursor, height, margin, opacity, overflow, overflowWrap, padding, rule, whiteSpace } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".SelectBox",
        border("inset 2px"),
        backgroundColor("white"),
        whiteSpace("nowrap"),
        overflowWrap("normal"),
        overflow("auto", "scroll")
    ),

    rule(".SelectBoxContent",
        height(0)
    ),

    rule(".SelectBoxRow",
        whiteSpace("pre"),
        color("black"),
        padding(0, "2px", "1px"),
        margin(0),
        cursor("default")
    ),

    rule(".SelectBoxRow:hover",
        backgroundColor("#eee")
    ),

    rule(".SelectBoxRow.disabled",
        opacity(0.5),
        cursor("not-allowed")
    ),

    rule(".SelectBoxRow.disabled:hover",
        backgroundColor("unset")
    ),

    rule(".SelectBoxRow.selected",
        border("solid 1px #488"),
        backgroundColor("#dff")
    ),

    rule(".SelectBoxRow.selected.disabled",
        border("solid 1px #444"),
        backgroundColor("#ddd")
    )
);