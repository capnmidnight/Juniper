import { className, id } from "@juniper-lib/dom/attrs";
import {
    backgroundColor,
    borderRadius,
    display,
    filter,
    flexFlow,
    fontSize, gridColumn, gridRow,
    gridTemplateColumns,
    gridTemplateRows,
    height,
    left,
    margin,
    padding,
    pointerEvents,
    position,
    rule,
    top,
    width,
    zIndex
} from "@juniper-lib/dom/css";
import { Div, ErsatzElement, Style } from "@juniper-lib/dom/tags";

Style(
    rule("#controls",
        position("absolute"),
        left(0),
        top(0),
        width("100%"),
        height("100%")
    ),

    rule("#controls",
        display("grid"),
        fontSize("20pt"),
        gridTemplateRows("auto", "1fr", "auto"),
        zIndex(1)
    ),

    rule("#controls, #controls *",
        pointerEvents("none")
    ),

    rule("#controls canvas",
        height("58px")
    ),

    rule("#controls > .row",
        display("grid"),
        margin("10px", "5px"),
        gridTemplateColumns("repeat(2, auto)")
    ),

    rule("#controls > .row.top",
        gridRow(1)
    ),

    rule("#controls > .row.middle",
        gridRow(2, -2)
    ),

    rule("#controls > .row.bottom",
        gridRow(-2)
    ),

    rule("#controls > .row > .cell",
        display("flex")
    ),

    rule("#controls > .row > .cell.left",
        gridColumn(1)
    ),

    rule("#controls > .row > .cell.right",
        gridColumn(-2),
        flexFlow("row-reverse")
    ),

    rule("#controls > .row > .cell > .btn",
        borderRadius(0),
        backgroundColor("#1e4388"),
        height("58px").important(),
        width("58px"),
        padding("0.25em"),
        margin(0, "5px"),
        pointerEvents("initial")
    ),

    rule("#controls .btn img",
        height("calc(100% - 0.5em)")
    ),

    rule("#controls button:disabled img",
        filter("invert(.5)")
    )
);

export class ScreenUI implements ErsatzElement {

    readonly element: HTMLElement;

    readonly topRowLeft: HTMLElement;
    readonly topRowRight: HTMLElement;

    readonly middleRowLeft: HTMLElement;
    readonly middleRowRight: HTMLElement;

    readonly bottomRowLeft: HTMLElement;
    readonly bottomRowRight: HTMLElement;

    readonly cells: Array<Array<HTMLElement>>;

    constructor() {
        this.element = Div(
            id("controls"),
            Div(className("row top"),
                this.topRowLeft = Div(className("cell left")),
                this.topRowRight = Div(className("cell right"))),
            Div(className("row middle"),
                this.middleRowLeft = Div(className("cell left")),
                this.middleRowRight = Div(className("cell right"))),
            Div(className("row bottom"),
                this.bottomRowLeft = Div(className("cell left")),
                this.bottomRowRight = Div(className("cell right")))
        );

        this.cells = [
            [this.topRowLeft, this.topRowRight],
            [this.middleRowLeft, this.middleRowRight],
            [this.bottomRowLeft, this.bottomRowRight]
        ];
    }
}
