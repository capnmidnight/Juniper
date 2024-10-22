import { alwaysFalse, alwaysTrue } from "./filters";
import { isDefined, isNullOrUndefined, isNumber } from "./typeChecks";
/**
 * Check to see if a an `on###` type event property exists in the given object.
 */
export function targetValidateEvent(target, type) {
    if (!(("on" + type) in target)) {
        console.warn(`The supplied target might not have an "${type}" event defined.`, target);
    }
}
export async function once(target, resolveEvt, rejectEvtOrTimeout, ...rejectEvts) {
    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }
    let timeout = undefined;
    if (typeof rejectEvtOrTimeout === "string") {
        rejectEvts.unshift(rejectEvtOrTimeout);
    }
    else if (typeof rejectEvtOrTimeout === "number") {
        timeout = rejectEvtOrTimeout;
    }
    if (target instanceof EventTarget) {
        [resolveEvt, ...rejectEvts].forEach(targetValidateEvent.bind(null, target));
    }
    let timeoutHandle = null;
    const listeners = [];
    try {
        return await new Promise((resolve, reject) => {
            if (typeof timeout === "number") {
                timeoutHandle = setTimeout(reject, timeout, `'${resolveEvt}' has timed out.`);
            }
            const register = (evtName, callback) => {
                target.addEventListener(evtName, callback);
                listeners.push([evtName, callback]);
            };
            register(resolveEvt, resolve);
            for (const rejectEvt of rejectEvts) {
                register(rejectEvt, reject);
            }
        });
    }
    finally {
        if (isDefined(timeoutHandle)) {
            clearTimeout(timeoutHandle);
            timeoutHandle = null;
        }
        for (const [name, handler] of listeners) {
            target.removeEventListener(name, handler);
        }
    }
}
export async function twonce(target, resolveEvt1, resolveEvt2, rejectEvtOrTimeout, ...rejectEvts) {
    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }
    let timeout = undefined;
    if (typeof rejectEvtOrTimeout === "string") {
        rejectEvts.unshift(rejectEvtOrTimeout);
    }
    else if (typeof rejectEvtOrTimeout === "number") {
        timeout = rejectEvtOrTimeout;
    }
    if (target instanceof EventTarget) {
        [resolveEvt1, ...rejectEvts].forEach(targetValidateEvent.bind(null, target));
    }
    let timeoutHandle = null;
    const listeners = [];
    try {
        return await new Promise((resolve, reject) => {
            if (typeof timeout === "number") {
                timeoutHandle = setTimeout(reject, timeout, `'${resolveEvt1}' has timed out.`);
            }
            const register = (evtName, callback) => {
                target.addEventListener(evtName, callback);
                listeners.push([evtName, callback]);
            };
            register(resolveEvt1, resolve);
            register(resolveEvt2, resolve);
            for (const rejectEvt of rejectEvts) {
                register(rejectEvt, reject);
            }
        });
    }
    finally {
        if (isDefined(timeoutHandle)) {
            clearTimeout(timeoutHandle);
            timeoutHandle = null;
        }
        for (const [name, handler] of listeners) {
            target.removeEventListener(name, handler);
        }
    }
}
export function success(promise) {
    return promise.then(alwaysTrue)
        .catch(alwaysFalse);
}
export function debounce(timeoutOrCallback, callback) {
    let timeout = 0;
    if (isNumber(timeoutOrCallback)) {
        timeout = timeoutOrCallback;
    }
    else {
        callback = timeoutOrCallback;
    }
    let handle = null;
    return function () {
        if (isDefined(handle)) {
            clearTimeout(handle);
            handle = null;
        }
        handle = setTimeout(() => {
            handle = null;
            callback(...arguments);
        }, timeout, ...arguments);
    };
}
export function debounceRAF(callback) {
    let handle = null;
    return function () {
        if (isDefined(handle)) {
            cancelAnimationFrame(handle);
            handle = null;
        }
        handle = requestAnimationFrame((time) => {
            handle = null;
            callback(time);
        });
    };
}
//# sourceMappingURL=events.js.map