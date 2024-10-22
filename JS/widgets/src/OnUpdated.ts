import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";

export function OnUpdated<TargetT extends EventTarget = EventTarget>(callback: eventHandler<UpdatedEvent<TargetT>>, opts?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("updated", callback, opts, false);
}

export class UpdatedEvent<TargetT extends EventTarget> extends TypedEvent<"updated", TargetT>{
    constructor(bubbles = false) {
        super("updated", { bubbles });
    }
}

