import type { IResponse } from "@juniper-lib/util";
import { IProgress } from "@juniper-lib/progress";
import type { IFetchingServiceImpl, XMLHttpRequestResponseTypeMap } from "./IFetchingServiceImpl";
import type { IRequest, IRequestWithBody } from "./IRequest";
export declare function isXHRBodyInit(obj: any): obj is XMLHttpRequestBodyInit;
export declare class FetchingServiceImpl implements IFetchingServiceImpl {
    #private;
    constructor();
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendSomethingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>>;
}
//# sourceMappingURL=FetchingServiceImpl.d.ts.map