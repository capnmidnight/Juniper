import { height, overflow, paddingRight, rule } from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".scroll-panel",
        overflow("auto", "scroll"),
        paddingRight("0.5rem")
    ),

    rule(".scroll-panel-inner",
        height(0)
    )
);