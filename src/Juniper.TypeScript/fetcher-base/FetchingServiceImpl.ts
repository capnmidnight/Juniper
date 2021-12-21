import type { IProgress } from "juniper-tslib";
import { isDefined, isString, isXHRBodyInit, mapJoin, progressPopper } from "juniper-tslib";
import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "./IFetcher";
import { ResponseTranslator } from "./ResponseTranslator";

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

function sendRequest(xhr: XMLHttpRequest, xhrType: XMLHttpRequestResponseType, method: string, path: string, timeout: number, headers: Map<string, string>, body?: BodyInit): void {
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
function readResponse<T>(xhr: XMLHttpRequest) {
    const parts = xhr
        .getAllResponseHeaders()
        .split(/[\r\n]+/)
        .map<[string, string]>(line => {
            const parts = line.split(": ");
            const key = parts.shift();
            const value = parts.join(": ");
            return [key, value];
        })
        .filter(kv => kv[0].length > 0);

    const headers = new Map<string, string>(parts);

    const response: IResponse<T> = {
        status: xhr.status,
        content: xhr.response as T,
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

export class FetchingServiceImpl
    extends ResponseTranslator
    implements IFetchingService {

    private readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    private async headOrGetXHR<T>(method: string, xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        const xhr = new XMLHttpRequest();
        const download = trackProgress(`requesting: ${request.path}`, xhr, xhr, progress, true);

        sendRequest(xhr, xhrType, method, request.path, request.timeout, request.headers);

        await download;
        return readResponse(xhr);
    }

    private getXHR<T>(xhrType: XMLHttpRequestResponseType, request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.headOrGetXHR("GET", xhrType, request, progress);
    }

    head(request: IRequest): Promise<IResponse<void>> {
        return this.headOrGetXHR("HEAD", "", request, null);
    }

    private async postXHR<T>(xhrType: XMLHttpRequestResponseType, request: IRequestWithBody, prog: IProgress): Promise<IResponse<T>> {

        let body: BodyInit = null;

        
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
