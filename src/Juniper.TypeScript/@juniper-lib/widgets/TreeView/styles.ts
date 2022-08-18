import {
    backgroundColor,
    border,
    bottom,
    color, cursor,
    display,
    fontFamily,
    fontSize,
    getSystemFamily,
    gridAutoFlow, gridTemplateRows,
    height, opacity,
    overflow,
    overflowWrap, padding,
    paddingLeft,
    position,
    rule, top,
    whiteSpace,
    width
} from "@juniper-lib/dom/css";
import {
    Style
} from "@juniper-lib/dom/tags";

Style(

    rule(".tree-view",
        display("grid"),
        gridTemplateRows("auto 1fr")
    ),

    rule(".tree-view .btn-sm",
        padding("0")
    ),

    rule(".tree-view-inner",
        border("inset 2px"),
        backgroundColor("#eee"),
        whiteSpace("nowrap"),
        overflowWrap("normal"),
        overflow("auto", "scroll"),
        getSystemFamily()
    ),

    rule(".tree-view-controls",
        display("grid"),
        gridAutoFlow("column")
    ),

    rule(".tree-view-children",
        height("0")
    ),

    rule(".tree-view-node > .tree-view-node-children",
        paddingLeft("1.25em")
    ),

    rule(".tree-view-children > .tree-view-node > .tree-view-node-children",
        padding(0)
    ),

    rule(".tree-view-node",
        whiteSpace("pre"),
        color("black"),
        cursor("default")
    ),

    rule(".tree-view-node:hover > .tree-view-node-label",
        backgroundColor("rgba(128,128,128,0.125)")
    ),

    rule(".tree-view-node-label[draggable=true]",
        cursor("grab")
    ),

    rule(".tree-view-node .tree-view-node.disabled, .tree-view-node .tree-view-node.disabled > .tree-view-node-label[draggable=true]",
        cursor("not-allowed")
    ),

    rule(".tree-view-node .tree-view-node.disabled > .tree-view-node-label",
        opacity(0.5)
    ),

    rule(".tree-view-node.disabled:hover > .tree-view-node-label",
        backgroundColor("unset")
    ),

    rule(".tree-view-node.selected",
        backgroundColor("#dff")
    ),

    rule(".tree-view-node.selected.disabled",
        backgroundColor("#ddd")
    ),

    rule(".tree-view-node .drag-buffer",
        position("absolute"),
        width("100%"),
        height("0.25em")
    ),

    rule(".tree-view-node .drag-buffer.top",
        top("-0.125em")),

    rule(".tree-view-node .drag-buffer.bottom",
        bottom("-0.125em")),

    rule(".tree-view-node.highlighted",
        backgroundColor("#dfd")
    ),

    rule(".tree-view-node .drag-buffer.highlighted",
        backgroundColor("#000")
    ),

    rule(".tree-view-node-collapser, .tree-view-node-adder",
        fontFamily("monospace"),
        fontSize("80%"),
        color("#777")
    )
);