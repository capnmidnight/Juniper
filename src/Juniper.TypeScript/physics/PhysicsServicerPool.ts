import { IPhysicsService } from "@juniper-lib/physics-base/IPhysicsService";
import { isDefined, isNumber } from "@juniper-lib/tslib";
import type { FullWorkerClientOptions, WorkerClient, WorkerConstructorT } from "@juniper-lib/worker-client";
import { WorkerPool } from "@juniper-lib/worker-client";
import { PhysicsServiceClient } from "./PhysicsServiceClient";

export abstract class BasePhysicsServicePool<EventsT, FetcherWorkerClientT extends WorkerClient<EventsT> & IPhysicsService>
    extends WorkerPool<EventsT, FetcherWorkerClientT>
    implements IPhysicsService {

    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, FetcherWorkerClientT>) {
        if (isDefined(options)
            && isNumber(options.workers)
            && options.workers !== 1) {
            options.workers = 1;
        }
        super(options, WorkerClientClass);
    }

    addBody(): Promise<number> {
        return this.nextWorker().addBody();
    }

    removeBody(id: number): Promise<void> {
        return this.nextWorker().removeBody(id);
    }
}

export class PhysicsServicePool
    extends BasePhysicsServicePool<void, PhysicsServiceClient>{
    // no additional functionality
}