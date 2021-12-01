import { isDefined, isString, mapJoin, progressPopper } from "juniper-tslib";
import { isXHRBodyInit } from "juniper-tslib-browser";
import { ResponseTranslator } from "./ResponseTranslator";
function trackProgress(name, xhr, target, onProgress, skipLoading, prevTask) {
    return new Promise((resolve, reject) => {
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
        function onError(msg) {
            return () => {
                if (prevDone) {
                    reject(`${msg} (${xhr.status})`);
                }
            };
        }
        target.addEventListener("loadstart", () => {
            if (prevDone && !done && onProgress) {
                onProgress.report(0, 1, name);
            }
        });
        target.addEventListener("progress", (ev) => {
            if (prevDone && !done) {
                const evt = ev;
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
function sendRequest(xhr, xhrType, method, path, timeout, headers, body) {
    xhr.open(method, path);
    xhr.responseType = xhrType;
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
function readResponse(xhr) {
    const parts = xhr
        .getAllResponseHeaders()
        .split(/[\r\n]+/)
        .map(line => {
        const parts = line.split(": ");
        const key = parts.shift();
        const value = parts.join(": ");
        return [key, value];
    })
        .filter(kv => kv[0].length > 0);
    const headers = new Map(parts);
    const response = {
        status: xhr.status,
        content: xhr.response,
        contentType: readResponseHeader(headers, "content-type", v => v),
        contentLength: readResponseHeader(headers, "content-length", parseFloat),
        fileName: readResponseHeader(headers, "content-disposition", v => {
            if (isDefined(v)) {
                const match = v.match(FILE_NAME_PATTERN);
                if (isDefined(match)) {
                    return match[1];
                }
            }
            return null;
        }),
        date: readResponseHeader(headers, "date", v => new Date(v)),
        headers
    };
    return response;
}
export class FetchingServiceImpl extends ResponseTranslator {
    defaultPostHeaders = new Map();
    setRequestVerificationToken(value) {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }
    async headOrGetXHR(method, xhrType, request, progress) {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);
        sendRequest(xhr, xhrType, method, request.path, request.timeout, request.headers);
        await download;
        return readResponse(xhr);
    }
    getXHR(xhrType, request, progress) {
        return this.headOrGetXHR("GET", xhrType, request, progress);
    }
    head(request) {
        return this.headOrGetXHR("HEAD", "", request, null);
    }
    async postXHR(xhrType, request, prog) {
        let body = null;
        const headers = mapJoin(new Map(), this.defaultPostHeaders, request.headers);
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
        if (isXHRBodyInit(request.body) && !isString(request.body)) {
            body = request.body;
        }
        else if (isDefined(request.body)) {
            body = JSON.stringify(request.body);
        }
        const progs = progressPopper(prog);
        const xhr = new XMLHttpRequest();
        const upload = isDefined(body)
            ? trackProgress("uploading", xhr, xhr.upload, progs.pop(), false)
            : Promise.resolve();
        const download = trackProgress("saving", xhr, xhr, progs.pop(), true, upload);
        sendRequest(xhr, xhrType, "POST", request.path, request.timeout, headers, body);
        await upload;
        await download;
        return readResponse(xhr);
    }
    getBlob(request, progress) {
        return this.getXHR("blob", request, progress);
    }
    postObjectForBlob(request, progress) {
        return this.postXHR("blob", request, progress);
    }
    getBuffer(request, progress) {
        return this.getXHR("arraybuffer", request, progress);
    }
    postObjectForBuffer(request, progress) {
        return this.postXHR("arraybuffer", request, progress);
    }
    getText(request, progress) {
        return this.getXHR("text", request, progress);
    }
    postObjectForText(request, progress) {
        return this.postXHR("text", request, progress);
    }
    async getObject(request, progress) {
        const response = await this.getXHR("json", request, progress);
        return response.content;
    }
    async postObjectForObject(request, progress) {
        const response = await this.postXHR("json", request, progress);
        return response.content;
    }
    postObject(request, progress) {
        return this.postXHR("", request, progress);
    }
    getFile(request, progress) {
        return this.translateResponse(this.getBlob(request, progress), URL.createObjectURL);
    }
    postObjectForFile(request, progress) {
        return this.translateResponse(this.postObjectForBlob(request, progress), URL.createObjectURL);
    }
    getXml(request, progress) {
        return this.translateResponse(this.getXHR("document", request, progress), doc => doc.documentElement);
    }
    postObjectForXml(request, progress) {
        return this.translateResponse(this.postXHR("document", request, progress), doc => doc.documentElement);
    }
    getImageBitmap(request, progress) {
        return this.translateResponse(this.getBlob(request, progress), createImageBitmap);
    }
    async postObjectForImageBitmap(request, progress) {
        return this.translateResponse(this.postObjectForBlob(request, progress), createImageBitmap);
    }
}
