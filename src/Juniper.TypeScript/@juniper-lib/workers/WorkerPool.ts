import { arrayClear } from "@juniper-lib/collections/arrays";
import { TypedEventBase } from "@juniper-lib/events/EventBase";
import { isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IDisposable, dispose } from "@juniper-lib/tslib/using";
import { WorkerClient } from "./WorkerClient";
import type { FullWorkerClientOptions } from "./WorkerClientOptions";

export type WorkerConstructorT<EventsT, WorkerClientT extends WorkerClient<EventsT>> = new (worker: Worker) => WorkerClientT;

export class WorkerPool<EventsT, WorkerClientT extends WorkerClient<EventsT>>
    extends TypedEventBase<EventsT>
    implements IDisposable {

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

        let workerPoolSize = -1;
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
        this.workers.forEach(dispose);
        arrayClear(this.workers);
    }

    protected nextWorker() {
        const worker = this.peekWorker();
        this.taskCounter++;
        return worker;
    }

    protected peekWorker() {
        // taskIDs help us keep track of return values.
        // The modulus selects them in a round-robin fashion.
        return this.workers[this.taskCounter % this.workers.length];
    }
}
