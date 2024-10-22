import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";


export function OnDelete<TargetT extends EventTarget = EventTarget>(callback: eventHandler<DeleteEvent<TargetT>>, opts?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("delete", callback, opts, false);
}

export class DeleteEvent<TargetT extends EventTarget> extends TypedEvent<"delete", TargetT> {
    constructor(bubbles = false) {
        super("delete", { bubbles });
    }
}
