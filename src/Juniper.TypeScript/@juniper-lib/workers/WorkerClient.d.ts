import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/TypedEventTarget";
import { IProgress } from "@juniper-lib/progress/IProgress";
import type { IDisposable } from "@juniper-lib/tslib/using";
import type { WorkerServerEventMessage } from "@juniper-lib/workers/WorkerMessages";
export declare abstract class WorkerClient<EventsMapT extends TypedEventMap<string> = TypedEventMap<string>> extends TypedEventTarget<EventsMapT> implements IDisposable {
    private worker;
    private readonly invocations;
    private readonly tasks;
    private taskCounter;
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(worker: Worker);
    private postMessage;
    dispose(): void;
    protected abstract propogateEvent(data: WorkerServerEventMessage<EventsMapT>): void;
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
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, prog?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], prog?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], transferables: (Transferable | OffscreenCanvas)[], prog?: IProgress): Promise<T>;
}
//# sourceMappingURL=WorkerClient.d.ts.map