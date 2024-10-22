import { ITypedEventTarget, TaskEvent, TypedEvent } from "@juniper-lib/events";
import { ICancelable } from "./Cancelable";

export class StandardDialogShowingEvent<InputT> extends TaskEvent<"showing", void> {
    constructor(public readonly value: InputT) {
        super("showing");
    }
}

export class StandardDialogShownEvent extends TypedEvent<"shown"> {
    constructor() {
        super("shown");
    }
}

export class StandardDialogClosingEvent extends TypedEvent<"closing"> {
    constructor() {
        super("closing");
    }
}

export class StandardDialogValidatingEvent extends TypedEvent<"validating"> {
    constructor() {
        super("validating");
    }
}

export class StandardDialogSubmitEvent<InputT, OutputT> extends TaskEvent<"submit", OutputT> {
    constructor(public readonly originalValue: InputT) {
        super("submit");
    }
}

export type StandardDialogEventsMap<InputT, OutputT> = {
    "showing": StandardDialogShowingEvent<InputT>;
    "shown": StandardDialogShownEvent;
    "closing": StandardDialogClosingEvent;
    "validating": StandardDialogValidatingEvent;
    "submit": StandardDialogSubmitEvent<InputT, OutputT>;
}

export interface IDialog<InputT = void, OutputT = InputT>
    extends ICancelable, ITypedEventTarget<StandardDialogEventsMap<InputT, OutputT>> {
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
