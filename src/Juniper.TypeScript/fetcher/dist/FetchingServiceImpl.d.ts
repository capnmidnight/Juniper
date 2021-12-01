import type { IProgress } from "juniper-tslib";
import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "./IFetcher";
import { ResponseTranslator } from "./ResponseTranslator";
export declare class FetchingServiceImpl extends ResponseTranslator implements IFetchingService {
    private readonly defaultPostHeaders;
    setRequestVerificationToken(value: string): void;
    private headOrGetXHR;
    private getXHR;
    head(request: IRequest): Promise<IResponse<void>>;
    private postXHR;
    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>>;
    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>>;
    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>>;
    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    getObject<T>(request: IRequest, progress: IProgress): Promise<T>;
    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T>;
    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>>;
    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>>;
    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>>;
    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>>;
    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>>;
    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>>;
    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>>;
}
