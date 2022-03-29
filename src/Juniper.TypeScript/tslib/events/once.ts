import { Exception } from "../Exception";
import { Task } from "../events/Promises";
import { isNullOrUndefined, isNumber, isString } from "../typeChecks";
import { EventBase, TypedEventBase } from "./EventBase";

function targetValidateEvent(target: EventTarget, type: string) {
    return ("on" + type) in target;
}

function firstEventTask<EventsT, ResultT>(
    target: EventTarget,
    rejectEvtOrTimeout: number | (keyof EventsT & string),
    resolveEvt: keyof EventsT & string,
    rejectEvts: (keyof EventsT & string)[],
    onResolve: (task: Task<ResultT>, evt: Event) => void,
    onReject: (task: Task<ResultT>, evt: Event) => void): Task<ResultT> {

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

    if (!(target instanceof EventBase)) {
        if (!targetValidateEvent(target, resolveEvt)) {
            throw new Exception(`Target does not have a ${resolveEvt} rejection event`);
        }

        for (const evt of rejectEvts) {
            if (!targetValidateEvent(target, evt)) {
                throw new Exception(`Target does not have a ${evt} rejection event`);
            }
        }
    }

    const task = new Task<ResultT>();

    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }

    const register = (evt: keyof EventsT & string, callback: (evt: Event) => void) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    }

    register(resolveEvt, (evt) => onResolve(task, evt));

    for (const rejectEvt of rejectEvts) {
        register(rejectEvt, (evt) => onReject(task, evt));
    }

    return task;

}

/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param [rejectEvt] - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string>
    (target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, timeout: number, ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[ResolveEventKeyT]>;
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string>
    (target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[ResolveEventKeyT]>;
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string>
    (target: EventTarget, resolveEvt: ResolveEventKeyT, rejectEvtOrTimeout?: number | (keyof EventsT & string), ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[ResolveEventKeyT]> {
    return firstEventTask<EventsT, EventsT[ResolveEventKeyT]>(target, rejectEvtOrTimeout, resolveEvt, rejectEvts,
        (task, evt) => task.resolve(evt as any as EventsT[ResolveEventKeyT]),
        (task, evt) => task.reject(evt))
}


/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param rejectEvt - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export function success<EventsT>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: keyof EventsT & string, timeout: number, ...rejectEvts: (keyof EventsT & string)[]): Task<boolean>;
export function success<EventsT>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: keyof EventsT & string, ...rejectEvts: (keyof EventsT & string)[]): Task<boolean>;
export function success<EventsT>(
    target: EventTarget,
    resolveEvt: keyof EventsT & string,
    rejectEvtOrTimeout: keyof EventsT & string | number,
    ...rejectEvts: (keyof EventsT & string)[]): Task<boolean> {
    return firstEventTask<EventsT, boolean>(target, rejectEvtOrTimeout, resolveEvt, rejectEvts,
        (task) => task.resolve(true),
        (task) => task.resolve(false));
};