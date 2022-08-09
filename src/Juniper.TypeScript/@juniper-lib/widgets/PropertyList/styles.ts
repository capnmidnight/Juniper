import { color, display, gridAutoFlow, gridColumn, gridColumnGap, gridTemplateColumns, rule, textAlign, width } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule("dl.properties",
        display("grid"),
        gridAutoFlow("row"),
        gridTemplateColumns("auto 1fr"),
        gridColumnGap("1em")
    ),

    rule("dl.properties.disabled",
        color("#ccc")
    ),

    rule("dl.properties > span, dl.properties > div",
        gridColumn(1, 3),
        width("100%")
    ),

    rule("dl.properties > dt",
        gridColumn(1),
        textAlign("right")
    ),

    rule("dl.properties > dd",
        gridColumn(2),
        display("grid"),
        gridAutoFlow("column")
    ),

    rule("dl.properties input[type=number]",
        textAlign("right")
    )
);