import type { TypedEventBase } from "../";
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param [rejectEvt] - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export declare function once<EventsT, ResolveEventKeyT extends keyof EventsT & string>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, rejectEvt?: keyof EventsT & string, timeout?: number): Promise<EventsT[ResolveEventKeyT]>;
export declare function once<EventsT, ResolveEventKeyT extends keyof EventsT & string>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, timeout?: number): Promise<EventsT[ResolveEventKeyT]>;
