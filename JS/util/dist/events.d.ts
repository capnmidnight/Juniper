export type TimeoutHandle = ReturnType<typeof setTimeout>;
/**
 * Check to see if a an `on###` type event property exists in the given object.
 */
export declare function targetValidateEvent(target: EventTarget, type: string): void;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export declare function once(target: EventTarget, resolveEvt: string, ...rejectEvts: string[]): Promise<any>;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt - the name of the event that will resolve the Promise this method creates.
 * @param timeout - the number of milliseconds to wait for the resolveEvt, before rejecting.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export declare function once(target: EventTarget, resolveEvt: string, timeout: number, ...rejectEvts: string[]): Promise<any>;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt1 - the name of the first event that will resolve the Promise this method creates.
 * @param resolveEvt2 - the name of the second event that will resolve the Promise this method creates.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export declare function twonce(target: EventTarget, resolveEvt1: string, resolveEvt2: string, ...rejectEvts: string[]): Promise<any>;
/**
 * Wait for a specific event, one time.
 * @param target - the event target.
 * @param resolveEvt1 - the name of the first event that will resolve the Promise this method creates.
 * @param resolveEvt2 - the name of the second event that will resolve the Promise this method creates.
 * @param timeout - the number of milliseconds to wait for the resolveEvt, before rejecting.
 * @param rejectEvts - names of the events that could reject the Promise this method creates.
 */
export declare function twonce(target: EventTarget, resolveEvt1: string, resolveEvt2: string, timeout: number, ...rejectEvts: string[]): Promise<any>;
export declare function success(promise: Promise<unknown>): Promise<boolean>;
export declare function debounce(timeout: number, callback: (...args: any[]) => void): () => void;
export declare function debounce(callback: (...args: any[]) => void): () => void;
export declare function debounceRAF(callback: (time?: DOMHighResTimeStamp) => void): () => void;
//# sourceMappingURL=events.d.ts.map