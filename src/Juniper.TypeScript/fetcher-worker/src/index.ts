import { FetchingServiceServer } from "./FetchingServiceServer";
(globalThis as any).server = new FetchingServiceServer((globalThis as any) as DedicatedWorkerGlobalScope);