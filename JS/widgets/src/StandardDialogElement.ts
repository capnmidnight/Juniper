import { Confirmation, arrayScan, once, singleton, stringRandom, success } from "@juniper-lib/util";
import {
    AriaLabelledBy,
    Button, ClassList,
    CustomData, Dialog, Div, FAIcon, FormTag, H1,
    HtmlRender,
    HtmlTag,
    ID, ImportDOM, InnerHTML, Name, OnClick, OnEscapeKeyPressed,
    Query, SlotTag,
    StyleBlob, TabIndex, Template,
    TitleAttr,
    alignItems,
    backgroundColor,
    border,
    borderRadius,
    boxShadow,
    display,
    height,
    hide,
    left, margin, marginLeft, marginTop, maxHeight, maxWidth, opacity, padding, perc, pointerEvents, position, rem, rgba, right, rule,
    show,
    textAlign,
    top, transform, transition, translate, vh,
    vw, width
} from "@juniper-lib/dom";
import { ButtonPrimary, ButtonSecondary, validateForm } from "./widgets";
import { ElementChild } from "@juniper-lib/dom";
import { IKEA } from "./IKEA";

const ERROR_ANIMATION_LENGTH_SECONDS = 0.5;

export function StandardDialog(...rest: ElementChild[]): StandardDialogElement {
    return HtmlTag("standard-dialog", ...rest);
}

export class StandardDialogElement extends HTMLElement {
    static async import(path: string): Promise<StandardDialogElement> {
        const elements = await ImportDOM(path);
        document.body.append(...elements);
        const dialog = arrayScan(elements, e => e.tagName === "STANDARD-DIALOG");
        return dialog as StandardDialogElement;
    }

    #dialog: HTMLDialogElement;
    #title: HTMLHeadingElement;
    #body: HTMLElement;
    get body(): HTMLElement { return this.#body; }
    #form: HTMLFormElement;
    get form(): HTMLFormElement { return this.#form; }
    #saveButton: HTMLButtonElement;
    get saveButtonText(): string { return this.#saveButton.innerHTML; }
    set saveButtonText(v) { this.#saveButton.innerHTML = v; }

    #errorBlock: HTMLElement;
    #errorMessage: HTMLElement;
    #confirmation: Confirmation;

    constructor() {
        super();
        this.#confirmation = new Confirmation();
    }

    connectedCallback(): void {
        const template = singleton("Juniper:Widgets:StandardDialogElement", () => {
            document.head.append(StyleBlob(
                rule("standard-dialog > dialog.modal",
                    position("fixed"),
                    top(0),
                    left(0),
                    width(vw(100)),
                    maxWidth(vw(100)),
                    height(vh(100)),
                    maxHeight(vh(100)),
                    pointerEvents("initial"),
                    margin(0)
                ),
                rule("standard-dialog > dialog.modal[open]",
                    display("block"),
                    border("none"),
                    borderRadius(0),
                    backgroundColor(rgba(0, 0, 0, 0.5))
                ),
                rule("standard-dialog > dialog.modal-dialog-scrollable > .modal-content",
                    position("fixed"),
                    maxWidth("calc(100vw - 10em)"),
                    maxHeight("calc(100vh - 4em)"),
                    top(perc(50)),
                    left(perc(50)),
                    transform(translate(perc(-50), perc(-50))),
                    boxShadow("rgba(0, 0, 0, 27%) 0px 5px 10px")
                ),
                rule("standard-dialog .modal-header",
                    display("block"),
                    textAlign("left")
                ),
                rule("standard-dialog .modal-header .btn-dismiss-dialog",
                    position("absolute"),
                    top(0),
                    right(0),
                    margin(rem(1))
                ),
                rule("standard-dialog .modal-error",
                    display("flex"),
                    alignItems("center"),
                    backgroundColor("var(--jhto-orange)"),
                    padding("0.5rem"),
                    marginTop("0.5rem"),
                    transition(`opacity ${ERROR_ANIMATION_LENGTH_SECONDS}s ease-out`)
                ),
                rule("standard-dialog .modal-error.hidden",
                    opacity(0)
                ),
                rule("standard-dialog .modal-error-message",
                    marginLeft(rem(0.5)),
                    width(perc(100)),
                    textAlign("center")
                )
            ));

            return Template(
                Dialog(
                    ClassList("modal", "modal-dialog", "modal-dialog-scrollable", "modal-xl"),
                    TabIndex(-1),
                    FormTag(
                        ClassList("modal-content"),
                        Div(
                            ClassList("modal-header"),
                            SlotTag(Name("modal-title")),
                            Div(
                                ClassList("modal-error", "hidden"),
                                FAIcon("warning"),
                                Div(
                                    ClassList("modal-error-message"),
                                    "[ERROR MESSAGE]"
                                ),
                                Button(
                                    ClassList("btn-close", "btn-dismiss-error"),
                                    CustomData("bs-dismiss", "modal"),
                                    TitleAttr("Dismiss error")
                                )
                            ),
                            Button(
                                ClassList("btn-close", "btn-dismiss-dialog"),
                                CustomData("bs-dismiss", "modal"),
                                TitleAttr("Close")
                            )
                        ),

                        SlotTag(Name("modal-body")),

                        Div(
                            ClassList("modal-footer"),
                            SlotTag(Name("modal-controls")),
                            ButtonSecondary(
                                "Close dialog without saving",
                                "Cancel",
                                CustomData("bs-dismiss", "modal"),
                                ClassList("btn-cancel")
                            ),
                            ButtonPrimary(
                                "Save changes",
                                "Save changes",
                                ClassList("btn-save")
                            )
                        )
                    )
                )
            );
        });

        this.append(template.content.cloneNode(true));

        IKEA(this);

        const titleID = stringRandom(12);
        this.#title = H1(
            Query(this, "[slot=\"modal-title\"]"),
            ID(titleID)
        );

        this.#body = Div(Query(this, "[slot=\"modal-body\"]"));
        this.#errorBlock = Div(Query(this, ".modal-error"));
        hide(this.#errorBlock);
        this.#errorMessage = Div(Query(this, ".modal-error-message"));
        Button(Query(this, ".btn-dismiss-error"), OnClick(() => this.errorMessage = null));

        this.#dialog = Dialog(
            Query(this, "dialog"),
            AriaLabelledBy(titleID),
            OnEscapeKeyPressed(() =>
                this.#confirmation.cancel())
        );

        this.#form = FormTag(Query(this, "form"));

        Button(Query(this, ".btn-dismiss-dialog"), OnClick(this.#confirmation.cancel));
        Button(Query(this, ".btn-cancel"), OnClick(this.#confirmation.cancel));
        this.#saveButton = Button(Query(this, ".btn-save"), OnClick(this.#confirmation.confirm));
    }

    override get title(): string {
        return this.#title.innerHTML;
    }

    override set title(v) {
        this.#title.innerHTML = v;
    }

    get errorMessage(): string {
        if (this.#errorBlock.classList.contains("hidden")) {
            return null;
        }

        return this.#errorMessage.innerHTML;
    }

    #errorTimeout: any = null;
    set errorMessage(msg) {
        if (this.#errorTimeout) {
            clearTimeout(this.#errorTimeout);
            this.#errorTimeout = null;
        }

        if (msg) {
            HtmlRender(
                this.#errorMessage,
                InnerHTML(msg)
            );

            show(this.#errorBlock);
            this.#errorBlock.classList.remove("hidden");
            this.#errorTimeout = setTimeout(() => {
                this.errorMessage = null;
                this.#errorTimeout = null;
            }, 5000);
        }
        else {
            this.#errorBlock.classList.add("hidden");
            this.#errorTimeout = setTimeout(() => {
                hide(this.#errorBlock);
                this.#errorTimeout = null;
            }, 1000 * ERROR_ANIMATION_LENGTH_SECONDS);
        }
    }

    show(): void {
        this.#form.reset();
        this.#dialog.show();
    }

    close(): void {
        this.#dialog.close();
    }

    cancel(): void {
        this.#confirmation.cancel();
    }

    get isOpen(): boolean {
        return this.#dialog.open;
    }

    async confirmed(): Promise<boolean> {
        return await success(once(this.#confirmation, "confirm", "cancel"));
    }

    async cancelled(): Promise<boolean> {
        return !(await this.confirmed());
    }

    validateForm(): boolean {
        return validateForm(this.#form);
    }
}

customElements.define("standard-dialog", StandardDialogElement);

