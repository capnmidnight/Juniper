import { IResponse, WorkerServerEventMessage } from "@juniper-lib/util";
import { WorkerClient } from "@juniper-lib/dom";
import { TypedEventMap } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
import { IFetchingService } from "./IFetchingService";
import { IRequest, IRequestWithBody } from "./IRequest";
export declare class FetchingServiceClient extends WorkerClient implements IFetchingService {
    #private;
    setRequestVerificationToken(value: string): void;
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    protected propogateEvent(_data: WorkerServerEventMessage<TypedEventMap<string>>): void;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse>;
    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>>;
    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
}
//# sourceMappingURL=FetchingServiceClient.d.ts.map