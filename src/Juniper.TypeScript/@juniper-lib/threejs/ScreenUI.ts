import { className, id } from "@juniper-lib/dom/attrs";
import { backgroundColor, rule } from "@juniper-lib/dom/css";
import { Div, ErsatzElement, Style } from "@juniper-lib/dom/tags";
import { singleton } from "@juniper-lib/tslib/singleton";

import "./ScreenUI.css";

export class ScreenUI implements ErsatzElement {

    readonly element: HTMLElement;

    readonly topRowLeft: HTMLElement;
    readonly topRowRight: HTMLElement;

    readonly middleRowLeft: HTMLElement;
    readonly middleRowRight: HTMLElement;

    readonly bottomRowLeft: HTMLElement;
    readonly bottomRowRight: HTMLElement;

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
