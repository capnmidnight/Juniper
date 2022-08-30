import { className } from "@juniper-lib/dom/attrs";
import { backgroundColor, border, borderRadius, display, float, gridTemplateRows, margin, padding, px, rule } from "@juniper-lib/dom/css";
import { Div, ElementChild, elementSetDisplay, elementSetText, ErsatzElement, H2, Span, Style } from "@juniper-lib/dom/tags";
import { debounce } from "@juniper-lib/tslib/events/debounce";

Style(
    rule(".named-panel",
        display("grid"),
        gridTemplateRows("auto", "1fr"),
        border(`${px(2)} outset #ccc`),
        borderRadius(px(5))
    ),

    rule(".named-panel > H2",
        margin(0),
        padding(px(3), px(6)),
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
    private readonly body: HTMLDivElement;

    private _open = true;
    refresh: () => void;

    constructor(private _title: string, ...rest: ElementChild[]) {

        this.element = Div(
            className("named-panel"),
            this.header = H2(
                this.titleText = Span(_title),
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
