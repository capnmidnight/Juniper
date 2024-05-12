import { IDisposable } from "@juniper-lib/util";
import { TypedEvent, TypedEventTarget, TypedEventMap } from "./TypedEventTarget";

class EventScope implements IDisposable {
    readonly #target: EventTarget;
    readonly #eventName: string;

    constructor(target: EventTarget, eventName: string, public readonly handler: any) {
        this.#target = target;
        this.#eventName = eventName;
        this.#target.addEventListener(this.#eventName, this.handler);
    }

    dispose(): void {
        this.#target.removeEventListener(this.#eventName, this.handler);
    }
}

export function eventScope<EventMapT extends TypedEventMap<string>, EventT extends string & keyof EventMapT = string & keyof EventMapT>(target: TypedEventTarget<EventMapT> | EventTarget, eventName: EventT, eventHandler: (evt: TypedEvent<EventT> & EventMapT[EventT]) => any): EventScope;
export function eventScope(target: EventTarget, eventName: string, eventHandler: any): EventScope {
    return new EventScope(target, eventName, eventHandler);
}
