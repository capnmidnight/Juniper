import { IDexDB, IDexStore } from "@juniper-lib/indexdb";
import { assertNever, identity, IProgress, isArrayBuffer, isArrayBufferView, isDefined, isNullOrUndefined, isString, mapJoin, PriorityList, PriorityMap, progressSplit, Task, using } from "@juniper-lib/tslib";
import type { HTTPMethods } from "./HTTPMethods";
import type { IFetchingServiceImpl, XMLHttpRequestResponseTypeMap } from "./IFetchingServiceImpl";
import type { IRequest, IRequestWithBody } from "./IRequest";
import type { IResponse } from "./IResponse";
import { translateResponse } from "./translateResponse";

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

    async drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        const response = await this.sendNothingGetSomething("blob", request, progress);
        const blob = response.content;
        return using(await createImageBitmap(blob, {
            imageOrientation: "none"
        }), (img) => {
            canvas.width = img.width;
            canvas.height = img.height;
            const g = canvas.getContext("2d");
            g.drawImage(img, 0, 0);
            return translateResponse(response, () => null);
        });
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

    async clearCache(): Promise<void> {
        await this.cacheReady;
        await this.store.clear();
    }

    private async readResponseHeaders(path: string, xhr: XMLHttpRequest): Promise<IResponse> {
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

        const headers = new Map<string, string>(normalizedHeaderParts);
        const contentType = readResponseHeader(headers, "content-type", identity);
        const contentLength = readResponseHeader(headers, "content-length", parseFloat);
        const date = readResponseHeader(headers, "date", (v) => new Date(v));
        const fileName = readResponseHeader(headers, "content-disposition", (v) => {
            if (isDefined(v)) {
                const match = v.match(FILE_NAME_PATTERN);
                if (isDefined(match)) {
                    return match[1];
                }
            }

            return null;
        });

        const response: IResponse = {
            status: xhr.status,
            path,
            content: undefined,
            contentType,
            contentLength,
            fileName,
            date,
            headers
        };

        return response;
    }

    private async readResponse(path: string, xhr: XMLHttpRequest): Promise<IResponse<Blob>> {
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

        return response;
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

    private readonly tasks = new PriorityMap<HTTPMethods, string, Promise<any>>();

    private async withCachedTask<T>(request: IRequest, action: () => Promise<IResponse<T>>): Promise<IResponse<T>> {
        if (request.method !== "GET"
            && request.method !== "HEAD"
            && request.method !== "OPTIONS") {
            return await action();
        }

        if (!this.tasks.has(request.method, request.path)) {
            this.tasks.add(
                request.method,
                request.path,
                action().finally(() =>
                    this.tasks.delete(request.method, request.path)));
        }

        return this.tasks.get(request.method, request.path);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.withCachedTask(request, async () => {
            const xhr = new XMLHttpRequest();
            const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, null, true);

            sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

            await download;

            return await this.readResponseHeaders(request.path, xhr);
        });
    }

    sendNothingGetSomething<K extends keyof (XMLHttpRequestResponseTypeMap), T extends XMLHttpRequestResponseTypeMap[K]>(xhrType: K, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.withCachedTask(request, async () => {
            let response: IResponse<Blob> = null;

            const useCache = request.useCache && request.method === "GET";

            if (useCache) {
                if (isDefined(progress)) {
                    progress.start();
                }
                await this.cacheReady;
                response = await this.store.get(request.path);
            }

            const noCachedResponse = isNullOrUndefined(response);

            if (noCachedResponse) {
                const xhr = new XMLHttpRequest();
                const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);

                sendRequest(xhr, request.method, request.path, request.timeout, request.headers);

                await download;

                response = await this.readResponse(request.path, xhr);

                if (useCache) {
                    await this.store.add(response);
                }
            }

            const value = await this.decodeContent<K, T>(xhrType, response);

            if (noCachedResponse && isDefined(progress)) {
                progress.end();
            }

            return value;
        });
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

        const response = await this.readResponse(request.path, xhr);
        return await this.decodeContent(xhrType, response);
    }
}
