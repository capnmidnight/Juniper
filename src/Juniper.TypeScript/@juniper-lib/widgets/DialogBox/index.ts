import { classList, className, customData } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { ButtonPrimary, ButtonSecondary, Div, elementApply, elementIsDisplayed, elementSetDisplay, elementSetText, ErsatzElement, H1 } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";

import "./styles.css";


export abstract class DialogBox implements ErsatzElement {

    public readonly element: HTMLDivElement;

    private readonly task = new Task<boolean>(false);

    private _title: string;
    private readonly titleElement: HTMLElement;

    protected readonly container: HTMLDivElement;
    protected readonly contentArea: HTMLDivElement;
    protected readonly confirmButton: HTMLButtonElement;
    protected readonly cancelButton: HTMLButtonElement;


    constructor(title: string) {

        this.element = Div(
            className("dialog"),
            display("none"),
            customData("dialogname", title),
            this.container = Div(className("dialog-container"),
                this.titleElement = H1(className("title-bar"), title),
                this.contentArea = Div(className("dialog-content")),
                Div(className("dialog-controls"),
                    this.confirmButton = ButtonPrimary("Confirm", classList("confirm-button")),
                    this.cancelButton = ButtonSecondary("Cancel", classList("cancel-button")))));


        this.confirmButton.addEventListener("click", this.task.resolver(true));
        this.cancelButton.addEventListener("click", this.task.resolver(false));

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

    hide() {
        this.show(false);
    }

    async toggle() {
        if (this.isOpen) {
            this.hide();
        }
        else {
            this.showDialog();
        }
    }

    async showDialog(): Promise<boolean> {
        await this.onShowing();
        this.show(true);
        this.onShown();
        this.task.restart();
        if (await this.task) {
            await this.onConfirm();
        }
        else {
            this.onCancel();
        }

        await this.onClosing();
        this.show(false);
        this.onClosed();

        return this.task.result;
    }
}
