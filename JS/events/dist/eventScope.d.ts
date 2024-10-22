import { IDisposable } from "@juniper-lib/util";
import { TypedEvent, TypedEventTarget, TypedEventMap } from "./TypedEventTarget";
declare class EventScope implements IDisposable {
    #private;
    readonly handler: any;
    constructor(target: EventTarget, eventName: string, handler: any);
    dispose(): void;
}
export declare function eventScope<EventMapT extends TypedEventMap<string>, EventT extends string & keyof EventMapT = string & keyof EventMapT>(target: TypedEventTarget<EventMapT> | EventTarget, eventName: EventT, eventHandler: (evt: TypedEvent<EventT> & EventMapT[EventT]) => any): EventScope;
export {};
//# sourceMappingURL=eventScope.d.ts.map