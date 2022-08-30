import { backgroundColor, border, borderRadius, boxShadow, display, getSystemFamily, gridTemplateColumns, margin, padding, position, px, rule, textAlign } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".context-menu",
        position("absolute"),
        backgroundColor("white"),
        padding(px(5)),
        display("grid"),
        gridTemplateColumns("auto"),
        borderRadius(px(5)),
        boxShadow(`rgb(0 0 0 / 15%) ${px(2)} ${px(2)} ${px(17)}`)
    ),

    rule(".context-menu > button",
        border("none"),
        textAlign("left"),
        backgroundColor("transparent"),
        margin(px(2)),
        padding(0, "2em", 0, "0.5em"),
        getSystemFamily()
    ),

    rule(".context-menu > button:hover",
        backgroundColor("#ddd")
    )
);