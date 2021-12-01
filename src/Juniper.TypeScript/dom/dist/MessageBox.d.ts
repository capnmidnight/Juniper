import { TypedEvent, TypedEventBase } from "juniper-tslib";
export declare class MessageBoxConfirmationEvent extends TypedEvent<"confirmed"> {
    confirmed: boolean;
    constructor(confirmed: boolean);
}
interface MessageBoxEvents {
    confirmed: MessageBoxConfirmationEvent;
}
export declare class MessageBox extends TypedEventBase<MessageBoxEvents> {
    private msgBox;
    private msgBoxConfirm;
    private msgBoxCancel;
    constructor(msgBox: HTMLElement);
    show(): Promise<boolean>;
}
export {};
