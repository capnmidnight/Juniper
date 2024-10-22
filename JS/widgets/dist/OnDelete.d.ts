import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare function OnDelete<TargetT extends EventTarget = EventTarget>(callback: eventHandler<DeleteEvent<TargetT>>, opts?: boolean | AddEventListenerOptions): HtmlEvent<"delete", DeleteEvent<TargetT>>;
export declare class DeleteEvent<TargetT extends EventTarget> extends TypedEvent<"delete", TargetT> {
    constructor(bubbles?: boolean);
}
//# sourceMappingURL=OnDelete.d.ts.map