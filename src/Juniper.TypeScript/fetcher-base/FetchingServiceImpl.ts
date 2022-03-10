import { assertNever, identity, IProgress, isArrayBuffer, isArrayBufferView, isDefined, isNullOrUndefined, isString, mapJoin, PriorityList, progressSplit } from "juniper-tslib";
import type { HTTPMethods, IFetchingService, IRequest, IRequestWithBody, IResponse } from "./IFetcher";
import { ResponseTranslator } from "./ResponseTranslator";

export type ProxyResolvingCallback = (path: string) => string;

export function isXHRBodyInit(obj: any): obj is XMLHttpRequestBodyInit {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || isArrayBuffer(obj)
        || obj instanceof ReadableStream
        || "Document" in globalThis && obj instanceof Document;
}

function trackProgress(name: string, xhr: XMLHttpRequest, target: (XMLHttpRequest | XMLHttpRequestUpload), onProgress: IProgress, skipLoading: boolean, prevTask?: Promise<void>): Promise<void> {
    return new Promise((resolve: () => void, reject: (msg: string) => void) => {
        let prevDone = !prevTask;
        if (prevTask) {
            prevTask.then(() => prevDone = true);
        }

        let done = false;
        let loaded = skipLoading;
        function maybeResolve() {
            if (loaded && done) {
                resolve();
            }
        }

        function onError(msg: string) {
            return () => {
                if (prevDone) {
                    reject(`${msg} (${xhr.status})`);
                }
            }
        }

        target.addEventListener("loadstart", () => {
            if (prevDone && !done && onProgress) {
                onProgress.report(0, 1, name);
            }
        });

        target.addEventListener("progress", (ev: Event) => {
            if (prevDone && !done) {
                const evt = ev as ProgressEvent<XMLHttpRequestEventTarget>;
                if (onProgress) {
                    onProgress.report(evt.loaded, Math.max(evt.loaded, evt.total), name);
                }
                if (evt.loaded === evt.total) {
                    loaded = true;
                    maybeResolve();
                }
            }
        });

        target.addEventListener("load", () => {
            if (prevDone && !done) {
                if (onProgress) {
                    onProgress.report(1, 1, name);
                }
                done = true;
                maybeResolve();
            }
        });

        target.addEventListener("error", onError("error"));
        target.addEventListener("abort", onError("abort"));
        target.addEventListener("timeout", onError("timeout"));
    });
}

function sendRequest(xhr: XMLHttpRequest, method: HTTPMethods, path: string, timeout: number, headers: Map<string, string>, body?: XMLHttpRequestBodyInit): void {
    xhr.open(method, path);
    xhr.responseType = "blob";
    xhr.timeout = timeout;
    if (headers) {
        for (const [key, value] of headers) {
            xhr.setRequestHeader(key, value);
        }
    }

    if (isDefined(body)) {
        xhr.send(body);
    }
    else {
        xhr.send();
    }
}

function readResponseHeader<T>(headers: Map<string, string>, key: string, translate: (value: string) => T): T {
    if (!headers.has(key)) {
        return null;
    }

    const value = headers.get(key);
    try {
        const translated = translate(value);
        headers.delete(key);
        return translated;
    }
    catch (exp) {
        console.warn(key, exp);
    }
    return null;
}


function readResponseHeaders<T>(xhr: XMLHttpRequest): IResponse<T> {
    const headerParts = xhr
        .getAllResponseHeaders()
        .split(/[\r\n]+/)
        .map(v => v.trim())
        .filter(v => v.length > 0)
        .map<[string, string]>(line => {
            const parts = line.split(": ");
            const key = parts.shift().toLowerCase();
            const value = parts.join(": ");
            return [key, value];
        });

    const pList = new PriorityList<string, string>();
    for (const [key, value] of headerParts) {
        pList.add(key, value);
    }

    const normalizedHeaderParts = Array.from(pList.keys())
        .map<[string, string]>(key =>
            [
                key,
                pList.get(key)
                    .join(", ")
            ]);

    let headers = new Map<string, string>(normalizedHeaderParts);

    let contentType = readResponseHeader(headers, "content-type", identity);
    let contentLength = readResponseHeader(headers, "content-length", parseFloat);
    let date = readResponseHeader(headers, "date", v => new Date(v));
    let fileName = readResponseHeader(headers, "content-disposition", v => {
        if (isDefined(v)) {
            const match = v.match(FILE_NAME_PATTERN);
            if (isDefined(match)) {
                return match[1];
            }
        }

        return null;
    });

    const response: IResponse<T> = {
        status: xhr.status,
        content: null,
        contentType,
        contentLength,
        fileName,
        date,
        headers
    };

    return response;
}

async function readResponse<T>(xhrType: XMLHttpRequestResponseType, xhr: XMLHttpRequest): Promise<IResponse<T>> {
    const response = readResponseHeaders<T>(xhr);
    const contentBlob = xhr.response as Blob;

    if (isDefined(contentBlob)) {
        response.contentType = response.contentType || contentBlob.type;
        response.contentLength = response.contentLength || contentBlob.size;
    }

    if (xhrType !== "") {
        if (isNullOrUndefined(response.contentType)) {
            const headerBlock = Array.from(response.headers.entries())
                .map(kv => kv.join(": "))
                .join("\n  ");
            throw new Error("No content type found in headers: \n  " + headerBlock);
        }
        else if (xhrType === "blob") {
            response.content = contentBlob as any as T;
        }
        else if (xhrType === "arraybuffer") {
            response.content = await contentBlob.arrayBuffer() as any as T;
        }
        else if (xhrType === "json") {
            const text = await contentBlob.text();
            if (text.length > 0) {
                response.content = JSON.parse(text) as T;
            }
        }
        else if (xhrType === "document") {
            const parser = new DOMParser();
            if (response.contentType === "application/xhtml+xml"
                || response.contentType === "text/html"
                || response.contentType === "application/xml"
                || response.contentType === "image/svg+xml"
                || response.contentType === "text/xml") {
                response.content = parser.parseFromString(await contentBlob.text(), response.contentType) as any as T;
            }
        }
        else if (xhrType === "text") {
            response.content = (await contentBlob.text()) as any as T;
        }
        else {
            assertNever(xhrType);
        }
    }

    return response;
}

const FILE_NAME_PATTERN = /filename=\"(.+)\"(;|$)/;

export class FetchingServiceImpl
    extends ResponseTranslator
    implements IFetchingService {

    private readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    async sendNothingGetNothing(request: IRequest): Promise<IResponse<void>> {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, null, true);

        sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

        await download;

        return readResponseHeaders(xhr);
    }

    private async sendNothingGetSomething<T>(xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);

        sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

        await download;

        return await readResponse(xhrType, xhr);
    }

    private async sendSomethingGetSomething<T>(xhrType: XMLHttpRequestResponseType, request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {

        let body: XMLHttpRequestBodyInit = null;


        const headers = mapJoin(new Map<string, string>(), this.defaultPostHeaders, request.headers);

        if (request.body instanceof FormData
            && isDefined(headers)) {
            const toDelete = new Array<string>();
            for (const key of headers.keys()) {
                if (key.toLowerCase() === "content-type") {
                    toDelete.push(key);
                }
            }
            for (const key of toDelete) {
                headers.delete(key);
            }
        }

        if (isXHRBodyInit(request.body) && !isString(request.body)) {
            body = request.body;
        }
        else if (isDefined(request.body)) {
            body = JSON.stringify(request.body);
        }

        const progs = progressSplit(progress, 2);
        const xhr = new XMLHttpRequest();
        const upload = isDefined(body)
            ? trackProgress("uploading", xhr, xhr.upload, progs.shift(), false)
            : Promise.resolve();
        const downloadProg = progs.shift();
        const download = trackProgress("saving", xhr, xhr, downloadProg, true, upload);

        sendRequest(xhr, request.method, request.path, request.timeout, headers, body);

        await upload;
        await download;

        return await readResponse(xhrType, xhr);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.sendNothingGetSomething<Blob>("blob", request, progress);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.sendSomethingGetSomething<Blob>("blob", request, progress);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.sendNothingGetSomething<ArrayBuffer>("arraybuffer", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.sendSomethingGetSomething<ArrayBuffer>("arraybuffer", request, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.sendNothingGetSomething<string>("text", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.sendSomethingGetSomething<string>("text", request, progress);
    }

    async sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        const response = await this.sendNothingGetSomething<T>("json", request, progress);
        return response.content;
    }

    async sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        const response = await this.sendSomethingGetSomething<T>("json", request, progress);
        return response.content;
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>> {
        return this.sendSomethingGetSomething<void>("", request, progress);
    }

    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.translateResponse(
            this.sendNothingGetBlob(request, progress),
            URL.createObjectURL);
    }

    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.translateResponse(
            this.sendObjectGetBlob(request, progress),
            URL.createObjectURL);
    }

    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.translateResponse(
            this.sendNothingGetSomething<Document>("document", request, progress),
            doc => doc.documentElement);
    }

    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.translateResponse(
            this.sendSomethingGetSomething<Document>("document", request, progress),
            doc => doc.documentElement);
    }

    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.translateResponse(
            this.sendNothingGetBlob(request, progress),
            createImageBitmap)
    }

    async sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.translateResponse(
            this.sendObjectGetBlob(request, progress),
            createImageBitmap);
    }
}
