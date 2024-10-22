import { arrayScan, isFunction, isObject, once, singleton, stringRandom, success } from "@juniper-lib/util";
import { AriaLabelledBy, Button, ClassList, CustomData, Dialog, Div, FAIcon, FormTag, H1, HtmlProp, ID, ImportDOM, Name, OnClick, OnEscapeKeyPressed, Query, SingletonStyleBlob, SlotTag, TabIndex, TemplateInstance, TitleAttr, TypedHTMLElement, alignItems, backgroundColor, border, borderRadius, boxShadow, display, height, hide, left, margin, marginLeft, marginTop, maxHeight, maxWidth, opacity, padding, perc, pointerEvents, position, registerFactory, rem, rgba, right, rule, show, textAlign, top, transition, translate, vh, vw, width } from "@juniper-lib/dom";
import { Confirmation } from "@juniper-lib/events";
import { elementSetDisplay } from '../../dom/src/util';
import { StandardDialogClosingEvent, StandardDialogShowingEvent, StandardDialogShownEvent, StandardDialogSubmitEvent, StandardDialogValidatingEvent } from "./IDialog";
import { IKEA } from "./IKEA";
import { ButtonPrimary, ButtonSecondary, resetValidation, validateForm } from "./widgets";
const ERROR_ANIMATION_LENGTH_SECONDS = 0.5;
export function CancelButtonText(value) { return new HtmlProp("cancelButtonText", value); }
export function SaveButtonText(value) { return new HtmlProp("saveButtonText", value); }
export class StandardDialogElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "cancelable",
        "open"
    ]; }
    static async import(path) {
        const elements = await ImportDOM(path);
        document.body.append(...elements);
        const dialog = arrayScan(elements, e => e.tagName === "STANDARD-DIALOG");
        return dialog;
    }
    #template;
    #dialog;
    #title;
    #body;
    get body() { return this.#body; }
    #form;
    get form() { return this.#form; }
    #cancelButton;
    get cancelButtonText() { return this.#cancelButton.innerHTML; }
    set cancelButtonText(v) { this.#cancelButton.innerHTML = v; }
    #saveButton;
    get saveButtonText() { return this.#saveButton.innerHTML; }
    set saveButtonText(v) { this.#saveButton.innerHTML = v; }
    #errorBlock;
    #errorMessage;
    #confirmation;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::StandardDialogElement", () => rule("standard-dialog", rule(" > dialog.modal", position("fixed"), top(0), left(0), width(vw(100)), maxWidth(vw(100)), height(vh(100)), maxHeight(vh(100)), pointerEvents("initial"), margin(0)), rule(" > dialog.modal[open]", display("block"), border("none"), borderRadius(0), backgroundColor(rgba(0, 0, 0, 0.5))), rule(" > dialog.modal-dialog-scrollable > .modal-content", position("fixed"), maxWidth("calc(100vw - 10em)"), maxHeight("calc(100vh - 4em)"), width("unset"), top(perc(50)), left(perc(50)), translate(perc(-50), perc(-50)), boxShadow("rgba(0, 0, 0, 27%) 0px 5px 10px")), rule(" .modal-header", display("block"), textAlign("left")), rule(" .modal-header .btn-dismiss-dialog", position("absolute"), top(0), right(0), margin(rem(1))), rule(" .modal-error", display("flex"), alignItems("center"), backgroundColor("orange"), padding("0.5rem"), marginTop("0.5rem"), transition(`opacity ${ERROR_ANIMATION_LENGTH_SECONDS}s ease-out`)), rule(" .modal-error.hidden", opacity(0)), rule(" .modal-error-message", marginLeft(rem(0.5)), width(perc(100)), textAlign("center"))));
        this.#template = TemplateInstance("Juniper::Widgets::StandardDialogElement", () => Dialog(ClassList("modal", "modal-dialog", "modal-dialog-scrollable", "modal-xl"), TabIndex(-1), FormTag(ClassList("modal-content"), Div(ClassList("modal-header"), SlotTag(Name("modal-title")), Div(ClassList("modal-error", "hidden"), FAIcon("warning"), Div(ClassList("modal-error-message"), "[ERROR MESSAGE]"), Button(ClassList("btn-close", "btn-dismiss-error"), CustomData("bs-dismiss", "modal"), TitleAttr("Dismiss error"))), Button(ClassList("btn-close", "btn-dismiss-dialog"), CustomData("bs-dismiss", "modal"), TitleAttr("Close"))), SlotTag(Name("modal-body")), Div(ClassList("modal-footer"), SlotTag(Name("modal-controls")), ButtonSecondary("Close dialog without saving", "Cancel", CustomData("bs-dismiss", "modal"), ClassList("btn-cancel")), ButtonPrimary("Save changes", "Save changes", ClassList("btn-save"))))));
        const titleID = stringRandom(12);
        this.#title = H1(Query(this, "[slot=\"modal-title\"]"), ID(titleID));
        this.#body = Div(Query(this, "[slot=\"modal-body\"]"));
        this.#errorBlock = Div(Query(this, ".modal-error"));
        hide(this.#errorBlock);
        this.#errorMessage = Div(Query(this, ".modal-error-message"));
        Button(Query(this, ".btn-dismiss-error"), OnClick(() => this.errorMessage = null));
        this.#dialog = Dialog(Query(this, "dialog"), AriaLabelledBy(titleID), OnEscapeKeyPressed(this.#confirmation.cancel));
        this.#form = FormTag(Query(this, "form"));
        Button(Query(this, ".btn-dismiss-dialog"), OnClick(this.#confirmation.cancel));
        this.#cancelButton = Button(Query(this, ".btn-cancel"), OnClick(this.#confirmation.cancel));
        this.#saveButton = Button(Query(this, ".btn-save"), OnClick(this.#confirmation.confirm));
        this.#confirmation = new Confirmation();
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.append(this.#template);
            IKEA(this);
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name) {
            case "cancelable":
                elementSetDisplay(this.#cancelButton, this.cancelable);
                break;
            case "open":
                this.#dialog.open = this.open;
                break;
        }
    }
    get cancelable() { return this.hasAttribute("cancelable"); }
    set cancelable(v) { this.toggleAttribute("cancelable", v); }
    get title() {
        return this.#title.innerHTML;
    }
    set title(v) {
        this.#title.innerHTML = v;
    }
    get errorMessage() {
        if (this.#errorBlock.classList.contains("hidden")) {
            return null;
        }
        return this.#errorMessage.innerHTML;
    }
    #errorTimeout = null;
    set errorMessage(msg) {
        if (this.#errorTimeout) {
            clearTimeout(this.#errorTimeout);
            this.#errorTimeout = null;
        }
        if (msg) {
            this.#errorMessage.replaceChildren(msg);
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
    get open() { return this.hasAttribute("open"); }
    set open(v) { this.toggleAttribute("open", v); }
    async #onShowing(value) {
        this.#form.reset();
        resetValidation(this.form);
        const task = new StandardDialogShowingEvent(value);
        this.dispatchEvent(task);
        await task;
    }
    async show(value) {
        await this.#onShowing(value);
        this.#dialog.show();
        return await this.#getResult(value);
    }
    async showModal(value) {
        await this.#onShowing(value);
        this.#dialog.showModal();
        return await this.#getResult(value);
    }
    #shownEvt = new StandardDialogShownEvent();
    async #getResult(originalInput) {
        try {
            resetValidation(this.form);
            this.dispatchEvent(this.#shownEvt);
            while (this.open) {
                if (await this.cancelled()) {
                    break;
                }
                try {
                    if (this.#validateForm()) {
                        const task = new StandardDialogSubmitEvent(originalInput);
                        this.dispatchEvent(task);
                        const value = await task;
                        return value;
                    }
                }
                catch (err) {
                    console.error(err);
                    if (isObject(err)) {
                        if ("message" in err) {
                            this.errorMessage = err.message;
                        }
                        else if ("error" in err) {
                            this.errorMessage = err.error;
                        }
                        else if ("target" in err
                            && isObject(err.target)
                            && "error" in err.target) {
                            this.errorMessage = err.target.error;
                        }
                        else if ("toString" in err
                            && isFunction(err.toString)) {
                            this.errorMessage = err.toString();
                        }
                    }
                    else {
                        this.errorMessage = "Unknown error";
                    }
                }
            }
        }
        finally {
            this.close();
        }
        return undefined;
    }
    #closingEvt = new StandardDialogClosingEvent();
    close() {
        this.dispatchEvent(this.#closingEvt);
        this.#dialog.close();
    }
    cancel() {
        this.#confirmation.cancel();
    }
    confirm() {
        this.#confirmation.confirm();
    }
    async confirmed() {
        return await success(once(this.#confirmation, "confirm", "cancel"));
    }
    async cancelled() {
        return !(await this.confirmed());
    }
    #validateForm() {
        const evt = new StandardDialogValidatingEvent();
        this.dispatchEvent(evt);
        return validateForm(this.#form)
            && !evt.defaultPrevented;
    }
    static install() {
        return singleton("Juniper::Widgets::StandardDialogElement", () => registerFactory("standard-dialog", StandardDialogElement));
    }
}
export function StandardDialog(...rest) {
    return StandardDialogElement.install()(...rest);
}
//# sourceMappingURL=StandardDialogElement.js.map