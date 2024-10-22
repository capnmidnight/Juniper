import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export function OnUpdated(callback, opts) {
    return new HtmlEvent("updated", callback, opts, false);
}
export class UpdatedEvent extends TypedEvent {
    constructor(bubbles = false) {
        super("updated", { bubbles });
    }
}
//# sourceMappingURL=OnUpdated.js.map