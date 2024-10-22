import { TypedEventMap } from "@juniper-lib/events";
import { IFetchingService, IFetchingServiceImpl } from "@juniper-lib/fetcher";
import { WorkerServer } from "@juniper-lib/workers";
export declare class FetchingServiceServer extends WorkerServer<TypedEventMap<string>> {
    constructor(self: DedicatedWorkerGlobalScope, impl: IFetchingServiceImpl);
}
export declare function addFetcherMethods(server: WorkerServer<TypedEventMap<string>>, fetcher: IFetchingService): void;
//# sourceMappingURL=FetchingServiceServer.d.ts.map