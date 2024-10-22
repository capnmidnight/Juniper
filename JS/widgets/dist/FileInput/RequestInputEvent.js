import { HtmlEvent } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export class RequestInputEvent extends TypedEvent {
    #value;
    #task;
    constructor(value, task) {
        super("requestinput", { bubbles: true });
        this.#value = value;
        this.#task = task;
    }
    get value() { return this.#value; }
    resolve(value) {
        this.#task.resolve(value);
    }
    reject() {
        this.#task.reject("Cancelled");
    }
}
export function OnRequestInput(evt, options) {
    return new HtmlEvent("requestinput", evt, options, false);
}
//# sourceMappingURL=RequestInputEvent.js.map