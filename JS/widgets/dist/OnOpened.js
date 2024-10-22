import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export function OnOpened(callback, opts) {
    return new HtmlEvent("opened", callback, opts, false);
}
export class OpenedEvent extends TypedEvent {
    constructor(bubbles = false) {
        super("opened", { bubbles });
    }
}
//# sourceMappingURL=OnOpened.js.map