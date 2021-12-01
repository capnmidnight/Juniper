import { WorkerServer } from "juniper-tslib-worker";
import type { IFetchingService } from "./IFetcher";
export declare class FetchingServiceServer extends WorkerServer {
    constructor(self: DedicatedWorkerGlobalScope);
}
export declare function addFetcherMethods(server: WorkerServer, fetcher: IFetchingService): void;
