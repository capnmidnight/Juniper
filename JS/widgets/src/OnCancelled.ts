import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";

export function OnCancelled<TargetT extends EventTarget = EventTarget>(callback: eventHandler<CancelledEvent<TargetT>>, opts?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("cancelled", callback, opts, false);
}

export class CancelledEvent<TargetT extends EventTarget> extends TypedEvent<"cancelled", TargetT> {
    constructor(bubbles = false) {
        super("cancelled", { bubbles });
    }
}
