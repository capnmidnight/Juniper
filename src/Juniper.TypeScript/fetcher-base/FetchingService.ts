import { IProgress } from "@juniper/tslib";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
import { IRequest, IRequestWithBody } from "./IRequest";
import { IResponse } from "./IResponse";
import { translateResponse } from "./ResponseTranslator";


export class FetchingService {

    constructor(private readonly impl: IFetchingServiceImpl) {
    }

    protected readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse<void>> {
        return this.impl.sendNothingGetNothing(request);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.impl.sendNothingGetSomething<Blob>("blob", request, progress);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.impl.sendSomethingGetSomething<Blob>("blob", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.impl.sendNothingGetSomething<ArrayBuffer>("arraybuffer", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.impl.sendSomethingGetSomething<ArrayBuffer>("arraybuffer", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.impl.sendNothingGetSomething<string>("text", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.impl.sendSomethingGetSomething<string>("text", request, this.defaultPostHeaders, progress);
    }

    async sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        const response = await this.impl.sendNothingGetSomething<T>("json", request, progress);
        return response.content;
    }

    async sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        const response = await this.impl.sendSomethingGetSomething<T>("json", request, this.defaultPostHeaders, progress);
        return response.content;
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>> {
        return this.impl.sendSomethingGetSomething<void>("", request, this.defaultPostHeaders, progress);
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
            await this.impl.sendNothingGetSomething<Document>("document", request, progress),
            (doc) => doc.documentElement);
    }

    async sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return translateResponse(
            await this.impl.sendSomethingGetSomething<Document>("document", request, this.defaultPostHeaders, progress),
            (doc) => doc.documentElement);
    }

    async sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendNothingGetBlob(request, progress),
            createImageBitmap)
    }

    async sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendObjectGetBlob(request, progress),
            createImageBitmap);
    }
}
