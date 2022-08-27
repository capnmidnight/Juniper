import {
    color,
    display,
    gridColumn,
    gridColumnGap,
    gridTemplateColumns,
    height,
    margin,
    maxWidth,
    padding,
    rule,
    textAlign,
    verticalAlign
} from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule("dl.properties",
        display("grid"),
        gridTemplateColumns("auto", "1fr"),
        gridColumnGap(".25em")
    ),

    rule("dl.properties.disabled",
        color("#ccc")
    ),

    rule("dl.properties > dt",
        gridColumn(1, 2),
        textAlign("right")
    ),

    rule("dl.properties > dt > label",
        margin(0),
        padding(0),
        height("100%"),
        verticalAlign("sub")
    ),

    rule("dl.properties > dd",
        gridColumn(2, -1),
        display("grid"),
        gridTemplateColumns("auto")
    ),

    rule("dl.properties > dd input[type=number]",
        textAlign("right")
    ),

    rule("dl.properties > dd > img",
        maxWidth("10em")
    ),

    rule("dl.properties > dd > select",
        height("30px")
    ),

    rule("dl.properties > .single-item",
        gridColumn(1, -1),
        textAlign("center")
    )
);