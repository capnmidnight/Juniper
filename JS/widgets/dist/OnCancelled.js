import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export function OnCancelled(callback, opts) {
    return new HtmlEvent("cancelled", callback, opts, false);
}
export class CancelledEvent extends TypedEvent {
    constructor(bubbles = false) {
        super("cancelled", { bubbles });
    }
}
//# sourceMappingURL=OnCancelled.js.map