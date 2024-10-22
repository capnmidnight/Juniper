import { DEBUG, isDefined } from "@juniper-lib/util";
import { Fetcher } from "./Fetcher";
import { FetchingService } from "./FetchingService";
import { FetchingServiceImpl } from "./FetchingServiceImpl";
import { FetchingServicePool } from "./FetchingServicePool";
import type { IFetcher } from "./IFetcher";
import type { IFetchingService } from "./IFetchingService";

export * from "./Asset";
export * from "./Fetcher";
export * from "./FetchingService";
export * from "./FetchingServiceClient";
export * from "./FetchingServiceImpl";
export * from "./FetchingServicePool";
export * from "./HTTPMethods";
export * from "./IDictionary";
export * from "./IFetcher";
export * from "./IFetchingService";
export * from "./IFetchingServiceImpl";
export * from "./IRequest";
export * from "./RequestBuilder";
export * from "./assertSuccess";
export * from "./translateResponse";
export * from "./unwrapResponse";

/**
 * Create's an IFetcher instance that optionally uses a 
 * service worker to make the requests and perform some
 * of the basic response translation.
 * @param workerPath (Optional) a path to the service worker script
 */
export function createFetcher(workerPath?: string): IFetcher {

    let fallback: IFetchingService = new FetchingService(new FetchingServiceImpl());

    if (isDefined(workerPath)) {
        fallback = new FetchingServicePool({
            scriptPath: workerPath
        }, fallback);
    }

    return new Fetcher(fallback, !DEBUG);
}