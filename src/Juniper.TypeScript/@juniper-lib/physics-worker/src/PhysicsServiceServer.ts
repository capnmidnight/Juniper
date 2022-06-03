import { IPhysicsService } from "@juniper-lib/physics-base/IPhysicsService";
import { PhysicsServiceImpl } from "@juniper-lib/physics-base/PhysicsServiceImpl";
import { WorkerServer } from "@juniper-lib/worker-server";

export class PhysicsServiceServer extends WorkerServer {
    constructor(self: DedicatedWorkerGlobalScope) {
        super(self);

        const physics = new PhysicsServiceImpl();
        addPhysicsMethods(this, physics);
    }
}

export function addPhysicsMethods(server: WorkerServer, physics: IPhysicsService) {
    server.addMethod(physics, "addBody", physics.addBody);
    server.addMethod(physics, "removeBody", physics.removeBody);
}
