import { eventHandler } from "@juniper-lib/util";
import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent, Task } from "@juniper-lib/events";


export class RequestInputEvent<InputT, OutputT> extends TypedEvent<"requestinput"> {
    readonly #value: InputT;
    readonly #task: Task<OutputT>;
    constructor(value: InputT, task: Task<OutputT>) {
        super("requestinput", { bubbles: true });

        this.#value = value;
        this.#task = task;
    }

    get value() { return this.#value; }

    resolve(value: OutputT) {
        this.#task.resolve(value);
    }

    reject() {
        this.#task.reject("Cancelled");
    }
}

export function OnRequestInput<InputT, OutputT>(evt: eventHandler<RequestInputEvent<InputT, OutputT>>, options?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("requestinput", evt, options, false);
}
