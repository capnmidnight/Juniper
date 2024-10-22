import { WorkerClient } from "@juniper-lib/dom";
function isDOMParsersSupportedType(type) {
    return type === "application/xhtml+xml"
        || type === "application/xml"
        || type === "image/svg+xml"
        || type === "text/html"
        || type === "text/xml";
}
function bufferToXml(response) {
    const { status, requestPath, responsePath, content: buffer, contentType, contentLength, fileName, headers, date, errorMessage, errorObject } = response;
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
function bufferToBlob(response) {
    const { status, requestPath, responsePath, content: buffer, contentType, contentLength, fileName, headers, date, errorMessage, errorObject } = response;
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
function cloneRequest(request) {
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
function cloneRequestWithBody(request) {
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
export class FetchingServiceClient extends WorkerClient {
    setRequestVerificationToken(value) {
        this.callMethod("setRequestVerificationToken", [value]);
    }
    clearCache() {
        return this.callMethod("clearCache");
    }
    evict(path) {
        return this.callMethod("evict", [path]);
    }
    propogateEvent(_data) {
    }
    #makeRequest(methodName, request, progress) {
        return this.callMethod(methodName, [cloneRequest(request)], progress);
    }
    #makeRequestWithBody(methodName, request, progress) {
        return this.callMethod(methodName, [cloneRequestWithBody(request)], progress);
    }
    sendNothingGetNothing(request) {
        return this.#makeRequest("sendNothingGetNothing", request, null);
    }
    sendNothingGetBuffer(request, progress) {
        return this.#makeRequest("sendNothingGetBuffer", request, progress);
    }
    sendNothingGetText(request, progress) {
        return this.#makeRequest("sendNothingGetText", request, progress);
    }
    sendNothingGetObject(request, progress) {
        return this.#makeRequest("sendNothingGetObject", request, progress);
    }
    sendNothingGetFile(request, progress) {
        return this.#makeRequest("sendNothingGetFile", request, progress);
    }
    sendNothingGetImageBitmap(request, progress) {
        return this.#makeRequest("sendNothingGetImageBitmap", request, progress);
    }
    sendObjectGetNothing(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetNothing", request, progress);
    }
    sendObjectGetBuffer(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetBuffer", request, progress);
    }
    sendObjectGetText(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetText", request, progress);
    }
    sendObjectGetObject(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetObject", request, progress);
    }
    sendObjectGetFile(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetFile", request, progress);
    }
    sendObjectGetImageBitmap(request, progress) {
        return this.#makeRequestWithBody("sendObjectGetImageBitmap", request, progress);
    }
    drawImageToCanvas(request, canvas, progress) {
        return this.callMethod("drawImageToCanvas", [cloneRequest(request), canvas], [canvas], progress);
    }
    sendNothingGetBlob(request, progress) {
        return this.sendNothingGetBuffer(request, progress)
            .then(bufferToBlob);
    }
    sendNothingGetXml(request, progress) {
        return this.sendNothingGetBuffer(request, progress)
            .then(bufferToXml);
    }
    sendObjectGetBlob(request, progress) {
        return this.sendObjectGetBuffer(request, progress)
            .then(bufferToBlob);
    }
    sendObjectGetXml(request, progress) {
        return this.sendObjectGetBuffer(request, progress)
            .then(bufferToXml);
    }
}
//# sourceMappingURL=FetchingServiceClient.js.map