import { IDisposable } from "@juniper-lib/tslib/using";
import { TypedEvent, TypedEventTarget, TypedEventMap } from "./TypedEventTarget";

class EventScope implements IDisposable {
    constructor(private readonly target: EventTarget, private readonly eventName: string, private readonly handler: any) {
        this.target.addEventListener(this.eventName, this.handler);
    }

    dispose(): void {
        this.target.removeEventListener(this.eventName, this.handler);
    }
}

export function eventScope<EventMapT extends TypedEventMap<string>, EventT extends string & keyof EventMapT = string & keyof EventMapT>(target: TypedEventTarget<EventMapT> | EventTarget, eventName: EventT, eventHandler: (evt: TypedEvent<EventT> & EventMapT[EventT]) => any): EventScope;
export function eventScope(target: EventTarget, eventName: string, eventHandler: any): EventScope {
    return new EventScope(target, eventName, eventHandler);
}