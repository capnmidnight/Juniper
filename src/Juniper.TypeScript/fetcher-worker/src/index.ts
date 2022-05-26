import { FetchingServiceImplXHR as FetchingServiceImpl } from "@juniper-lib/fetcher-base/FetchingServiceImplXHR";
import { FetchingServiceServer } from "./FetchingServiceServer";
(globalThis as any).server = new FetchingServiceServer(
    (globalThis as any) as DedicatedWorkerGlobalScope,
    new FetchingServiceImpl());