import {
    height,
    overflow,
    padding, rule
} from "@juniper-lib/dom/css";
import { Style } from "@juniper-lib/dom/tags";

Style(
    rule(".scroll-panel",
        overflow("auto", "auto"),
        padding("0.5em")
    ),

    rule(".scroll-panel-inner",
        height(0)
    )
);