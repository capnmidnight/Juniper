import { alwaysFalse, alwaysTrue } from "./filters";
import { isDefined, isNullOrUndefined, isNumber } from "./typeChecks";

export type TimeoutHandle = ReturnType<typeof setTimeout>;

/**
 * Check to see if a an `on###` type event property exists in the given object.
 */
export function targetValidateEvent(target: EventTarget, type: string) {
    if (!(("on" + type) in target)) {
        console.warn(`The supplied target might not have an "${type}" event defined.`, target);
    }
}

/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export function once(target: EventTarget, resolveEvt: string, ...rejectEvts: string[]): Promise<any>;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param timeout - the number of milliseconds to wait for the resolveEvt, before rejecting.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export function once(target: EventTarget, resolveEvt: string, timeout: number, ...rejectEvts: string[]): Promise<any>;
export async function once(target: EventTarget, resolveEvt: string, rejectEvtOrTimeout: number | string, ...rejectEvts: string[]) {

    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }

    let timeout: number = undefined;
    if (typeof rejectEvtOrTimeout === "string") {
        rejectEvts.unshift(rejectEvtOrTimeout);
    }
    else if (typeof rejectEvtOrTimeout === "number") {
        timeout = rejectEvtOrTimeout;
    }

    if (target instanceof EventTarget) {
        [resolveEvt, ...rejectEvts].forEach(
            targetValidateEvent.bind(null, target));
    }

    let timeoutHandle: TimeoutHandle = null;
    const listeners: [string, (value: unknown) => void][] = [];

    try {
        return await new Promise((resolve, reject) => {
            if (typeof timeout === "number") {
                timeoutHandle = setTimeout(reject, timeout, `'${resolveEvt}' has timed out.`);
            }

            const register = (evtName: string, callback: (value: unknown) => void) => {
                target.addEventListener(evtName, callback);
                listeners.push([evtName, callback]);
            };

            register(resolveEvt, resolve);

            for (const rejectEvt of rejectEvts) {
                register(rejectEvt, reject);
            }
        });
    } finally {
        if (isDefined(timeoutHandle)) {
            clearTimeout(timeoutHandle);
            timeoutHandle = null;
        }

        for (const [name, handler] of listeners) {
            target.removeEventListener(name, handler);
        }
    }
}


/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt1 - the name of the first event that will resolve the Promise this method creates.
 * @param resolveEvt2 - the name of the second event that will resolve the Promise this method creates.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export function twonce(target: EventTarget, resolveEvt1: string, resolveEvt2: string, ...rejectEvts: string[]): Promise<any>;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt1 - the name of the first event that will resolve the Promise this method creates.
 * @param resolveEvt2 - the name of the second event that will resolve the Promise this method creates.
 * @param timeout - the number of milliseconds to wait for the resolveEvt, before rejecting.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export function twonce(target: EventTarget, resolveEvt1: string, resolveEvt2: string, timeout: number, ...rejectEvts: string[]): Promise<any>;
export async function twonce(target: EventTarget, resolveEvt1: string, resolveEvt2: string, rejectEvtOrTimeout: number | string, ...rejectEvts: string[]) {

    if (isNullOrUndefined(rejectEvts)) {
        rejectEvts = [];
    }

    let timeout: number = undefined;
    if (typeof rejectEvtOrTimeout === "string") {
        rejectEvts.unshift(rejectEvtOrTimeout);
    }
    else if (typeof rejectEvtOrTimeout === "number") {
        timeout = rejectEvtOrTimeout;
    }

    if (target instanceof EventTarget) {
        [resolveEvt1, ...rejectEvts].forEach(
            targetValidateEvent.bind(null, target));
    }

    let timeoutHandle: TimeoutHandle = null;
    const listeners: [string, (value: unknown) => void][] = [];

    try {
        return await new Promise((resolve, reject) => {
            if (typeof timeout === "number") {
                timeoutHandle = setTimeout(reject, timeout, `'${resolveEvt1}' has timed out.`);
            }

            const register = (evtName: string, callback: (value: unknown) => void) => {
                target.addEventListener(evtName, callback);
                listeners.push([evtName, callback]);
            };

            register(resolveEvt1, resolve);
            register(resolveEvt2, resolve);

            for (const rejectEvt of rejectEvts) {
                register(rejectEvt, reject);
            }
        });
    } finally {
        if (isDefined(timeoutHandle)) {
            clearTimeout(timeoutHandle);
            timeoutHandle = null;
        }

        for (const [name, handler] of listeners) {
            target.removeEventListener(name, handler);
        }
    }
}

export function success(promise: Promise<unknown>): Promise<boolean> {
    return promise.then(alwaysTrue)
        .catch(alwaysFalse);
}

export function debounce(timeout: number, callback: (...args: any[]) => void): () => void;
export function debounce(callback: (...args: any[]) => void): () => void;
export function debounce(timeoutOrCallback: number | ((...args: any[]) => void), callback?: (...args: any[]) => void): () => void {
    let timeout = 0;
    if (isNumber(timeoutOrCallback)) {
        timeout = timeoutOrCallback;
    }
    else {
        callback = timeoutOrCallback;
    }

    let handle: number = null;

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


export function debounceRAF(callback: (time?: DOMHighResTimeStamp) => void) {
    let handle: number = null;
    return function () {
        if (isDefined(handle)) {
            cancelAnimationFrame(handle);
            handle = null;
        }

        handle = requestAnimationFrame((time: DOMHighResTimeStamp) => {
            handle = null;
            callback(time);
        });
    };
}