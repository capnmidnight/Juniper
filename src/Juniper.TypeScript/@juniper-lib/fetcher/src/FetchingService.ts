import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { IFetchingService } from "./IFetchingService";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
import { IRequest, IRequestWithBody } from "./IRequest";
import { IResponse } from "./IResponse";
import { translateResponse } from "./translateResponse";


export class FetchingService implements IFetchingService {

    constructor(private readonly impl: IFetchingServiceImpl) {
    }

    protected readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    clearCache(): Promise<void> {
        return this.impl.clearCache();
    }

    evict(path: string): Promise<void> {
        return this.impl.evict(path);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.impl.sendNothingGetNothing(request);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.impl.sendNothingGetSomething("blob", request, progress);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.impl.sendSomethingGetSomething("blob", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.impl.sendNothingGetSomething("arraybuffer", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.impl.sendSomethingGetSomething("arraybuffer", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.impl.sendNothingGetSomething("text", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.impl.sendSomethingGetSomething("text", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.impl.sendNothingGetSomething<"json", T>("json", request, progress);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.impl.sendSomethingGetSomething<"json", T>("json", request, this.defaultPostHeaders, progress);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.impl.sendSomethingGetSomething("", request, this.defaultPostHeaders, progress);
    }

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return this.impl.drawImageToCanvas(request, canvas, progress);
    }

    async sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return translateResponse(
            await this.sendNothingGetBlob(request, progress),
            URL.createObjectURL);
    }

    async sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return translateResponse(
            await this.sendObjectGetBlob(request, progress),
            URL.createObjectURL);
    }

    async sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return translateResponse(
            await this.impl.sendNothingGetSomething("document", request, progress),
            (doc) => doc.documentElement);
    }

    async sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return translateResponse(
            await this.impl.sendSomethingGetSomething("document", request, this.defaultPostHeaders, progress),
            (doc) => doc.documentElement);
    }

    async sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendNothingGetBlob(request, progress),
            createImageBitmap);
    }

    async sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendObjectGetBlob(request, progress),
            createImageBitmap);
    }
}
