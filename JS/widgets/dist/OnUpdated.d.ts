import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare function OnUpdated<TargetT extends EventTarget = EventTarget>(callback: eventHandler<UpdatedEvent<TargetT>>, opts?: boolean | AddEventListenerOptions): HtmlEvent<"updated", UpdatedEvent<TargetT>>;
export declare class UpdatedEvent<TargetT extends EventTarget> extends TypedEvent<"updated", TargetT> {
    constructor(bubbles?: boolean);
}
//# sourceMappingURL=OnUpdated.d.ts.map