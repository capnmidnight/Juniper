import { once, success, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { classList, className, customData } from "./attrs";
import { backgroundColor, boxShadow, display, float, gridArea, gridTemplateColumns, gridTemplateRows, height, left, maxHeight, maxWidth, overflow, padding, position, rule, styles, textAlign, top, transform, width, zIndex } from "./css";
import { ButtonPrimary, ButtonSecondary, Div, elementApply, elementIsDisplayed, elementSetDisplay, elementSetText, ErsatzElement, H1, Style } from "./tags";

Style(
    rule(".dialog, .dialog-container",
        position("fixed")),
    rule(".dialog",
        top(0),
        left(0),
        width("100%"),
        height("100%"),
        backgroundColor("rgba(0, 0, 0, 0.5)"),
        zIndex(100)),
    rule(".dialog-container",
        top("50%"),
        left("50%"),
        maxWidth("100%"),
        maxHeight("100%"),
        transform("translateX(-50%) translateY(-50%)"),
        backgroundColor("white"),
        boxShadow("rgba(0,0,0,0.5) 0px 5px 30px"),
        display("grid"),
        gridTemplateColumns("2em auto 2em"),
        gridTemplateRows("auto 1fr auto 2em")),
    rule(".dialog .title-bar",
        gridArea(1, 1, 2, -1),
        padding("0.25em")),
    rule(".dialog-content",
        gridArea(2, 2, -4, -2),
        overflow("auto")),
    rule(".dialog-controls",
        gridArea(-2, 2, -3, -2)),
    rule(".dialog .confirm-button",
        float("right")),
    rule(".dialog h1, .dialog h2, .dialog h3, .dialog h4, .dialog h5, .dialog h6",
        textAlign("left")),
    rule(".dialog select",
        maxWidth("10em"))
);

export abstract class DialogBox
    implements ErsatzElement {

    public readonly element: HTMLDivElement;

    private subEventer = new TypedEventBase<{
        confirm: TypedEvent<"confirm">;
        cancel: TypedEvent<"cancel">;
    }>();

    private _title: string;
    private readonly titleElement: HTMLElement;

    protected readonly container: HTMLDivElement;
    protected readonly contentArea: HTMLDivElement;
    protected readonly confirmButton: HTMLButtonElement;
    protected readonly cancelButton: HTMLButtonElement;


    constructor(title: string) {

        this.element = Div(
            className("dialog"),
            customData("dialogname", title),
            styles(display("none")),
            this.container = Div(className("dialog-container"),
                this.titleElement = H1(className("title-bar"), title),
                this.contentArea = Div(className("dialog-content")),
                Div(className("dialog-controls"),
                    this.confirmButton = ButtonPrimary("Confirm", classList("confirm-button")),
                    this.cancelButton = ButtonSecondary("Cancel", classList("cancel-button")))));

        this.confirmButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("confirm")));

        this.cancelButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("cancel")));

        elementApply(document.body, this);
    }

    get title() {
        return this._title;
    }

    set title(v: string) {
        elementSetText(this.titleElement, this._title = v);
    }

    protected async onShowing(): Promise<void> {
        // nothing to do in the base case.
    }

    protected onShown(): void {
        // nothing to do in the base case.
    }

    protected async onConfirm(): Promise<void> {
        // nothing to do in the base case.
    }

    protected onCancel(): void {
        // nothing to do in the base case.
    }

    protected async onClosing(): Promise<void> {
        // nothing to do in the base case.
    }

    protected onClosed(): void {
        // nothing to do in the base case.
    }

    private show(v: boolean) {
        elementSetDisplay(this, v);
    }

    get isOpen() {
        return elementIsDisplayed(this);
    }

    async showDialog(): Promise<boolean> {
        await this.onShowing();
        this.show(true);
        this.onShown();

        const confirming = once(this.subEventer, "confirm", "cancel");
        const confirmed = await success(confirming);
        if (confirmed) {
            await this.onConfirm();
        }
        else {
            this.onCancel();
        }

        await this.onClosing();
        this.show(false);
        this.onClosed();

        return confirmed;
    }
}
