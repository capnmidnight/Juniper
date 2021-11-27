import { once, TypedEvent, TypedEventBase } from "juniper-tslib";
import { className, customData } from "./attrs";
import { backgroundColor, boxShadow, display, float, gridArea, gridTemplateColumns, gridTemplateRows, height, left, maxHeight, maxWidth, overflow, padding, position, rule, styles, textAlign, top, transform, width, zIndex } from "./css";
import { Button, Div, elementApply, elementSetText, H1, Style } from "./tags";
Style(rule(".dialog, .dialog-container", position("fixed")), rule(".dialog", top("0"), left("0"), width("100%"), height("100%"), backgroundColor("rgba(0, 0, 0, 0.5)"), zIndex(100)), rule(".dialog-container", top("50%"), left("50%"), maxWidth("100%"), maxHeight("100%"), transform("translateX(-50%) translateY(-50%)"), backgroundColor("white"), boxShadow("rgba(0,0,0,0.5) 0px 5px 30px"), display("grid"), gridTemplateColumns("2em auto 2em"), gridTemplateRows("auto 1fr auto 2em")), rule(".dialog .title-bar", gridArea("1/1/2/-1"), padding("0.25em")), rule(".dialog-content", gridArea("2/2/-4/-2"), overflow("auto")), rule(".dialog-controls", gridArea("-2/2/-3/-2")), rule(".dialog .confirm-button", float("right")), rule(".dialog h1, .dialog h2, .dialog h3, .dialog h4, .dialog h5, .dialog h6", textAlign("left")), rule(".dialog select", maxWidth("10em")));
export class DialogBox {
    element;
    subEventer = new TypedEventBase();
    _title;
    titleElement;
    container;
    contentArea;
    confirmButton;
    cancelButton;
    constructor(title) {
        this.element = Div(className("dialog"), customData("dialogname", title), styles(display("none")), this.container = Div(className("dialog-container"), this.titleElement = H1(className("title-bar"), title), this.contentArea = Div(className("dialog-content")), Div(className("dialog-controls"), this.confirmButton = Button("Confirm", className("btn btn-primary confirm-button")), this.cancelButton = Button("Cancel", className("btn btn-secondary cancel-button")))));
        this.confirmButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("confirm")));
        this.cancelButton.addEventListener("click", () => this.subEventer.dispatchEvent(new TypedEvent("cancel")));
        elementApply(document.body, this);
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
    async showDialog() {
        await this.onShowing();
        this.element.style.display = "block";
        this.onShown();
        try {
            await once(this.subEventer, "confirm", "cancel");
            await this.onConfirm();
            return true;
        }
        catch {
            this.onCancel();
            return false;
        }
        finally {
            await this.onClosing();
            this.element.style.display = "none";
            this.onClosed();
        }
    }
}
