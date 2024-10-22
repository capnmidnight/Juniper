import { ITypedEventTarget, TaskEvent, TypedEvent } from "@juniper-lib/events";
import { ICancelable } from "./Cancelable";
export declare class StandardDialogShowingEvent<InputT> extends TaskEvent<"showing", void> {
    readonly value: InputT;
    constructor(value: InputT);
}
export declare class StandardDialogShownEvent extends TypedEvent<"shown"> {
    constructor();
}
export declare class StandardDialogClosingEvent extends TypedEvent<"closing"> {
    constructor();
}
export declare class StandardDialogValidatingEvent extends TypedEvent<"validating"> {
    constructor();
}
export declare class StandardDialogSubmitEvent<InputT, OutputT> extends TaskEvent<"submit", OutputT> {
    readonly originalValue: InputT;
    constructor(originalValue: InputT);
}
export type StandardDialogEventsMap<InputT, OutputT> = {
    "showing": StandardDialogShowingEvent<InputT>;
    "shown": StandardDialogShownEvent;
    "closing": StandardDialogClosingEvent;
    "validating": StandardDialogValidatingEvent;
    "submit": StandardDialogSubmitEvent<InputT, OutputT>;
};
export interface IDialog<InputT = void, OutputT = InputT> extends ICancelable, ITypedEventTarget<StandardDialogEventsMap<InputT, OutputT>> {
    readonly body: HTMLElement;
    readonly form: HTMLFormElement;
    cancelButtonText: string;
    saveButtonText: string;
    title: string;
    errorMessage: string;
    open: boolean;
    show(value: InputT): Promise<OutputT>;
    showModal(value: InputT): Promise<OutputT>;
    close(): void;
    cancel(): void;
    confirm(): void;
    confirmed(): Promise<boolean>;
    cancelled(): Promise<boolean>;
}
//# sourceMappingURL=IDialog.d.ts.map