import { arrayClear } from "../collections/arrayClear";
import { TypedEventBase } from "../events/EventBase";
import type { IProgress } from "../progress/IProgress";
import { isProgressCallback } from "../progress/IProgress";
import { isArray, isDefined, isNullOrUndefined, isNumber } from "../typeChecks";
import type { IDisposable } from "../using";
import { WorkerClient } from "./WorkerClient";
import type { FullWorkerClientOptions } from "./WorkerClientOptions";

export type WorkerConstructorT<EventsT, WorkerClientT extends WorkerClient<EventsT>> = new (worker: Worker) => WorkerClientT;

export class WorkerPool<EventsT, WorkerClientT extends WorkerClient<EventsT>>
    extends TypedEventBase<EventsT>
    implements IDisposable {
    static isSupported = "Worker" in globalThis;

    private scriptPath: string;
    private taskCounter: number;
    protected readonly workers: WorkerClientT[];

    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, WorkerClientT>) {
        super();

        this.scriptPath = options.scriptPath;

        let workerPoolSize: number = -1;
        const workersDef = options.workers;
        let workers: Array<Worker> = null;
        if (isNumber(workersDef)) {
            workerPoolSize = workersDef;
        }
        else if (isDefined(workersDef)) {
            this.taskCounter = workersDef.curTaskCounter;
            workers = workersDef.workers;
            workerPoolSize = workers.length;
        }
        else {
            workerPoolSize = navigator.hardwareConcurrency || 4;
        }

        // Validate parameters
        if (workerPoolSize < 1) {
            throw new Error("Worker pool size must be a postive integer greater than 0");
        }

        this.workers = new Array(workerPoolSize);

        if (isNullOrUndefined(workers)) {
            this.taskCounter = 0;
            for (let i = 0; i < workerPoolSize; ++i) {
                this.workers[i] = new WorkerClientClass(new Worker(this.scriptPath, { type: "module" }));
            }
        }
        else {
            for (let i = 0; i < workerPoolSize; ++i) {
                this.workers[i] = new WorkerClientClass(workers[i]);
            }
        }

        for (const worker of this.workers) {
            worker.addBubbler(this);
        }
    }

    dispose() {
        for (const worker of this.workers) {
            worker.dispose();
        }
        arrayClear(this.workers);
    }

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

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    protected callMethod<T>(methodName: string, params?: any[] | IProgress, transferables?: Transferable[] | IProgress, onProgress?: IProgress): Promise<T | undefined> {
        if (!WorkerClient.isSupported) {
            return Promise.reject(new Error("Workers are not supported on this system."));
        }

        // Normalize method parameters.
        let parameters: any[] = null;
        let tfers: Transferable[] = null;

        if (isProgressCallback(params)) {
            onProgress = params;
            params = null;
            transferables = null;
        }

        if (isProgressCallback(transferables)
            && !onProgress) {
            onProgress = transferables;
            transferables = null;
        }

        if (isArray(params)) {
            parameters = params;
        }

        if (isArray(transferables)) {
            tfers = transferables;
        }

        const worker = this.nextWorker();

        return worker.callMethod<T>(methodName, parameters, tfers, onProgress);
    }

    protected nextWorker() {
        // taskIDs help us keep track of return values.
        // The modulus selects them in a round-robin fashion.
        const taskID = this.taskCounter++;
        const workerID = taskID % this.workers.length;
        return this.workers[workerID];
    }
}
