import { assertNever, identity, IDexDB, IDexStore, IProgress, isArrayBuffer, isArrayBufferView, isDefined, isNullOrUndefined, isString, mapJoin, PriorityList, progressSplit, Task } from "@juniper/tslib";
import { HTTPMethods } from "./HTTPMethods";
import { IFetchingServiceImpl, XMLHttpRequestResponseTypeMap } from "./IFetchingServiceImpl";
import { IRequest, IRequestWithBody } from "./IRequest";
import { IBodilessResponse, IResponse } from "./IResponse";
import { translateResponse } from "./ResponseTranslator";

function isXHRBodyInit(obj: any): obj is XMLHttpRequestBodyInit {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || isArrayBuffer(obj)
        || obj instanceof ReadableStream
        || "Document" in globalThis && obj instanceof Document;
}

function trackProgress(name: string, xhr: XMLHttpRequest, target: (XMLHttpRequest | XMLHttpRequestUpload), prog: IProgress, skipLoading: boolean, prevTask?: Promise<void>): Promise<void> {

    let prevDone = !prevTask;
    if (prevTask) {
        prevTask.then(() => prevDone = true);
    }

    let done = false;
    let loaded = skipLoading;

    const requestComplete = new Task(
        () => loaded && done,
        () => prevDone);

    target.addEventListener("loadstart", () => {
        if (prevDone && !done && prog) {
            prog.start(name);
        }
    });

    target.addEventListener("progress", (ev: Event) => {
        if (prevDone && !done) {
            const evt = ev as ProgressEvent<XMLHttpRequestEventTarget>;
            if (prog) {
                prog.report(evt.loaded, Math.max(evt.loaded, evt.total), name);
            }
            if (evt.loaded === evt.total) {
                loaded = true;
                requestComplete.resolve();
            }
        }
    });

    target.addEventListener("load", () => {
        if (prevDone && !done) {
            if (prog) {
                prog.end(name);
            }
            done = true;
            requestComplete.resolve();
        }
    });

    const onError = (msg: string) => () => requestComplete.reject(`${msg} (${xhr.status})`);

    target.addEventListener("error", onError("error"));
    target.addEventListener("abort", onError("abort"));
    target.addEventListener("timeout", onError("timeout"));

    return requestComplete;
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
const DB_NAME = "Juniper:Fetcher:Cache";

export class FetchingServiceImplXHR implements IFetchingServiceImpl {

    private readonly cacheReady: Promise<void>;
    private cache: IDexDB = null;
    private store: IDexStore<IResponse<Blob>> = null;

    constructor() {
        this.cacheReady = this.openCache();
    }

    private async openCache(): Promise<void> {
        this.cache = await IDexDB.open(DB_NAME, {
            name: "files",
            options: {
                keyPath: "path"
            }
        });

        this.store = await this.cache.getStore("files");
    }

    private async readResponseHeaders(path: string, xhr: XMLHttpRequest): Promise<IBodilessResponse> {
        const headerParts = xhr
            .getAllResponseHeaders()
            .split(/[\r\n]+/)
            .map((v) => v.trim())
            .filter((v) => v.length > 0)
            .map<[string, string]>((line) => {
                const parts = line.split(": ");
                const key = parts.shift().toLowerCase();
                const value = parts.join(": ");
                return [key, value];
            });

        const pList = new PriorityList<string, string>(headerParts);
        const normalizedHeaderParts = Array.from(pList.keys())
            .map<[string, string]>((key) =>
                [
                    key,
                    pList.get(key)
                        .join(", ")
                ]);

        let headers = new Map<string, string>(normalizedHeaderParts);

        let contentType = readResponseHeader(headers, "content-type", identity);
        let contentLength = readResponseHeader(headers, "content-length", parseFloat);
        let date = readResponseHeader(headers, "date", (v) => new Date(v));
        let fileName = readResponseHeader(headers, "content-disposition", (v) => {
            if (isDefined(v)) {
                const match = v.match(FILE_NAME_PATTERN);
                if (isDefined(match)) {
                    return match[1];
                }
            }

            return null;
        });

        const response: IBodilessResponse = {
            status: xhr.status,
            path,
            contentType,
            contentLength,
            fileName,
            date,
            headers
        };

        if (useCache) {
            await this.store.add(response);
        }

        return response;
    }

    private async readResponse<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(path: string, useCache: boolean, xhrType: K, xhr: XMLHttpRequest): Promise<IResponse<T>> {
        const {
            status,
            contentType,
            contentLength,
            fileName,
            date,
            headers
        } = await this.readResponseHeaders(path, xhr);

        const response: IResponse<Blob> = {
            path,
            status,
            contentType,
            contentLength,
            fileName,
            date,
            headers,
            content: xhr.response as Blob
        }

        if (isDefined(response.content)) {
            response.contentType = response.contentType || response.content.type;
            response.contentLength = response.contentLength || response.content.size;
        }

        if (useCache) {
            await this.store.put(response);
        }

        return await this.decodeContent(xhrType, response);
    }

    private async decodeContent<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, response: IResponse<Blob>): Promise<IResponse<T>> {
        return translateResponse<Blob, T>(response, async (contentBlob) => {
            if (xhrType === "") {
                return null;
            }
            else if (isNullOrUndefined(response.contentType)) {
                const headerBlock = Array.from(response.headers.entries())
                    .map((kv) => kv.join(": "))
                    .join("\n  ");
                throw new Error("No content type found in headers: \n  " + headerBlock);
            }
            else if (xhrType === "blob") {
                return contentBlob as any as T;
            }
            else if (xhrType === "arraybuffer") {
                return (await contentBlob.arrayBuffer()) as any as T;
            }
            else if (xhrType === "json") {
                const text = await contentBlob.text();
                if (text.length > 0) {
                    return JSON.parse(text) as T;
                }
                else {
                    return null;
                }
            }
            else if (xhrType === "document") {
                const parser = new DOMParser();
                if (response.contentType === "application/xhtml+xml"
                    || response.contentType === "text/html"
                    || response.contentType === "application/xml"
                    || response.contentType === "image/svg+xml"
                    || response.contentType === "text/xml") {
                    return parser.parseFromString(await contentBlob.text(), response.contentType) as any as T;
                }
                else {
                    throw new Error("Couldn't parse document");
                }
            }
            else if (xhrType === "text") {
                return (await contentBlob.text()) as any as T;
            }
            else {
                assertNever(xhrType);
            }
        });
    }

        if (request.useCache) {
            await this.cacheReady;
            const result = await this.store.get(request.path);
            if (isDefined(result)) {
                return this.decodeContent("", result);
            }
        }

    async sendNothingGetNothing(request: IRequest): Promise<IBodilessResponse> {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, null, true);

        sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

        await download;

        return await this.readResponseHeaders<void>(request.path, request.useCache, xhr);
    }

    async sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        if (request.useCache) {
            await this.cacheReady;
            const result = await this.store.get(request.path);
            if (isDefined(result)) {
                return this.decodeContent(xhrType, result);
            }
        }

        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);

        sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

        await download;

        return await this.readResponse(request.path, request.useCache, xhrType, xhr);
    }

    async sendSomethingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequestWithBody, defaultPostHeaders: Map<string, string>, progress: IProgress): Promise<IResponse<T>> {

        let body: XMLHttpRequestBodyInit = null;


        const headers = mapJoin(new Map<string, string>(), defaultPostHeaders, request.headers);

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

        return await this.readResponse(request.path, false, xhrType, xhr);
    }
}
