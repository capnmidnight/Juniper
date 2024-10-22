import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare class IndexChangedEvent extends TypedEvent<"indexchanged"> {
    #private;
    get index(): number;
    constructor(index: number);
}
export declare function OnIndexChanged(evt: eventHandler<IndexChangedEvent>, options?: boolean | AddEventListenerOptions): HtmlEvent<"indexchanged", IndexChangedEvent>;
//# sourceMappingURL=IndexChangedEvent.d.ts.map