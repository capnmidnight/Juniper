import type { IDisposable, WorkerServerEventMessage } from "@juniper-lib/util";
import { TypedEventMap, TypedEventTarget } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
export declare abstract class WorkerClient<EventsMapT extends TypedEventMap<string> = TypedEventMap<string>> extends TypedEventTarget<EventsMapT> implements IDisposable {
    #private;
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(worker: Worker);
    dispose(): void;
    protected abstract propogateEvent(data: WorkerServerEventMessage<EventsMapT>): void;
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