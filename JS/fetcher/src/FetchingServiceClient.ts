import { IResponse, WorkerServerEventMessage } from "@juniper-lib/util";
import { WorkerClient } from "@juniper-lib/dom";
import { TypedEventMap } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
import { IFetchingService } from "./IFetchingService";
import { IRequest, IRequestWithBody } from "./IRequest";

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
        requestPath,
        responsePath,
        content: buffer,
        contentType,
        contentLength,
        fileName,
        headers,
        date,
        errorMessage,
        errorObject
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
        requestPath,
        responsePath,
        content: doc.documentElement,
        contentType,
        contentLength,
        fileName,
        date,
        headers,
        errorMessage,
        errorObject
    };
}

function bufferToBlob(response: IResponse<ArrayBuffer>): IResponse<Blob> {
    const {
        status,
        requestPath,
        responsePath,
        content: buffer,
        contentType,
        contentLength,
        fileName,
        headers,
        date,
        errorMessage,
        errorObject
    } = response;

    const blob = new Blob([buffer], {
        type: contentType
    });

    return {
        status,
        requestPath,
        responsePath,
        content: blob,
        contentType,
        contentLength,
        fileName,
        date,
        headers,
        errorMessage,
        errorObject
    };
}

function cloneRequest(request: IRequest): IRequest {
    request = {
        method: request.method,
        path: request.path,
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials,
        useCache: request.useCache,
        retryCount: request.retryCount
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
        useCache: request.useCache,
        retryCount: request.retryCount
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

    protected propogateEvent(_data: WorkerServerEventMessage<TypedEventMap<string>>) {
        
    }

    #makeRequest<T>(methodName: string, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }

    #makeRequestWithBody<T>(methodName: string, request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.#makeRequest("sendNothingGetNothing", request, null);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.#makeRequest("sendNothingGetBuffer", request, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.#makeRequest("sendNothingGetText", request, progress);
    }

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.#makeRequest("sendNothingGetObject", request, progress);
    }

    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.#makeRequest("sendNothingGetFile", request, progress);
    }

    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.#makeRequest("sendNothingGetImageBitmap", request, progress);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.#makeRequestWithBody("sendObjectGetNothing", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.#makeRequestWithBody("sendObjectGetBuffer", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.#makeRequestWithBody("sendObjectGetText", request, progress);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.#makeRequestWithBody("sendObjectGetObject", request, progress);
    }

    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.#makeRequestWithBody("sendObjectGetFile", request, progress);
    }

    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.#makeRequestWithBody("sendObjectGetImageBitmap", request, progress);
    }

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return this.callMethod("drawImageToCanvas", [cloneRequest(request), canvas], [canvas], progress);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.sendNothingGetBuffer(request, progress)
            .then(bufferToBlob);
    }

    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.sendNothingGetBuffer(request, progress)
            .then(bufferToXml);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.sendObjectGetBuffer(request, progress)
            .then(bufferToBlob);
    }

    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.sendObjectGetBuffer(request, progress)
            .then(bufferToXml);
    }
}
