import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { WorkerClient } from "@juniper-lib/workers/WorkerClient";
import { WorkerServerEventMessage } from "@juniper-lib/workers/WorkerMessages";
import { IFetchingService } from "./IFetchingService";
import { IRequest, IRequestWithBody } from "./IRequest";
import { InternalResponse, IResponse } from "./IResponse";

function isDOMParsersSupportedType(type: string): type is DOMParserSupportedType {
    return type === "application/xhtml+xml"
        || type === "application/xml"
        || type === "image/svg+xml"
        || type === "text/html"
        || type === "text/xml";
}

function bufferToXml(response: IResponse<ArrayBuffer>): IResponse<HTMLElement> {
    const {
        status,
        path,
        content: buffer,
        contentType,
        contentLength,
        fileName,
        headers,
        date
    } = response;

    if (!isDOMParsersSupportedType(contentType)) {
        throw new Error(`Content-Type ${contentType} is not one supported by the DOM parser.`);
    }

    const decoder = new TextDecoder();
    const text = decoder.decode(buffer);
    const parser = new DOMParser();
    const doc = parser.parseFromString(text, contentType);

    return {
        status,
        path,
        content: doc.documentElement,
        contentType,
        contentLength,
        fileName,
        date,
        headers
    };
}

function bufferToBlob(response: IResponse<ArrayBuffer>): IResponse<Blob> {
    const {
        status,
        path,
        content: buffer,
        contentType,
        contentLength,
        fileName,
        headers,
        date
    } = response;

    const blob = new Blob([buffer], {
        type: contentType
    });

    return {
        status,
        path,
        content: blob,
        contentType,
        contentLength,
        fileName,
        date,
        headers
    };
}

function cloneRequest(request: IRequest): IRequest {
    request = {
        method: request.method,
        path: request.path,
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials,
        useCache: request.useCache
    };
    return request;
}

function cloneRequestWithBody(request: IRequestWithBody): IRequestWithBody {
    request = {
        method: request.method,
        path: request.path,
        body: request.body,
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials,
        useCache: request.useCache
    };
    return request;
}


export class FetchingServiceClient
    extends WorkerClient
    implements IFetchingService {

    setRequestVerificationToken(value: string): void {
        this.callMethod("setRequestVerificationToken", [value]);
    }

    clearCache(): Promise<void> {
        return this.callMethod("clearCache");
    }

    evict(path: string): Promise<void> {
        return this.callMethod("evict", [path]);
    }

    protected propogateEvent(data: WorkerServerEventMessage<void>) {
        assertNever(data.eventName);
    }

    private makeRequest<T>(methodName: string, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }

    private makeRequestWithBody<T>(methodName: string, request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }

    async sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return new InternalResponse(await this.makeRequest("sendNothingGetNothing", request, null));
    }        

    async sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return new InternalResponse(await this.makeRequest("sendNothingGetBuffer", request, progress));
    }

    async sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return new InternalResponse(await this.makeRequest("sendNothingGetText", request, progress));
    }

    async sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return new InternalResponse(await this.makeRequest("sendNothingGetObject", request, progress));
    }

    async sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return new InternalResponse(await this.makeRequest("sendNothingGetFile", request, progress));
    }

    async sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return new InternalResponse(await this.makeRequest("sendNothingGetImageBitmap", request, progress));
    }

    async sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetNothing", request, progress));
    }

    async sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetBuffer", request, progress));
    }

    async sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetText", request, progress));
    }

    async sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetObject", request, progress));
    }

    async sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetFile", request, progress));
    }

    async sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return new InternalResponse(await this.makeRequestWithBody("sendObjectGetImageBitmap", request, progress));
    }

    async drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return new InternalResponse(await this.callMethod("drawImageToCanvas", [cloneRequest(request), canvas], [canvas], progress));
    }

    async sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.sendNothingGetBuffer(request, progress);
        return new InternalResponse(await bufferToBlob(response));
    }

    async sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.sendNothingGetBuffer(request, progress);
        return new InternalResponse(await bufferToXml(response));
    }

    async sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.sendObjectGetBuffer(request, progress);
        return new InternalResponse(await bufferToBlob(response));
    }

    async sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.sendObjectGetBuffer(request, progress);
        return new InternalResponse(await bufferToXml(response));
    }
}
