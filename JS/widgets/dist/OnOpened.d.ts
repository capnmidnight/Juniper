import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare function OnOpened<TargetT extends EventTarget = EventTarget>(callback: eventHandler<OpenedEvent<TargetT>>, opts?: boolean | AddEventListenerOptions): HtmlEvent<"opened", OpenedEvent<TargetT>>;
export declare class OpenedEvent<TargetT extends EventTarget> extends TypedEvent<"opened", TargetT> {
    constructor(bubbles?: boolean);
}
//# sourceMappingURL=OnOpened.d.ts.map