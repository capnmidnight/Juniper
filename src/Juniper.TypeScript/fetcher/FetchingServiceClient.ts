import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "juniper-fetcher-base/IFetcher";
import { IProgress, WorkerClient } from "juniper-tslib";

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
        content: buffer,
        contentType,
        contentLength,
        fileName,
        headers,
        date } = response;

    const blob = new Blob([buffer], {
        type: contentType
    });

    return {
        status,
        content: blob,
        contentType,
        contentLength,
        fileName,
        date,
        headers
    };
}

function resolvePath(path: string) {
    return new URL(path, document.location.href).href;
}

function cloneRequest(request: IRequest): IRequest {
    request = {
        path: resolvePath(request.path),
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials
    };
    return request;
}

function cloneRequestWithBody(request: IRequestWithBody): IRequestWithBody {
    request = {
        path: resolvePath(request.path),
        body: request.body,
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials
    };
    return request;
}


export class FetchingServiceClient
    extends WorkerClient<void>
    implements IFetchingService {

    setRequestVerificationToken(value: string): void {
        this.callMethod("setRequestVerificationToken", [value]);
    }

    private makeRequest<T>(methodName: string, request: IRequest, progress: IProgress): Promise<T> {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }

    private makeRequestWithBody<T>(methodName: string, request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }

    head(request: IRequest): Promise<IResponse<void>> {
        return this.makeRequest("head", request, null);
    }        

    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.makeRequest("getBuffer", request, progress);
    }

    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequest("getText", request, progress);
    }

    getObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        return this.makeRequest("getObject", request, progress);
    }

    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequest("getFile", request, progress);
    }

    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.makeRequest("getImageBitmap", request, progress);
    }

    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>> {
        return this.makeRequestWithBody("postObject", request, progress);
    }

    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.makeRequestWithBody("postObjectForBuffer", request, progress);
    }

    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequestWithBody("postObjectForText", request, progress);
    }

    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.makeRequestWithBody("postObjectForObject", request, progress);
    }

    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.makeRequestWithBody("postObjectForFile", request, progress);
    }

    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.makeRequestWithBody("postObjectForImageBitmap", request, progress);
    }

    async getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.getBuffer(request, progress);
        return bufferToBlob(response);
    }

    async getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.getBuffer(request, progress);
        return bufferToXml(response);
    }

    async postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        const response = await this.postObjectForBuffer(request, progress);
        return bufferToBlob(response);
    }

    async postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        const response = await this.postObjectForBuffer(request, progress);
        return bufferToXml(response);
    }
}
