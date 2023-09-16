import { alwaysFalse, alwaysTrue } from "@juniper-lib/tslib/dist/identity";
import { isNullOrUndefined, isNumber, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { CustomEventTarget } from "./EventTarget";
import { Task } from "./Task";
function targetValidateEvent(target, type) {
    return ("on" + type) in target;
}
export function once(target, resolveEvt, rejectEvtOrTimeout, ...rejectEvts) {
    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }
    let timeout = undefined;
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
    const task = new Task();
    if (isNumber(timeout)) {
        const timeoutHandle = setTimeout(task.reject, timeout, `'${resolveEvt}' has timed out.`);
        task.finally(clearTimeout.bind(globalThis, timeoutHandle));
    }
    const register = (evt, callback) => {
        target.addEventListener(evt, callback);
        task.finally(() => target.removeEventListener(evt, callback));
    };
    register(resolveEvt, (evt) => task.resolve(evt));
    const onReject = (evt) => task.reject(evt);
    for (const rejectEvt of rejectEvts) {
        register(rejectEvt, onReject);
    }
    return task;
}
export function success(task) {
    return task.then(alwaysTrue)
        .catch(alwaysFalse);
}
//# sourceMappingURL=once.js.map