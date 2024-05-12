import { alwaysFalse, alwaysTrue, eventHandler, hasValue } from "./filters";

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

    if (!hasValue(rejectEvts)) {
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
        if (hasValue(timeoutHandle)) {
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

    if (!hasValue(rejectEvts)) {
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
        if (hasValue(timeoutHandle)) {
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

/**
 * Make sure the call this as `this.#onname = replaceEventHandler.call(this, "name", this.#onname, handler);`
 * from the source EventTarget.
 * @param name
 * @param oldHandler the handler to replace
 * @param newHandler the handler to add
 */
export function replaceEventHandler<T extends Event = Event>(this: EventTarget, name: string, oldHandler: eventHandler<T>, newHandler: eventHandler<T>) {
    if (oldHandler) this.removeEventListener(name, oldHandler as eventHandler<Event>);
    if (newHandler) this.addEventListener(name, newHandler as eventHandler<Event>, true);
    return newHandler;
}

export type EventReplacer = <T extends Event = Event>(this: EventTarget, name: string, oldHandler: eventHandler<T>, newHandler: eventHandler<T>) => eventHandler<T>;

export class Confirmation extends EventTarget {
    #onconfirm: (evt: Event) => any = null;
    get onconfirm() { return this.#onconfirm; }
    set onconfirm(v) { this.#onconfirm = replaceEventHandler.call(this, "confirm", this.#onconfirm, v); }
    #confirmEvt = new Event("confirm");
    confirm = () => this.dispatchEvent(this.#confirmEvt);

    #oncancel: (evt: Event) => any = null;
    get oncancel() { return this.#oncancel; }
    set oncancel(v) { this.#oncancel = replaceEventHandler.call(this, "cancel", this.#oncancel, v); }
    #cancelEvt = new Event("cancel");
    cancel = () => this.dispatchEvent(this.#cancelEvt);

    confirmed() {
        return success(once(this, "confirm", "cancel"));
    }

    cancelled() {
        return success(once(this, "cancel", "confirm"));
    }
}

export function debounce(callback: (...args: any[]) => void) {
    let handle: number = null;
    return function () {
        if (hasValue(handle)) {
            clearTimeout(handle);
            handle = null;
        }

        handle = setTimeout(() => {
            handle = null;
            callback(...arguments);
        }, 0, ...arguments);
    };
}


export function debounceRAF(callback: (time?: DOMHighResTimeStamp) => void) {
    let handle: number = null;
    return function () {
        if (hasValue(handle)) {
            cancelAnimationFrame(handle);
            handle = null;
        }

        handle = requestAnimationFrame((time: DOMHighResTimeStamp) => {
            handle = null;
            callback(time);
        });
    };
}