import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent, Task } from "@juniper-lib/events";
export declare class RequestInputEvent<InputT, OutputT> extends TypedEvent<"requestinput"> {
    #private;
    constructor(value: InputT, task: Task<OutputT>);
    get value(): InputT;
    resolve(value: OutputT): void;
    reject(): void;
}
export declare function OnRequestInput<InputT, OutputT>(evt: eventHandler<RequestInputEvent<InputT, OutputT>>, options?: boolean | AddEventListenerOptions): HtmlEvent<"requestinput", RequestInputEvent<InputT, OutputT>>;
//# sourceMappingURL=RequestInputEvent.d.ts.map