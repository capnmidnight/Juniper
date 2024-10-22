import { backgroundColor, borderRadius, bottom, ClassList, color, CssColorValue, display, Div, em, filter, fontWeight, height, left, margin, padding, perc, position, px, right, rule, SingletonStyleBlob, top, transform, translateX, translateY, width } from "@juniper-lib/dom";

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
        SingletonStyleBlob("Juniper.ThreeJS.ScreenUI.ButtonFillColor", () =>
            rule("#appContainer",
                rule(">.cell",
                    position("absolute"),
                    display("flex"),
                    margin(px(10), px(5)),

                    rule(".top",
                        top(0)
                    ),

                    rule(".middle",
                        top(perc(50)),
                        transform(translateY(perc(-50)))
                    ),

                    rule(".bottom",
                        bottom(0)
                    ),

                    rule(".right",
                        right(0)
                    ),

                    rule(".center",
                        left(perc(50)),
                        transform(translateX(perc(-50)))
                    ),

                    
                    rule(".left", 
                        left(0)
                    ),

                    rule(" .btn",
                        borderRadius(0),
                        height(px(58)).important(true),
                        width(px(58)),
                        padding(em(.25)),
                        margin(0, px(5)),
                        color("white"),
                        fontWeight("bold"),

                        rule(" img",
                            height("calc(100% - 0.5em)")
                        )
                    ),

                    rule(" canvas",
                        height(px(58))
                    )
                ),

                rule(" button:disabled img",
                    filter("invert(.5)")
                ),

                rule(">.row>.cell>.btn",
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
