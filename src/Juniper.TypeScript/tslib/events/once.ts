import { Exception } from "../Exception";
import { Task } from "../Promises";
import { isGoodNumber, isNumber, isString } from "../typeChecks";
import { TypedEventBase } from "./EventBase";

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
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string>
    (target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, rejectEvt?: keyof EventsT & string, timeout?: number): Promise<EventsT[ResolveEventKeyT]>;
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string>
    (target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, timeout?: number): Promise<EventsT[ResolveEventKeyT]>;
export function once<
    EventsT,
    ResolveEventKeyT extends keyof EventsT & string,
    RejectEventKeyT extends keyof EventsT & string>
    (target: EventTarget, resolveEvt: ResolveEventKeyT, rejectEvt?: RejectEventKeyT | number, timeout?: number): Promise<EventsT[ResolveEventKeyT]> {

    if (isGoodNumber(rejectEvt)) {
        timeout = rejectEvt;
        rejectEvt = undefined;
    }

    if (target instanceof EventTarget) {
        if (!targetValidateEvent(target, resolveEvt)) {
            throw new Exception(`Target does not have a ${resolveEvt} resolution event`);
        }
        if (isString(rejectEvt) && !targetValidateEvent(target, rejectEvt)) {
            throw new Exception(`Target does not have a ${rejectEvt} rejection event`);
        }
    }

    const task = new Task<EventsT[ResolveEventKeyT]>();

    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }

    const register = (evt: RejectEventKeyT | ResolveEventKeyT, callback: any) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    }

    if (isString(rejectEvt)) {
        register(rejectEvt, task.reject);
    }

    register(resolveEvt, task.resolve);

    return task;
};


/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param rejectEvt - the name of the event that could reject the Promise this method creates.
 * @param [timeout] - the number of milliseconds to wait for the resolveEvt, before rejecting.
 */
export function success<EventsT>(
    target: TypedEventBase<EventsT> | EventTarget,
    resolveEvt: keyof EventsT & string,
    rejectEvt: keyof EventsT & string,
    timeout?: number): Promise<boolean> {

    if (target instanceof EventTarget) {
        if (!targetValidateEvent(target, resolveEvt)) {
            throw new Exception(`Target does not have a ${resolveEvt} resolution event`);
        }
        if (isString(rejectEvt) && !targetValidateEvent(target, rejectEvt)) {
            throw new Exception(`Target does not have a ${rejectEvt} rejection event`);
        }
    }

    const task = new Task<boolean>();

    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }

    const register = (evt: keyof EventsT & string, callback: any) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    }

    register(rejectEvt, () => task.resolve(false));
    register(resolveEvt, () => task.resolve(true));

    return task;
};