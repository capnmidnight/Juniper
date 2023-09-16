/// <reference types="offscreencanvas" />
import { WorkerServer } from "@juniper-lib/workers/dist/WorkerServer";
import { IFetchingService } from "./IFetchingService";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
import { TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
export declare class FetchingServiceServer extends WorkerServer<TypedEventMap<string>> {
    constructor(self: DedicatedWorkerGlobalScope, impl: IFetchingServiceImpl);
}
export declare function addFetcherMethods(server: WorkerServer<TypedEventMap<string>>, fetcher: IFetchingService): void;
//# sourceMappingURL=FetchingServiceServer.d.ts.map