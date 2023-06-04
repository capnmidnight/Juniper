import type { IProgress } from "@juniper-lib/progress/IProgress";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IResponse } from "./IResponse";


export interface IFetchingService {

    clearCache(): Promise<void>;
    evict(path: string): Promise<void>;

    setRequestVerificationToken(value: string): void;

    sendNothingGetNothing(request: IRequest): Promise<IResponse>;

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>>;
    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse>;

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>>;
    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse>;
}
