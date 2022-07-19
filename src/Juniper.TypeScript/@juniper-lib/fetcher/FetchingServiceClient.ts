import { IFetchingService, IRequest, IRequestWithBody, IResponse } from "@juniper-lib/fetcher";
import { assertNever, IProgress } from "@juniper-lib/tslib";
import { WorkerClient, WorkerServerEventMessage } from "@juniper-lib/workers";

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
        date } = response;

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

    protected propogateEvent(data: WorkerServerEventMessage<void>) {
        assertNever(data.eventName);
    }

    private makeRequest<T>(methodName: string, request: IRequest, progress: IProgress): Promise<T> {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }

    private makeRequestWithBody<T>(methodName: string, request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.makeRequest("sendNothingGetNothing", request, null);
    }        

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.makeRequest("sendNothingGetBuffer", request, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequest("sendNothingGetText", request, progress);
    }

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        return this.makeRequest("sendNothingGetObject", request, progress);
    }

    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequest("sendNothingGetFile", request, progress);
    }

    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.makeRequest("sendNothingGetImageBitmap", request, progress);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.makeRequestWithBody("sendObjectGetNothing", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.makeRequestWithBody("sendObjectGetBuffer", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequestWithBody("sendObjectGetText", request, progress);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.makeRequestWithBody("sendObjectGetObject", request, progress);
    }

    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequestWithBody("sendObjectGetFile", request, progress);
    }

    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.makeRequestWithBody("sendObjectGetImageBitmap", request, progress);
    }

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return this.callMethod("drawImageToCanvas", [cloneRequest(request), canvas], [canvas], progress);
    }

    async sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.sendNothingGetBuffer(request, progress);
        return bufferToBlob(response);
    }

    async sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.sendNothingGetBuffer(request, progress);
        return bufferToXml(response);
    }

    async sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.sendObjectGetBuffer(request, progress);
        return bufferToBlob(response);
    }

    async sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.sendObjectGetBuffer(request, progress);
        return bufferToXml(response);
    }
}
