import { IProgress } from "juniper-tslib";
import { WorkerClient } from "juniper-tslib-browser";
import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "./IFetcher";
export declare class FetchingServiceClient extends WorkerClient<void> implements IFetchingService {
    setRequestVerificationToken(value: string): void;
    private makeRequest;
    private makeRequestWithBody;
    head(request: IRequest): Promise<IResponse<void>>;
    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getObject<T>(request: IRequest, progress: IProgress): Promise<T>;
    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>>;
    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T>;
    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
}
