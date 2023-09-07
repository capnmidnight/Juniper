import { ClassList, CustomData } from "@juniper-lib/dom/attrs";
import { display } from "@juniper-lib/dom/css";
import { ButtonPrimary, ButtonSecondary, Div, H1, HtmlRender, elementIsDisplayed, elementSetDisplay, elementSetText } from "@juniper-lib/dom/tags";
import { Task } from "@juniper-lib/events/Task";
import "./styles.css";
export class DialogBox {
    constructor(title) {
        this.task = new Task(false);
        this.element = Div(ClassList("dialog"), display("none"), CustomData("dialogname", title), this.container = Div(ClassList("dialog-container"), this.titleElement = H1(ClassList("title-bar"), title), this.contentArea = Div(ClassList("dialog-content")), Div(ClassList("dialog-controls"), this.confirmButton = ButtonPrimary("Confirm", ClassList("confirm-button")), this.cancelButton = ButtonSecondary("Cancel", ClassList("cancel-button")))));
        this.confirmButton.addEventListener("click", this.task.resolver(true));
        this.cancelButton.addEventListener("click", this.task.resolver(false));
        HtmlRender(document.body, this);
    }
    get title() {
        return this._title;
    }
    set title(v) {
        elementSetText(this.titleElement, this._title = v);
    }
    async onShowing() {
        // nothing to do in the base case.
    }
    onShown() {
        // nothing to do in the base case.
    }
    async onConfirm() {
        // nothing to do in the base case.
    }
    onCancel() {
        // nothing to do in the base case.
    }
    async onClosing() {
        // nothing to do in the base case.
    }
    onClosed() {
        // nothing to do in the base case.
    }
    show(v) {
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
    async showDialog() {
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
//# sourceMappingURL=index.js.map