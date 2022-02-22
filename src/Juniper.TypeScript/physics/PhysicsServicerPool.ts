import { IPhysicsService } from "juniper-physics-base/IPhysicsService";
import { PhysicsServiceImpl } from "juniper-physics-base/PhysicsServiceImpl";
import type { FullWorkerClientOptions, WorkerClient, WorkerConstructorT } from "juniper-worker-client";
import { WorkerPool } from "juniper-worker-client";
import { PhysicsServiceClient } from "./PhysicsServiceClient";

export abstract class BasePhysicsServicePool<EventsT, FetcherWorkerClientT extends WorkerClient<EventsT> & IPhysicsService>
    extends WorkerPool<EventsT, FetcherWorkerClientT>
    implements IPhysicsService {

    private readonly physics = new PhysicsServiceImpl();

    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, FetcherWorkerClientT>) {
        super(options, WorkerClientClass);
    }

    addBody(): Promise<number> {
        return this.physics.addBody();
    }

    removeBody(id: number): Promise<void> {
        return this.physics.removeBody(id);
    }
}

export class PhysicsServicePool
    extends BasePhysicsServicePool<void, PhysicsServiceClient>{
    // no additional functionality
}