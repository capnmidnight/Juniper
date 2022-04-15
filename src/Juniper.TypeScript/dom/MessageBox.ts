import { once, TypedEvent, TypedEventBase } from "@juniper/events";


export class MessageBoxConfirmationEvent extends TypedEvent<"confirmed"> {
    constructor(public confirmed: boolean) {
        super("confirmed");
    }
}

interface MessageBoxEvents {
    confirmed: MessageBoxConfirmationEvent;
}

export class MessageBox extends TypedEventBase<MessageBoxEvents> {
    private msgBoxConfirm: HTMLButtonElement;
    private msgBoxCancel: HTMLButtonElement;

    constructor(private msgBox: HTMLElement) {
        super();

        this.msgBoxConfirm = this.msgBox.querySelector<HTMLButtonElement>("button.confirm");
        this.msgBoxCancel = this.msgBox.querySelector<HTMLButtonElement>("button.cancel");

        this.msgBoxConfirm.addEventListener("click", () => {
            this.dispatchEvent(new MessageBoxConfirmationEvent(true));
        });

        this.msgBoxCancel.addEventListener("click", () => {
            this.dispatchEvent(new MessageBoxConfirmationEvent(false));
        });
    }

    async show(): Promise<boolean> {
        this.msgBox.style.display = "block";
        const evt = await once(this, "confirmed");
        this.msgBox.style.display = "none";
        return evt.confirmed;
    }
}