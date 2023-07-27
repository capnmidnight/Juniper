import { ClassList } from "@juniper-lib/dom/attrs";
import { backgroundColor, rule } from "@juniper-lib/dom/css";
import { Div, Style } from "@juniper-lib/dom/tags";
import { singleton } from "@juniper-lib/tslib/singleton";

import "./style.css";

export class ScreenUI {

    readonly elements: Array<HTMLElement>;

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

    constructor(buttonFillColor: CssColorValue) {
        singleton("Juniper.ThreeJS.ScreenUI.ButtonFillColor", () =>
            Style(
                rule("#appContainer > .row > .cell > .btn",
                    backgroundColor(buttonFillColor)
                )
            )
        );

        this.elements = [
            this.topLeft = Div(ClassList("cell", "top", "left")),
            this.topCenter = Div(ClassList("cell", "top", "center")),
            this.topRight = Div(ClassList("cell", "top", "right")),
            this.middleLeft = Div(ClassList("cell", "middle", "left")),
            this.middleCenter = Div(ClassList("cell", "middle", "center")),
            this.middleRight = Div(ClassList("cell", "middle", "right")),
            this.bottomLeft = Div(ClassList("cell", "bottom", "left")),
            this.bottomCenter = Div(ClassList("cell", "bottom", "center")),
            this.bottomRight = Div(ClassList("cell", "bottom", "right"))
        ];

        this.cells = [
            [this.topLeft, this.topCenter, this.topRight],
            [this.middleLeft, this.middleCenter, this.middleRight],
            [this.bottomLeft, this.bottomCenter, this.bottomRight]
        ];

        this.hide();
    }

    show() {
        this.elements.forEach(v => v.style.removeProperty("display"));
    }

    hide() {
        this.elements.forEach(v => v.style.display = "none");
    }
}
