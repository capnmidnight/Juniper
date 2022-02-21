import { PhysicsServiceServer } from "./PhysicsServiceServer";
(globalThis as any).server = new PhysicsServiceServer((globalThis as any) as DedicatedWorkerGlobalScope);