import { assertNever, identity, isArrayBuffer, isArrayBufferView, isDefined, isNullOrUndefined, isString, mapJoin, using } from "@juniper-lib/util";
import { PriorityList, PriorityMap } from "@juniper-lib/collections";
import { Task, withRetry } from "@juniper-lib/events";
import { IDex } from "@juniper-lib/indexdb";
import { Text_Plain } from "@juniper-lib/mediatypes";
import { progressSplit } from "@juniper-lib/progress";
import { translateResponse } from "./translateResponse";
export function isXHRBodyInit(obj) {
    return isString(obj)
        || isArrayBufferView(obj)
        || obj instanceof Blob
        || obj instanceof FormData
        || isArrayBuffer(obj)
        || "Document" in globalThis && obj instanceof Document;
}
function trackProgress(name, xhr, target, prog, skipLoading, prevTask) {
    let prevDone = !prevTask;
    if (prevTask) {
        prevTask.then(() => prevDone = true);
    }
    let done = false;
    let loaded = skipLoading;
    const requestComplete = new Task();
    target.addEventListener("loadstart", () => {
        if (prevDone && !done && prog) {
            prog.start(name);
        }
    });
    target.addEventListener("progress", (ev) => {
        if (prevDone && !done) {
            const evt = ev;
            if (prog) {
                prog.report(evt.loaded, Math.max(evt.loaded, evt.total), name);
            }
            if (evt.loaded === evt.total) {
                loaded = true;
                if (done) {
                    requestComplete.resolve();
                }
            }
        }
    });
    target.addEventListener("load", () => {
        if (prevDone && !done) {
            if (prog) {
                prog.end(name);
            }
            done = true;
            if (loaded) {
                requestComplete.resolve();
            }
        }
    });
    const onError = (msg) => () => {
        if (prevDone) {
            requestComplete.reject(`${msg} (${xhr.status})`);
        }
    };
    target.addEventListener("error", onError("error"));
    target.addEventListener("abort", onError("abort"));
    target.addEventListener("timeout", onError("timeout"));
    return requestComplete;
}
function sendRequest(xhr, method, path, timeout, headers, body) {
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
function readResponseHeader(headers, key, translate) {
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
export class FetchingServiceImpl {
    #cacheReady;
    #factory = null;
    #cache = null;
    #store = null;
    constructor() {
        this.#factory = new IDex(DB_NAME);
        this.#cacheReady = this.#openCache();
    }
    async drawImageToCanvas(request, canvas, progress) {
        const response = await this.sendNothingGetSomething("blob", request, progress);
        const blob = response.content;
        return using(await createImageBitmap(blob, {
            imageOrientation: "from-image"
        }), (img) => {
            canvas.width = img.width;
            canvas.height = img.height;
            const g = canvas.getContext("2d");
            g.drawImage(img, 0, 0);
            return translateResponse(response);
        });
    }
    async #openCache() {
        const options = {
            keyPath: "requestPath"
        };
        await this.#factory.assert({
            name: "files",
            options
        });
        this.#cache = await this.#factory.open();
        this.#store = await this.#cache.getStore("files");
    }
    async clearCache() {
        await this.#cacheReady;
        await this.#store.clear();
    }
    async evict(path) {
        await this.#cacheReady;
        if (this.#store.has(path)) {
            await this.#store.delete(path);
        }
    }
    async #readResponseHeaders(requestPath, xhr) {
        const headerParts = xhr
            .getAllResponseHeaders()
            .split(/[\r\n]+/)
            .map((v) => v.trim())
            .filter((v) => v.length > 0)
            .map((line) => {
            const parts = line.split(": ");
            const key = parts.shift().toLowerCase();
            const value = parts.join(": ");
            return [key, value];
        });
        const pList = new PriorityList(headerParts);
        const normalizedHeaderParts = Array.from(pList.keys())
            .map((key) => [
            key,
            pList.get(key)
                .join(", ")
        ]);
        const headers = new Map(normalizedHeaderParts);
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
        const response = {
            status: xhr.status,
            requestPath,
            responsePath: xhr.responseURL,
            content: undefined,
            contentType,
            contentLength,
            fileName,
            date,
            headers,
            errorMessage: null,
            errorObject: null
        };
        if (xhr.status >= 400) {
            const blob = xhr.response;
            const errorMessage = await blob.text();
            try {
                response.errorObject = JSON.parse(errorMessage);
            }
            catch { }
            response.errorMessage = `${xhr.statusText}: ${errorMessage}`;
        }
        return response;
    }
    async #readResponse(requestPath, xhr) {
        const { responsePath, status, contentType, contentLength, fileName, date, headers, errorMessage, errorObject } = await this.#readResponseHeaders(requestPath, xhr);
        const response = {
            requestPath,
            responsePath,
            status,
            contentType,
            contentLength,
            fileName,
            date,
            headers,
            content: xhr.response,
            errorMessage,
            errorObject
        };
        if (isDefined(response.content)) {
            response.contentType = response.contentType || response.content.type;
            response.contentLength = response.contentLength || response.content.size;
        }
        return response;
    }
    async #decodeContent(xhrType, response) {
        return translateResponse(response, async (contentBlob) => {
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
                return contentBlob;
            }
            else if (xhrType === "arraybuffer") {
                return (await contentBlob.arrayBuffer());
            }
            else if (xhrType === "json") {
                const text = await contentBlob.text();
                if (text.length > 0) {
                    return JSON.parse(text);
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
                    return parser.parseFromString(await contentBlob.text(), response.contentType);
                }
                else {
                    throw new Error("Couldn't parse document");
                }
            }
            else if (xhrType === "text") {
                return (await contentBlob.text());
            }
            else {
                assertNever(xhrType);
            }
        });
    }
    #tasks = new PriorityMap();
    async #withCachedTask(request, action) {
        if (request.method !== "GET"
            && request.method !== "HEAD"
            && request.method !== "OPTIONS") {
            return await action();
        }
        if (!this.#tasks.has(request.method, request.path)) {
            this.#tasks.add(request.method, request.path, action().finally(() => this.#tasks.delete(request.method, request.path)));
        }
        return this.#tasks.get(request.method, request.path);
    }
    sendNothingGetNothing(request) {
        return this.#withCachedTask(request, withRetry(request.retryCount, async () => {
            const xhr = new XMLHttpRequest();
            const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, null, true);
            sendRequest(xhr, request.method, request.path, request.timeout, request.headers);
            await download;
            return await this.#readResponseHeaders(request.path, xhr);
        }));
    }
    sendNothingGetSomething(xhrType, request, progress) {
        return this.#withCachedTask(request, withRetry(request.retryCount, async () => {
            let response = null;
            const useCache = request.useCache && request.method === "GET";
            if (useCache) {
                if (isDefined(progress)) {
                    progress.start();
                }
                await this.#cacheReady;
                response = await this.#store.get(request.path);
            }
            const noCachedResponse = isNullOrUndefined(response);
            if (noCachedResponse) {
                const xhr = new XMLHttpRequest();
                const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);
                sendRequest(xhr, request.method, request.path, request.timeout, request.headers);
                await download;
                response = await this.#readResponse(request.path, xhr);
                if (useCache) {
                    await this.#store.add(response);
                }
            }
            const value = await this.#decodeContent(xhrType, response);
            if (noCachedResponse && isDefined(progress)) {
                progress.end();
            }
            return value;
        }));
    }
    sendSomethingGetSomething(xhrType, request, defaultPostHeaders, progress) {
        let body = null;
        const headers = mapJoin(new Map(), defaultPostHeaders, request.headers);
        let contentType = null;
        if (isDefined(headers)) {
            const contentTypeHeaders = new Array();
            for (const key of headers.keys()) {
                if (key.toLowerCase() === "content-type") {
                    contentTypeHeaders.push(key);
                }
            }
            if (contentTypeHeaders.length > 0) {
                if (!(request.body instanceof FormData)) {
                    contentType = headers.get(contentTypeHeaders[0]);
                    // If there's more than one, keep just the first one
                    contentTypeHeaders.shift();
                }
                // delete all the rest, or all if we're submitting a form
                for (const key of contentTypeHeaders) {
                    headers.delete(key);
                }
            }
        }
        if (isXHRBodyInit(request.body) && !isString(request.body)
            || isString(request.body) && Text_Plain.matches(contentType)) {
            body = request.body;
        }
        else if (isDefined(request.body)) {
            body = JSON.stringify(request.body);
        }
        const hasBody = isDefined(body);
        const progs = progressSplit(progress, hasBody ? 2 : 1);
        const [progUpload, progDownload] = progs;
        const query = async () => {
            const xhr = new XMLHttpRequest();
            const upload = hasBody
                ? trackProgress("uploading", xhr, xhr.upload, progUpload, false)
                : Promise.resolve();
            const download = trackProgress("saving", xhr, xhr, progDownload, true, upload);
            sendRequest(xhr, request.method, request.path, request.timeout, headers, body);
            await upload;
            await download;
            const response = await this.#readResponse(request.path, xhr);
            return await this.#decodeContent(xhrType, response);
        };
        return withRetry(request.retryCount, query)();
    }
}
//# sourceMappingURL=FetchingServiceImpl.js.map