import { TypedEventBase } from "../events/EventBase";
import type { IProgress } from "../progress/IProgress";
import type { IDisposable } from "../using";
import { WorkerClient } from "./WorkerClient";
import type { FullWorkerClientOptions } from "./WorkerClientOptions";
export declare type WorkerConstructorT<EventsT, WorkerClientT extends WorkerClient<EventsT>> = new (worker: Worker) => WorkerClientT;
export declare class WorkerPool<EventsT, WorkerClientT extends WorkerClient<EventsT>> extends TypedEventBase<EventsT> implements IDisposable {
    static isSupported: boolean;
    private scriptPath;
    private taskCounter;
    protected readonly workers: WorkerClientT[];
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, WorkerClientT>);
    dispose(): void;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    protected callMethod<T>(methodName: string, onProgress?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    protected callMethod<T>(methodName: string, params: any[], onProgress?: IProgress): Promise<T>;
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    protected callMethod<T>(methodName: string, params: any[], transferables: Transferable[], onProgress?: IProgress): Promise<T>;
    protected nextWorker(): WorkerClientT;
}
