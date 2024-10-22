import { IDisposable } from "@juniper-lib/util";
import { TypedEventMap, TypedEventTarget } from "@juniper-lib/events";
import { WorkerClient } from "./WorkerClient";
import type { FullWorkerClientOptions } from "./WorkerClientOptions";
export type WorkerConstructorT<EventsT extends TypedEventMap<string>, WorkerClientT extends WorkerClient<EventsT>> = new (worker: Worker) => WorkerClientT;
export declare class WorkerPool<EventMapT extends TypedEventMap<string>, WorkerClientT extends WorkerClient<EventMapT>> extends TypedEventTarget<EventMapT> implements IDisposable {
    #private;
    protected readonly workers: WorkerClientT[];
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventMapT, WorkerClientT>);
    dispose(): void;
    protected nextWorker(): WorkerClientT;
    protected peekWorker(): WorkerClientT;
}
//# sourceMappingURL=WorkerPool.d.ts.map