import { ClassList } from "@juniper-lib/dom/attrs";
import { Div, ElementChild, elementSetDisplay, elementSetText, ErsatzElement, H2, Span } from "@juniper-lib/dom/tags";
import { debounce } from "@juniper-lib/events/debounce";

import "./style.css";

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
            ClassList("named-panel"),
            this.header = H2(
                this.titleText = Span(_title),
            ),
            this.body = Div(
                ClassList("body"),
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
