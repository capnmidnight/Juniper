import { IProgress } from "@juniper-lib/progress/IProgress";
import { IFetchingService } from "./IFetchingService";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
import { IRequest, IRequestWithBody } from "./IRequest";
import { IResponse } from "./IResponse";
export declare class FetchingService implements IFetchingService {
    private readonly impl;
    constructor(impl: IFetchingServiceImpl);
    protected readonly defaultPostHeaders: Map<string, string>;
    setRequestVerificationToken(value: string): void;
    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;
    sendNothingGetNothing(request: IRequest): Promise<IResponse>;
    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>>;
    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse>;
    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}
//# sourceMappingURL=FetchingService.d.ts.map