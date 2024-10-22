import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export function OnDelete(callback, opts) {
    return new HtmlEvent("delete", callback, opts, false);
}
export class DeleteEvent extends TypedEvent {
    constructor(bubbles = false) {
        super("delete", { bubbles });
    }
}
//# sourceMappingURL=OnDelete.js.map