import { Exception } from "../Exception";
import { isDefined, isGoodNumber, isString } from "../typeChecks";
import { add } from "./add";
function targetValidateEvent(target, type) {
    return ("on" + type) in target;
}
export function once(target, resolveEvt, rejectEvt, timeout) {
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
    return new Promise((resolve, reject) => {
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
            const timer = setTimeout(reject, timeout, `'${resolveEvt}' has timed out.`), cancel = () => clearTimeout(timer);
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
}
;
