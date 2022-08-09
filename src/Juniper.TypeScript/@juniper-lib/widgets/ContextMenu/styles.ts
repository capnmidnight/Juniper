import { backgroundColor, border, display, getSystemFamily, gridTemplateColumns, margin, padding, position, rule, textAlign } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".context-menu",
        position("absolute"),
        backgroundColor("white"),
        padding("5px"),
        display("grid"),
        gridTemplateColumns("auto")
    ),

    rule(".context-menu > button",
        border("none"),
        textAlign("left"),
        backgroundColor("transparent"),
        margin("2px"),
        getSystemFamily()
    ),

    rule(".context-menu > button:hover",
        backgroundColor("#ddd")
    )
);