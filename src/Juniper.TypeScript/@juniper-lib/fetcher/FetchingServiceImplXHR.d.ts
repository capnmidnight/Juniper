import { IProgress } from "@juniper-lib/progress/IProgress";
import type { IFetchingServiceImpl, XMLHttpRequestResponseTypeMap } from "./IFetchingServiceImpl";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IResponse } from "./IResponse";
export declare function isXHRBodyInit(obj: any): obj is XMLHttpRequestBodyInit;
export declare class FetchingServiceImplXHR implements IFetchingServiceImpl {
    private readonly cacheReady;
    private cache;
    private store;
    constructor();
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
    private openCache;
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    private readResponseHeaders;
    private readResponse;
    private decodeContent;
    private readonly tasks;
    private withCachedTask;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendSomethingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>>;
}
//# sourceMappingURL=FetchingServiceImplXHR.d.ts.map