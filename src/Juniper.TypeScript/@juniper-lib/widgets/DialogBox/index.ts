import { classList, className, customData } from "@juniper-lib/dom/attrs";
import { display, styles } from "@juniper-lib/dom/css";
import { ButtonPrimary, ButtonSecondary, Div, elementApply, elementIsDisplayed, elementSetDisplay, elementSetText, ErsatzElement, H1 } from "@juniper-lib/dom/tags";
import { once, success, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";

import "./styles";

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
