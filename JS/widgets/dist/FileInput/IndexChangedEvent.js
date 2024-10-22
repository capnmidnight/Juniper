import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export class IndexChangedEvent extends TypedEvent {
    #index;
    get index() { return this.#index; }
    constructor(index) {
        super("indexchanged", { bubbles: true });
        this.#index = index;
    }
}
export function OnIndexChanged(evt, options) {
    return new HtmlEvent("indexchanged", evt, options, false);
}
//# sourceMappingURL=IndexChangedEvent.js.map