import { WorkerClient } from "juniper-tslib";
function isDOMParsersSupportedType(type) {
    return type === "application/xhtml+xml"
        || type === "application/xml"
        || type === "image/svg+xml"
        || type === "text/html"
        || type === "text/xml";
}
function bufferToXml(response) {
    const { status, content: buffer, contentType, contentLength, fileName, headers, date } = response;
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
function bufferToBlob(response) {
    const { status, content: buffer, contentType, contentLength, fileName, headers, date } = response;
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
function resolvePath(path) {
    return new URL(path, document.location.href).href;
}
function cloneRequest(request) {
    request = {
        path: resolvePath(request.path),
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials
    };
    return request;
}
function cloneRequestWithBody(request) {
    request = {
        path: resolvePath(request.path),
        body: request.body,
        timeout: request.timeout,
        headers: request.headers,
        withCredentials: request.withCredentials
    };
    return request;
}
export class FetchingServiceClient extends WorkerClient {
    setRequestVerificationToken(value) {
        this.callMethod("setRequestVerificationToken", [value]);
    }
    makeRequest(methodName, request, progress) {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }
    makeRequestWithBody(methodName, request, progress) {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }
    head(request) {
        return this.makeRequest("head", request, null);
    }
    getBuffer(request, progress) {
        return this.makeRequest("getBuffer", request, progress);
    }
    getText(request, progress) {
        return this.makeRequest("getText", request, progress);
    }
    getObject(request, progress) {
        return this.makeRequest("getObject", request, progress);
    }
    getFile(request, progress) {
        return this.makeRequest("getFile", request, progress);
    }
    getImageBitmap(request, progress) {
        return this.makeRequest("getImageBitmap", request, progress);
    }
    postObject(request, progress) {
        return this.makeRequestWithBody("postObject", request, progress);
    }
    postObjectForBuffer(request, progress) {
        return this.makeRequestWithBody("postObjectForBuffer", request, progress);
    }
    postObjectForText(request, progress) {
        return this.makeRequestWithBody("postObjectForText", request, progress);
    }
    postObjectForObject(request, progress) {
        return this.makeRequestWithBody("postObjectForObject", request, progress);
    }
    postObjectForFile(request, progress) {
        return this.makeRequestWithBody("postObjectForFile", request, progress);
    }
    postObjectForImageBitmap(request, progress) {
        return this.makeRequestWithBody("postObjectForImageBitmap", request, progress);
    }
    async getBlob(request, progress) {
        const response = await this.getBuffer(request, progress);
        return bufferToBlob(response);
    }
    async getXml(request, progress) {
        const response = await this.getBuffer(request, progress);
        return bufferToXml(response);
    }
    async postObjectForBlob(request, progress) {
        const response = await this.postObjectForBuffer(request, progress);
        return bufferToBlob(response);
    }
    async postObjectForXml(request, progress) {
        const response = await this.postObjectForBuffer(request, progress);
        return bufferToXml(response);
    }
}
