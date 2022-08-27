import { className } from "@juniper-lib/dom/attrs";
import { backgroundColor, border, borderRadius, display, float, gridTemplateRows, margin, padding, rule } from "@juniper-lib/dom/css";
import { Div, ElementChild, elementSetDisplay, elementSetText, ErsatzElement, H2, Span, Style } from "@juniper-lib/dom/tags";
//import { blackMediumDownPointingTriangleCentered, blackMediumRightPointingTriangleCentered } from "@juniper-lib/emoji";
import { debounce } from "@juniper-lib/tslib/events/debounce";

Style(
    rule(".named-panel",
        display("grid"),
        gridTemplateRows("auto", "1fr"),
        border("2px outset #ccc"),
        borderRadius("5px")
    ),

    rule(".named-panel > H2",
        margin(0),
        padding("3px", "6px"),
        backgroundColor("#ccc")
    ),

    rule(".named-panel > H2 > button",
        float("right")
    ),

    rule(".named-panel > .body",
        display("grid")
    )
);

export class NamedPanel
    implements ErsatzElement {

    readonly element: HTMLElement;

    private readonly header: HTMLHeadingElement;
    private readonly titleText: HTMLSpanElement;
    //private readonly closer: HTMLButtonElement;
    private readonly body: HTMLDivElement;

    private _open = true;
    refresh: () => void;

    constructor(private _title: string, ...rest: ElementChild[]) {

        this.element = Div(
            className("named-panel"),
            this.header = H2(
                this.titleText = Span(_title),
                //this.closer = ButtonSmall(
                //    blackMediumDownPointingTriangleCentered.textStyle,
                //    onClick(() => this.open = !this.open)
                //)
            ),
            this.body = Div(
                className("body"),
                ...rest
            )
        );

        this.refresh = debounce(() => this.onRefresh());

        Object.seal(this);
    }

    get title() {
        return this._title;
    }

    set title(v) {
        if (v !== this.title) {
            this._title = v;
            this.refresh();
        }
    }

    get open() {
        return this._open;
    }

    set open(v) {
        if (v !== this.open) {
            this._open = v;
            this.refresh();
        }
    }

    private onRefresh() {
        elementSetText(this.titleText, this._title);
        //elementSetText(this.closer, this.open
        //    ? blackMediumDownPointingTriangleCentered.textStyle
        //    : blackMediumRightPointingTriangleCentered.textStyle
        //);
        this.header.classList.toggle("closed", !this.open);
        elementSetDisplay(this.body, this.open);
    }
}
