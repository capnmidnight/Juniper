import { alwaysFalse, alwaysTrue } from "@juniper-lib/util";
import { isNullOrUndefined, isNumber, isString } from "@juniper-lib/util";
import { CustomEventTarget, EventMap } from "./EventTarget";
import { Task } from "./Task";
import { TypedEventTarget, TypedEventMap } from "./TypedEventTarget";

function targetValidateEvent(target: EventTarget, type: string) {
    return ("on" + type) in target;
}

/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param [rejectEvt] - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export function once<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(target: TypedEventTarget<EventMapT>, resolveEvt: EventT, timeout: number, ...rejectEvts: (keyof EventMapT & string)[]): Task<EventMapT[EventT]>;
export function once<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(target: TypedEventTarget<EventMapT>, resolveEvt: EventT, ...rejectEvts: (keyof EventMapT & string)[]): Task<EventMapT[EventT]>;
export function once<EventMapT extends EventMap, EventT extends keyof EventMapT = keyof EventMapT>(target: EventTarget, resolveEvt: EventT, rejectEvtOrTimeout?: number | string, ...rejectEvts: EventT[]): Task<Event>
export function once(target: EventTarget, resolveEvt: string, rejectEvtOrTimeout?: number | string, ...rejectEvts: string[]): Task<Event> {

    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }

    let timeout: number = undefined;
    if (isString(rejectEvtOrTimeout)) {
        rejectEvts.unshift(rejectEvtOrTimeout);
    }
    else if (isNumber(rejectEvtOrTimeout)) {
        timeout = rejectEvtOrTimeout;
    }

    if (!(target instanceof CustomEventTarget)) {
        if (!targetValidateEvent(target, resolveEvt)) {
            throw new Error(`Target does not have a ${resolveEvt} rejection event`);
        }

        for (const evt of rejectEvts) {
            if (!targetValidateEvent(target, evt)) {
                throw new Error(`Target does not have a ${evt} rejection event`);
            }
        }
    }

    const task = new Task<Event>();

    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }

    const register = (evt: string, callback: (evt: Event) => void) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    };

    register(resolveEvt, (evt) => task.resolve(evt));

    const onReject = (evt: Event) => task.reject(evt);
    for (const rejectEvt of rejectEvts) {
        register(rejectEvt, onReject);
    }

    return task;
}

export function success<T>(task: Task<T>): Promise<boolean> {
    return task.then(alwaysTrue)
        .catch(alwaysFalse);
}
