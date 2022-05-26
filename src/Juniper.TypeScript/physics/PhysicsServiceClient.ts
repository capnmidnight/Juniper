import type { IPhysicsService } from "@juniper-lib/physics-base/IPhysicsService";
import { WorkerClient } from "@juniper-lib/worker-client";


export class PhysicsServiceClient
    extends WorkerClient
    implements IPhysicsService {

    addBody(): Promise<number> {
        return this.callMethod("addBody");
    }

    removeBody(id: number): Promise<void> {
        return this.callMethod("removeBody", [id]);
    }
}
