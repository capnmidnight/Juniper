import { Application_X_Url } from "juniper-mediatypes/application";
import { parseInternetShortcut } from "juniper-mediatypes/internetShortcut";
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

const FILE_NAME_PATTERN = /filename=\"(.+)\"(;|$)/;

export class FetchingServiceImpl
    extends ResponseTranslator
    implements IFetchingService {

    private readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    private async readResponse<T>(method: HTTPMethods, xhrType: XMLHttpRequestResponseType, request: IRequest, xhr: XMLHttpRequest, progress: IProgress, depth: number): Promise<IResponse<T>> {
        if (xhr.status >= 400) {
            throw new Error(`Error [${xhr.status}]: ${xhr.responseText}.`);
        }

        let content: T = null;
        const contentBlob = xhr.response as Blob;

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

        let contentType = readResponseHeader(headers, "content-type", identity)
            || contentBlob && contentBlob.type
            || null;
        let contentLength = readResponseHeader(headers, "content-length", parseFloat)
            || contentBlob && contentBlob.size
            || null;
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

        if (xhrType !== "") {
            if (isNullOrUndefined(contentType)) {
                const headerBlock = normalizedHeaderParts
                    .map(kv => kv.join(": "))
                    .join("\n  ");
                throw new Error("No content type found in headers: \n  " + headerBlock);
            }
            else if (Application_X_Url.matches(contentType)) {
                if (depth > 0) {
                    throw new Error("Too many redirects");
                }
                else {
                    if (method === "POST") {
                        method = "GET";
                    }
                    const shortcutText = await contentBlob.text();

                    const newPath = this.parseInternetShortcut(shortcutText);
                    request.path = newPath;
                    if (isDefined(request.headers)) {
                        request.headers.delete("accept");
                    }

                    const redirectedResponse = await this.headOrGetXHR<T>(method, xhrType, request, progress, depth + 1);
                    content = redirectedResponse.content;
                    headers = redirectedResponse.headers;
                    contentType = redirectedResponse.contentType;
                    contentLength = redirectedResponse.contentLength;
                    date = redirectedResponse.date;
                    fileName = redirectedResponse.fileName;
                }
            }
            else if (xhrType === "blob") {
                content = contentBlob as any as T;
            }
            else if (xhrType === "arraybuffer") {
                content = await contentBlob.arrayBuffer() as any as T;
            }
            else if (xhrType === "json") {
                content = JSON.parse(await contentBlob.text()) as T;
            }
            else if (xhrType === "document") {
                const parser = new DOMParser();
                if (contentType === "application/xhtml+xml"
                    || contentType === "text/html"
                    || contentType === "application/xml"
                    || contentType === "image/svg+xml"
                    || contentType === "text/xml") {
                    content = parser.parseFromString(await contentBlob.text(), contentType) as any as T;
                }
            }
            else if (xhrType === "text") {
                content = (await contentBlob.text()) as any as T;
            }
            else {
                assertNever(xhrType);
            }
        }

        const response: IResponse<T> = {
            status: xhr.status,
            content,
            contentType,
            contentLength,
            fileName,
            date,
            headers
        };

        return response;
    }

    protected parseInternetShortcut(path: string): string {
        return parseInternetShortcut(path);
    }

    private async headOrGetXHR<T>(method: HTTPMethods, xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress, depth?: number): Promise<IResponse<T>> {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);

        sendRequest(xhr, method, request.path, request.timeout, request.headers);

        await download;
        return await this.readResponse(method, xhrType, request, xhr, progress, depth || 0);
    }

    private getXHR<T>(xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.headOrGetXHR("GET", xhrType, request, progress);
    }

    private async postXHR<T>(xhrType: XMLHttpRequestResponseType, request: IRequestWithBody, progress: IProgress, depth?: number): Promise<IResponse<T>> {

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

        sendRequest(xhr, "POST", request.path, request.timeout, headers, body);

        await upload;
        await download;

        return await this.readResponse("POST", xhrType, request, xhr, downloadProg, depth || 0);
    }

    head(request: IRequest): Promise<IResponse<void>> {
        return this.headOrGetXHR("HEAD", "", request, null);
    }

    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.getXHR<Blob>("blob", request, progress);
    }

    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.postXHR<Blob>("blob", request, progress);
    }

    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.getXHR<ArrayBuffer>("arraybuffer", request, progress);
    }

    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.postXHR<ArrayBuffer>("arraybuffer", request, progress);
    }

    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.getXHR<string>("text", request, progress);
    }

    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.postXHR<string>("text", request, progress);
    }

    async getObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        const response = await this.getXHR<T>("json", request, progress);
        return response.content;
    }

    async postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        const response = await this.postXHR<T>("json", request, progress);
        return response.content;
    }

    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>> {
        return this.postXHR<void>("", request, progress);
    }

    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.translateResponse(
            this.getBlob(request, progress),
            URL.createObjectURL);
    }

    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.translateResponse(
            this.postObjectForBlob(request, progress),
            URL.createObjectURL);
    }

    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.translateResponse(
            this.getXHR<Document>("document", request, progress),
            doc => doc.documentElement);
    }

    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.translateResponse(
            this.postXHR<Document>("document", request, progress),
            doc => doc.documentElement);
    }

    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.translateResponse(
            this.getBlob(request, progress),
            createImageBitmap)
    }

    async postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.translateResponse(
            this.postObjectForBlob(request, progress),
            createImageBitmap);
    }
}
