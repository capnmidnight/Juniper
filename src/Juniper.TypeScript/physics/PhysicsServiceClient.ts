import type { IPhysicsService } from "juniper-physics-base/IPhysicsService";
import { WorkerClient } from "juniper-worker-client";


export class PhysicsServiceClient
    extends WorkerClient<void>
    implements IPhysicsService {

    addBody(): Promise<number> {
        return this.callMethod("addBody");
    }
}