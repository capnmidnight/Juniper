import { borderBottom, borderBottomColor, borderRadius, boxShadow, display, flexDirection, marginBottom, paddingTop, rule, zIndex } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".tabs",
        display("flex"),
        flexDirection("row"),
        borderBottom("solid 1px #6c757d"),
        paddingTop("5px")
    ),

    rule(".tabs > button",
        borderRadius("5px 5px 0 0"),
        marginBottom("-1px")
    ),

    rule(".tabs > button.btn-secondary",
        zIndex(0)
    ),

    rule(".tabs > button.btn-outline-secondary",
        borderBottomColor("white"),
        boxShadow("#ccc 0 -5px 10px"),
        zIndex(1)
    )
);