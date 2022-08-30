import { backgroundColor, border, color, cursor, height, margin, opacity, overflow, overflowWrap, padding, px, rule, whiteSpace } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".SelectBox",
        border(`inset ${px(2)}`),
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
        padding(0, px(2), px(1)),
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
        border(`solid ${px(1)} #488`),
        backgroundColor("#dff")
    ),

    rule(".SelectBoxRow.selected.disabled",
        border(`solid ${px(1)} #444`),
        backgroundColor("#ddd")
    )
);