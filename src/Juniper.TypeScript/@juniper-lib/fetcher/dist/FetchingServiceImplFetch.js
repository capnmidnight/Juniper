import { IDexDB } from "@juniper-lib/indexdb/dist";
import { mapJoin } from "@juniper-lib/collections/dist/mapJoin";
import { PriorityList } from "@juniper-lib/collections/dist/PriorityList";
import { PriorityMap } from "@juniper-lib/collections/dist/PriorityMap";
import { withRetry } from "@juniper-lib/events/dist/withRetry";
import { identity } from "@juniper-lib/tslib/dist/identity";
import { assertNever, isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { using } from "@juniper-lib/tslib/dist/using";
import { isXHRBodyInit } from "./FetchingServiceImplXHR";
import { translateResponse } from "./translateResponse";
function isBodyInit(obj) {
    return isXHRBodyInit(obj)
        || obj instanceof ReadableStream;
}
function sendRequest(method, path, headers, body) {
    const request = {
        method
    };
    if (headers) {
        request.headers = {};
        for (const [key, value] of headers) {
            request.headers[key] = value;
        }
    }
    if (isDefined(body)) {
        request.body = body;
    }
    return fetch(path, request);
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
export class FetchingServiceImplFetch {
    constructor() {
        this.cache = null;
        this.store = null;
        this.tasks = new PriorityMap();
        this.cacheReady = this.openCache();
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
    async openCache() {
        const options = {
            keyPath: "requestPath"
        };
        this.cache = await IDexDB.open(DB_NAME, {
            name: "files",
            options
        });
        this.store = await this.cache.getStore("files");
    }
    async clearCache() {
        await this.cacheReady;
        await this.store.clear();
    }
    async evict(path) {
        await this.cacheReady;
        if (this.store.has(path)) {
            await this.store.delete(path);
        }
    }
    async readResponseHeaders(requestPath, res) {
        const headerParts = Array.from(res
            .headers
            .entries());
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
            status: res.status,
            requestPath,
            responsePath: res.url,
            content: undefined,
            contentType,
            contentLength,
            fileName,
            date,
            headers
        };
        return response;
    }
    async readResponse(requestPath, res) {
        const { status, responsePath, contentType, contentLength, fileName, date, headers } = await this.readResponseHeaders(requestPath, res);
        const response = {
            requestPath,
            responsePath,
            status,
            contentType,
            contentLength,
            fileName,
            date,
            headers,
            content: await res.blob()
        };
        if (isDefined(response.content)) {
            response.contentType = response.contentType || response.content.type;
            response.contentLength = response.contentLength || response.content.size;
        }
        return response;
    }
    async decodeContent(xhrType, response) {
        return translateResponse(response, async () => {
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
                return response.content;
            }
            else if (xhrType === "arraybuffer") {
                return (await response.content.arrayBuffer());
            }
            else if (xhrType === "json") {
                const text = await response.content.text();
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
                    return parser.parseFromString(await response.content.text(), response.contentType);
                }
                else {
                    throw new Error("Couldn't parse document");
                }
            }
            else if (xhrType === "text") {
                return (await response.content.text());
            }
            else {
                assertNever(xhrType);
            }
        });
    }
    async withCachedTask(request, action) {
        if (request.method !== "GET"
            && request.method !== "HEAD"
            && request.method !== "OPTIONS") {
            return await action();
        }
        if (!this.tasks.has(request.method, request.path)) {
            this.tasks.add(request.method, request.path, action().finally(() => this.tasks.delete(request.method, request.path)));
        }
        return this.tasks.get(request.method, request.path);
    }
    sendNothingGetNothing(request) {
        return this.withCachedTask(request, withRetry(request.retryCount, async () => {
            const res = await sendRequest(request.method, request.path, request.headers);
            return await this.readResponseHeaders(request.path, res);
        }));
    }
    sendNothingGetSomething(xhrType, request, progress) {
        return this.withCachedTask(request, withRetry(request.retryCount, async () => {
            let response = null;
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
                const res = await sendRequest(request.method, request.path, request.headers);
                response = await this.readResponse(request.path, res);
                if (useCache) {
                    await this.store.add(response);
                }
            }
            const value = await this.decodeContent(xhrType, response);
            if (noCachedResponse && isDefined(progress)) {
                progress.end();
            }
            return value;
        }));
    }
    async sendSomethingGetSomething(xhrType, request, defaultPostHeaders, _progress) {
        let body = null;
        const headers = mapJoin(new Map(), defaultPostHeaders, request.headers);
        if (request.body instanceof FormData
            && isDefined(headers)) {
            const toDelete = new Array();
            for (const key of headers.keys()) {
                if (key.toLowerCase() === "content-type") {
                    toDelete.push(key);
                }
            }
            for (const key of toDelete) {
                headers.delete(key);
            }
        }
        if (isBodyInit(request.body) && !isString(request.body)) {
            body = request.body;
        }
        else if (isDefined(request.body)) {
            body = JSON.stringify(request.body);
        }
        const query = async () => {
            const res = await sendRequest(request.method, request.path, headers, body);
            const response = await this.readResponse(request.path, res);
            return await this.decodeContent(xhrType, response);
        };
        return withRetry(request.retryCount, query)();
    }
}
//# sourceMappingURL=FetchingServiceImplFetch.js.map