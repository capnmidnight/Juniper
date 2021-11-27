import { once, TypedEvent, TypedEventBase } from "juniper-tslib";
export class MessageBoxConfirmationEvent extends TypedEvent {
    confirmed;
    constructor(confirmed) {
        super("confirmed");
        this.confirmed = confirmed;
    }
}
export class MessageBox extends TypedEventBase {
    msgBox;
    msgBoxConfirm;
    msgBoxCancel;
    constructor(msgBox) {
        super();
        this.msgBox = msgBox;
        this.msgBoxConfirm = this.msgBox.querySelector("button.confirm");
        this.msgBoxCancel = this.msgBox.querySelector("button.cancel");
        this.msgBoxConfirm.addEventListener("click", () => {
            this.dispatchEvent(new MessageBoxConfirmationEvent(true));
        });
        this.msgBoxCancel.addEventListener("click", () => {
            this.dispatchEvent(new MessageBoxConfirmationEvent(false));
        });
    }
    async show() {
        this.msgBox.style.display = "block";
        const evt = await once(this, "confirmed");
        this.msgBox.style.display = "none";
        return evt.confirmed;
    }
}
