import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "juniper-fetcher-base";
import type { FullWorkerClientOptions, WorkerClient, WorkerConstructorT } from "juniper-tslib";
import { IProgress, WorkerPool } from "juniper-tslib";
import { FetchingServiceClient } from "./FetchingServiceClient";
export declare abstract class BaseFetchingServicePool<EventsT, FetcherWorkerClientT extends WorkerClient<EventsT> & IFetchingService> extends WorkerPool<EventsT, FetcherWorkerClientT> implements IFetchingService {
    private readonly fetcher;
    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, FetcherWorkerClientT>);
    private getFetcher;
    setRequestVerificationToken(value: string): void;
    head(request: IRequest): Promise<IResponse<void>>;
    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getObject<T>(request: IRequest, progress: IProgress): Promise<T>;
    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>>;
    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T>;
    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}
export declare class FetchingServicePool extends BaseFetchingServicePool<void, FetchingServiceClient> {
}
