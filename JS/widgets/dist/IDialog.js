import { TaskEvent, TypedEvent } from "@juniper-lib/events";
export class StandardDialogShowingEvent extends TaskEvent {
    constructor(value) {
        super("showing");
        this.value = value;
    }
}
export class StandardDialogShownEvent extends TypedEvent {
    constructor() {
        super("shown");
    }
}
export class StandardDialogClosingEvent extends TypedEvent {
    constructor() {
        super("closing");
    }
}
export class StandardDialogValidatingEvent extends TypedEvent {
    constructor() {
        super("validating");
    }
}
export class StandardDialogSubmitEvent extends TaskEvent {
    constructor(originalValue) {
        super("submit");
        this.originalValue = originalValue;
    }
}
//# sourceMappingURL=IDialog.js.map