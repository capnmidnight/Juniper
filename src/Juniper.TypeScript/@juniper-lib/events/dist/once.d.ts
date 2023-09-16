import { EventMap } from "./EventTarget";
import { Task } from "./Task";
import { TypedEventTarget, TypedEventMap } from "./TypedEventTarget";
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param [rejectEvt] - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export declare function once<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(target: TypedEventTarget<EventMapT>, resolveEvt: EventT, timeout: number, ...rejectEvts: (keyof EventMapT & string)[]): Task<EventMapT[EventT]>;
export declare function once<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(target: TypedEventTarget<EventMapT>, resolveEvt: EventT, ...rejectEvts: (keyof EventMapT & string)[]): Task<EventMapT[EventT]>;
export declare function once<EventMapT extends EventMap, EventT extends keyof EventMapT = keyof EventMapT>(target: EventTarget, resolveEvt: EventT, rejectEvtOrTimeout?: number | string, ...rejectEvts: EventT[]): Task<Event>;
export declare function success<T>(task: Task<T>): Promise<boolean>;
//# sourceMappingURL=once.d.ts.map