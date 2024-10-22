import { TypedHTMLElement } from "@juniper-lib/dom";
import { StandardDialog } from './StandardDialogElement';
export class BaseDialogElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "cancelable",
        "title"
    ]; }
    #title;
    constructor(title, body, ...rest) {
        super();
        title.slot = "modal-title";
        body.slot = "modal-body";
        this.dialog = StandardDialog(title, body, ...rest);
        this.#title = title;
    }
    Q(selectors) {
        return this.dialog.querySelector(selectors);
    }
    connectedCallback() {
        if (this.dialog.parentElement !== this) {
            this.append(this.dialog);
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
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
    get title() { return this.getAttribute("title"); }
    set title(v) { this.setAttribute("titie", v); }
    show(value) {
        return this.dialog.show(value);
    }
    showModal(value) {
        return this.dialog.showModal(value);
    }
    close() {
        this.dialog.close();
    }
    cancel() {
        this.dialog.cancel();
    }
    confirm() {
        this.dialog.confirm();
    }
    confirmed() {
        return this.dialog.confirmed();
    }
    cancelled() {
        return this.dialog.cancelled();
    }
}
//# sourceMappingURL=BaseDialogElement.js.map