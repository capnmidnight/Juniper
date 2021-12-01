import { TypedEventBase } from "../events/EventBase";
import type { IProgress } from "../progress/IProgress";
import type { IDisposable } from "../using";
export declare class WorkerClient<EventsT> extends TypedEventBase<EventsT> implements IDisposable {
    private worker;
    static isSupported: boolean;
    private taskCounter;
    private invocations;
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(worker: Worker);
    private postMessage;
    dispose(): void;
    private propogateEvent;
    private progressReport;
    private methodReturned;
    private invocationError;
    /**
     * When the invocation has errored, we want to stop listening to the worker
     * message channel so we don't eat up processing messages that have no chance
     * ever pertaining to the invocation.
     **/
    private removeInvocation;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, onProgress?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], onProgress?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], transferables: Transferable[], onProgress?: IProgress): Promise<T>;
}
