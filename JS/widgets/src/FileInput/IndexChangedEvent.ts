import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";


export class IndexChangedEvent extends TypedEvent<"indexchanged"> {
    readonly #index: number;
    get index() { return this.#index; }
    constructor(index: number) {
        super("indexchanged", { bubbles: true });
        this.#index = index;
    }
}


export function OnIndexChanged(evt: eventHandler<IndexChangedEvent>, options?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("indexchanged", evt, options, false);
}
