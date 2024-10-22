import type { IFetcher } from "./IFetcher";
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
export declare function createFetcher(workerPath?: string): IFetcher;
//# sourceMappingURL=index.d.ts.map