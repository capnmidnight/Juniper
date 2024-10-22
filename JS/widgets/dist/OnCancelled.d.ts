import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare function OnCancelled<TargetT extends EventTarget = EventTarget>(callback: eventHandler<CancelledEvent<TargetT>>, opts?: boolean | AddEventListenerOptions): HtmlEvent<"cancelled", CancelledEvent<TargetT>>;
export declare class CancelledEvent<TargetT extends EventTarget> extends TypedEvent<"cancelled", TargetT> {
    constructor(bubbles?: boolean);
}
//# sourceMappingURL=OnCancelled.d.ts.map