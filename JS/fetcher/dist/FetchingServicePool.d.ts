import { IResponse } from "@juniper-lib/util";
import type { FullWorkerClientOptions } from "@juniper-lib/dom";
import { WorkerPool } from "@juniper-lib/dom";
import { TypedEventMap } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
import { FetchingServiceClient } from "./FetchingServiceClient";
import { IFetchingService } from "./IFetchingService";
import { IRequest, IRequestWithBody } from "./IRequest";
export declare class FetchingServicePool extends WorkerPool<TypedEventMap<string>, FetchingServiceClient> implements IFetchingService {
    #private;
    constructor(options: FullWorkerClientOptions, fetcher: IFetchingService);
    setRequestVerificationToken(value: string): void;
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse>;
    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>>;
    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}
//# sourceMappingURL=FetchingServicePool.d.ts.map