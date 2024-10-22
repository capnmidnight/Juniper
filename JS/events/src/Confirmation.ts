import { success, once } from "@juniper-lib/util";
import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";

type ConfirmationEventsMap = {
    "confirm": TypedEvent<"confirm">;
    "cancel": TypedEvent<"cancel">;
}

export class Confirmation extends TypedEventTarget<ConfirmationEventsMap> {
    readonly confirm = this.dispatchEvent.bind(this, new TypedEvent("confirm"));
    confirmed() { return success(once(this, "confirm", "cancel")); }
    
    readonly cancel = this.dispatchEvent.bind(this, new TypedEvent("cancel"));
    cancelled() { return success(once(this, "cancel", "confirm")); }
}