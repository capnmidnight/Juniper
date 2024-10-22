import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";

export function OnOpened<TargetT extends EventTarget = EventTarget>(callback: eventHandler<OpenedEvent<TargetT>>, opts?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("opened", callback, opts, false);
}

export class OpenedEvent<TargetT extends EventTarget> extends TypedEvent<"opened", TargetT> {
    constructor(bubbles = false) {
        super("opened", { bubbles });
    }
}