import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { IDialog, StandardDialogEventsMap } from "./IDialog";
import { StandardDialog, StandardDialogElement } from './StandardDialogElement';

export abstract class BaseDialogElement<InputT = void, OutputT = InputT>
    extends TypedHTMLElement<StandardDialogEventsMap<InputT, OutputT>>
    implements IDialog<InputT, OutputT> {

    static observedAttributes = [
        "cancelable",
        "title"
    ];

    readonly #title: HTMLElement;
    protected dialog: StandardDialogElement<InputT, OutputT>;

    constructor(title: HTMLElement, body: HTMLElement, ...rest: ElementChild<StandardDialogElement<InputT, OutputT>>[]) {
        super();

        title.slot = "modal-title";
        body.slot = "modal-body";

        this.dialog = StandardDialog(
            title,
            body,
            ...rest
        );

        this.#title = title;
    }

    protected Q<T extends Element>(selectors: string): T {
        return this.dialog.querySelector(selectors);
    }

    connectedCallback() {
        if (this.dialog.parentElement !== this) {
            this.append(this.dialog);
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name) {
            case "cancelable":
                this.dialog.cancelable = this.cancelable;
                break;
            case "title":
                this.#title.replaceChildren(this.title);
                break;
        }
    }

    get body() { return this.dialog.body; }
    get form() { return this.dialog.form; }

    get cancelButtonText() { return this.dialog.cancelButtonText; }
    set cancelButtonText(v) { this.dialog.cancelButtonText = v; }

    get saveButtonText() { return this.dialog.saveButtonText; }
    set saveButtonText(v) { this.dialog.saveButtonText = v; }

    get errorMessage() { return this.dialog.errorMessage; }
    set errorMessage(v) { this.dialog.errorMessage = v; }

    get open() { return this.hasAttribute("open"); }
    set open(v) { this.toggleAttribute("open", v); }

    get cancelable() { return this.hasAttribute("cancelable"); }
    set cancelable(v) { this.toggleAttribute("cancelable", v); }

    override get title() { return this.getAttribute("title"); }
    override set title(v) { this.setAttribute("titie", v); }

    show(value: InputT): Promise<OutputT> {
        return this.dialog.show(value);
    }

    showModal(value: InputT): Promise<OutputT> {
        return this.dialog.showModal(value);
    }

    close(): void {
        this.dialog.close();
    }

    cancel(): void {
        this.dialog.cancel();
    }

    confirm(): void {
        this.dialog.confirm();
    }

    confirmed(): Promise<boolean> {
        return this.dialog.confirmed();
    }

    cancelled(): Promise<boolean> {
        return this.dialog.cancelled();
    }
}
