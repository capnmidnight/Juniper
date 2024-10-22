/// <reference types="offscreencanvas" />
import { TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
import { WorkerServer } from "@juniper-lib/workers";
import { IFetchingService } from "./IFetchingService";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
export declare class FetchingServiceServer extends WorkerServer<TypedEventMap<string>> {
    constructor(self: DedicatedWorkerGlobalScope, impl: IFetchingServiceImpl);
}
export declare function addFetcherMethods(server: WorkerServer<TypedEventMap<string>>, fetcher: IFetchingService): void;
//# sourceMappingURL=FetchingServiceServer.d.ts.map