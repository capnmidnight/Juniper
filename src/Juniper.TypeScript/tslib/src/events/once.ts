import { Exception } from "../Exception";
import { isDefined, isGoodNumber, isString } from "../typeChecks";
import { add } from "./add";
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
    ResolveEventKeyT extends keyof EventsT & string>
    (target: TypedEventBase<EventsT> | EventTarget, resolveEvt: ResolveEventKeyT, rejectEvt?: (keyof EventsT & string) | number, timeout?: number): Promise<EventsT[ResolveEventKeyT]> {

    if (timeout == null
        && isGoodNumber(rejectEvt)) {
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

    return new Promise((resolve: (value: any) => void, reject) => {
        const remove = () => {
            target.removeEventListener(resolveEvt, resolve);
        };
        resolve = add(remove, resolve);
        reject = add(remove, reject);

        if (isString(rejectEvt)) {
            const rejectEvt2 = rejectEvt;
            const remove = () => {
                target.removeEventListener(rejectEvt2, reject);
            };

            resolve = add(remove, resolve);
            reject = add(remove, reject);
        }

        if (isDefined(timeout)) {
            const timer = setTimeout(reject, timeout, `'${resolveEvt}' has timed out.`),
                cancel = () => clearTimeout(timer);
            resolve = add(cancel, resolve);
            reject = add(cancel, reject);
        }

        target.addEventListener(resolveEvt, resolve);

        if (isString(rejectEvt)) {
            target.addEventListener(rejectEvt, () => {
                reject("Rejection event found");
            });
        }
    });
};