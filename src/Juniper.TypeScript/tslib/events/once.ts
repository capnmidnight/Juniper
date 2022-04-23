import { alwaysFalse, alwaysTrue, Exception, isNullOrUndefined, isNumber, isString } from "../";
import { EventBase, TypedEventBase } from "./EventBase";
import { Task } from "./Task";

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
export function once<EventsT>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: keyof EventsT & string, timeout: number, ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[typeof resolveEvt], EventsT[keyof EventsT]>;
export function once<EventsT>(target: TypedEventBase<EventsT> | EventTarget, resolveEvt: keyof EventsT & string, ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[typeof resolveEvt], EventsT[keyof EventsT]>;
export function once<EventsT>(target: EventTarget, resolveEvt: keyof EventsT & string, rejectEvtOrTimeout?: number | (keyof EventsT & string), ...rejectEvts: (keyof EventsT & string)[]): Task<EventsT[typeof resolveEvt], EventsT[keyof EventsT]> {

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

    const task = new Task<EventsT[typeof resolveEvt], EventsT[keyof EventsT]>();

    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }

    const register = (evt: keyof EventsT & string, callback: (evt: Event) => void) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    }

    const onResolve = (evt: Event) => task.resolve(evt as any as EventsT[typeof resolveEvt]);
    const onReject = (evt: Event) => task.reject(evt as any as EventsT[keyof EventsT]);
    register(resolveEvt, onResolve);

    for (const rejectEvt of rejectEvts) {
        register(rejectEvt, onReject);
    }

    return task;
}

export function success<T, E>(task: Task<T, E>): Promise<boolean> {
    return task.then(alwaysTrue)
        .catch(alwaysFalse);
};