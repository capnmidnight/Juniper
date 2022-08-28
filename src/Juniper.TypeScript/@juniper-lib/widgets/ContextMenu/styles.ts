import { backgroundColor, border, borderRadius, boxShadow, display, getSystemFamily, gridTemplateColumns, margin, padding, position, rule, textAlign } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".context-menu",
        position("absolute"),
        backgroundColor("white"),
        padding("5px"),
        display("grid"),
        gridTemplateColumns("auto"),
        borderRadius("5px"),
        boxShadow("rgb(0 0 0 / 15%) 2px 2px 17px")
    ),

    rule(".context-menu > button",
        border("none"),
        textAlign("left"),
        backgroundColor("transparent"),
        margin("2px"),
        padding(0, "2em", 0, "0.5em"),
        getSystemFamily()
    ),

    rule(".context-menu > button:hover",
        backgroundColor("#ddd")
    )
);