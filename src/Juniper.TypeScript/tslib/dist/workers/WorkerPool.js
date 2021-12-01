import { arrayClear } from "../collections/arrayClear";
import { TypedEventBase } from "../events/EventBase";
import { isProgressCallback } from "../progress/IProgress";
import { isArray, isDefined, isNullOrUndefined, isNumber } from "../typeChecks";
import { WorkerClient } from "./WorkerClient";
export class WorkerPool extends TypedEventBase {
    static isSupported = "Worker" in globalThis;
    scriptPath;
    taskCounter;
    workers;
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(options, WorkerClientClass) {
        super();
        this.scriptPath = options.scriptPath;
        let workerPoolSize = -1;
        const workersDef = options.workers;
        let workers = null;
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
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    callMethod(methodName, params, transferables, onProgress) {
        if (!WorkerClient.isSupported) {
            return Promise.reject(new Error("Workers are not supported on this system."));
        }
        // Normalize method parameters.
        let parameters = null;
        let tfers = null;
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
        return worker.callMethod(methodName, parameters, tfers, onProgress);
    }
    nextWorker() {
        // taskIDs help us keep track of return values.
        // The modulus selects them in a round-robin fashion.
        const taskID = this.taskCounter++;
        const workerID = taskID % this.workers.length;
        return this.workers[workerID];
    }
}
