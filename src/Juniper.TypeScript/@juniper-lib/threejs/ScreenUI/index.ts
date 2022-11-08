import { classList, id } from "@juniper-lib/dom/attrs";
import { backgroundColor, rule } from "@juniper-lib/dom/css";
import { Div, ErsatzElement, Style } from "@juniper-lib/dom/tags";
import { singleton } from "@juniper-lib/tslib/singleton";

import "./style.css";

export class ScreenUI implements ErsatzElement {

    readonly element: HTMLElement;

    readonly topLeft: HTMLElement;
    readonly topCenter: HTMLElement;
    readonly topRight: HTMLElement;

    readonly middleLeft: HTMLElement;
    readonly middleCenter: HTMLElement;
    readonly middleRight: HTMLElement;

    readonly bottomLeft: HTMLElement;
    readonly bottomCenter: HTMLElement;
    readonly bottomRight: HTMLElement;

    readonly cells: Array<Array<HTMLElement>>;

    constructor(buttonFillColor: CSSColorValue) {

        singleton("Juniper.ThreeJS.ScreenUI.ButtonFillColor", () =>
            Style(
                rule("#controls > .row > .cell > .btn",
                    backgroundColor(buttonFillColor)
                )
            )
        );

        this.element = Div(
            id("controls"),
            this.topLeft = Div(classList("cell", "top", "left")),
            this.topCenter = Div(classList("cell", "top", "center")),
            this.topRight = Div(classList("cell", "top", "right")),
            this.middleLeft = Div(classList("cell", "middle", "left")),
            this.middleCenter = Div(classList("cell", "middle", "center")),
            this.middleRight = Div(classList("cell", "middle", "right")),
            this.bottomLeft = Div(classList("cell", "bottom", "left")),
            this.bottomCenter = Div(classList("cell", "bottom", "center")),
            this.bottomRight = Div(classList("cell", "bottom", "right"))
        );

        this.cells = [
            [this.topLeft, this.topCenter, this.topRight],
            [this.middleLeft, this.middleCenter, this.middleRight],
            [this.bottomLeft, this.bottomCenter, this.bottomRight]
        ];
    }
}
