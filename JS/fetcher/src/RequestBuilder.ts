import { IResponse, assertNever, dispose, isDefined, isFunction, isString } from "@juniper-lib/util";
import { CanvasTypes, Img, Link, Rel, Script, Type, createCanvas, createOffscreenCanvas, drawImageToCanvas, hasOffscreenCanvas } from "@juniper-lib/dom";
import { once, waitFor } from "@juniper-lib/events";
import { Application_Javascript, Application_Json, Application_Wasm, MediaType, Text_Css, Text_Plain, Text_Xml } from "@juniper-lib/mediatypes";
import { IProgress } from "@juniper-lib/progress";
import { HTTPMethods } from "./HTTPMethods";
import { IFetcherBasic, IFetcherBodiedResult, IFetcherBodilessResult, IFetcherResult, IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec, IFetcherSendProgressTimeoutCredentialsCacheGetBody } from "./IFetcher";
import { IFetchingService } from "./IFetchingService";
import { IRequestWithBody } from "./IRequest";
import { translateResponse } from "./translateResponse";
declare const IS_WORKER: boolean;

// This HTML Audio Element is just used to get access to the
// canPlayType method. It is not used for any other functionality.
// This is to get around some of the crappy problems various
// browsers have with being able to report the types of media
// they can play.
let testAudio: HTMLAudioElement = null;
function canPlay(type: string): boolean {

    if (testAudio === null) {
        testAudio = new Audio();
    }

    return testAudio.canPlayType(type) !== "";
}

export class RequestBuilder implements
    IFetcherSendProgressTimeoutCredentialsCacheGetBody,
    IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec,
    IFetcherBasic,
    IFetcherBodiedResult,
    IFetcherResult,
    IFetcherBodilessResult {

    readonly #fetcher: IFetchingService;
    readonly #method: HTTPMethods;
    readonly #path: URL;
    readonly #useBLOBs: boolean;
    readonly #request: IRequestWithBody;
    #prog: IProgress = null;

    constructor(
        fetcher: IFetchingService,
        method: HTTPMethods,
        path: URL,
        useBLOBs = false) {
        this.#fetcher = fetcher;
        this.#method = method;
        this.#path = path;
        this.#useBLOBs = useBLOBs;
        this.#request = {
            method,
            path: this.#path.href,
            body: null,
            headers: null,
            timeout: null,
            withCredentials: false,
            useCache: false,
            retryCount: 3
        };
    }

    retries(count: number) {
        this.#request.retryCount = count;
        return this;
    }

    query(name: string, value: string) {
        this.#path.searchParams.set(name, value);
        this.#request.path = this.#path.href;
        return this;
    }

    header(name: string, value: string) {
        if (this.#request.headers === null) {
            this.#request.headers = new Map<string, string>();
        }

        this.#request.headers.set(name.toLowerCase(), value);
        return this;
    }

    headers(headers: Headers) {
        for (const [name, value] of headers.entries()) {
            this.header(name, value);
        }
        return this;
    }

    timeout(value: number) {
        this.#request.timeout = value;
        return this;
    }

    progress(prog: IProgress) {
        this.#prog = prog;
        return this;
    }

    body(body: any, contentType?: string | MediaType) {
        if (isDefined(body)) {
            const seen = new Set<unknown>();
            const queue = new Array<unknown>();
            queue.push(body);
            let isForm = false;
            while (!isForm && queue.length > 0) {
                const here = queue.shift();
                if (here && !seen.has(here)) {
                    seen.add(here);
                    if (here instanceof Blob) {
                        isForm = true;
                        break;
                    }
                    else if (!isString(here)) {
                        queue.push(...Object.values(here));
                    }
                }
            }
            if (isForm) {
                const form = new FormData();
                const fileNames = new Map<Blob, string>();
                const toSkip = new Set<string>();
                for (const [key, value] of Object.entries(body)) {
                    if (value instanceof Blob) {
                        const fileNameKey = key + ".name";
                        const fileName = body[fileNameKey];
                        if (isString(fileName)) {
                            fileNames.set(value, fileName);
                            toSkip.add(fileNameKey);
                        }
                    }
                }
                for (const [key, value] of Object.entries(body)) {
                    if (toSkip.has(key)) {
                        continue;
                    }

                    if (value instanceof Blob) {
                        form.append(key, value, fileNames.get(value));
                    }
                    else if (isString(value)) {
                        form.append(key, value);
                    }
                    else if (isDefined(value) && isFunction(value.toString)) {
                        form.append(key, value.toString());
                    }
                    else {
                        console.warn("Can't serialize value to formdata", key, value);
                    }
                }

                body = form;
                contentType = undefined;
            }

            this.#request.body = body;
            this.#content(contentType);
        }
        return this;
    }

    withCredentials() {
        this.#request.withCredentials = true;
        return this;
    }

    useCache(enabled = true) {
        this.#request.useCache = enabled;
        return this;
    }

    #media(key: string, mediaType: string | MediaType): void {
        if (isDefined(mediaType)) {
            if (!isString(mediaType)) {
                mediaType = mediaType.value;
            }
            this.header(key, mediaType);
        }
    }

    #content(contentType: string | MediaType): void {
        this.#media("content-type", contentType);
    }

    accept(acceptType: string | MediaType): this {
        this.#media("accept", acceptType);
        return this;
    }

    blob(acceptType?: string | MediaType): Promise<IResponse<Blob>> {
        this.accept(acceptType);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetBlob(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetBlob(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    buffer(acceptType?: string | MediaType): Promise<IResponse<ArrayBuffer>> {
        this.accept(acceptType);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetBuffer(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetBuffer(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    async file(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return await this.#fetcher.sendObjectGetFile(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            if (this.#useBLOBs) {
                return await this.#fetcher.sendNothingGetFile(this.#request, this.#prog);
            }
            else {
                const response = await this.#fetcher.sendNothingGetNothing(this.#request);
                return translateResponse(response, () => this.#request.path);
            }
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    async dataUri(acceptType?: string | MediaType): Promise<IResponse<string>> {
        const response = await this.blob(acceptType);
        const reader = new FileReader();
        const task = new Promise<string>((resolve) => reader.addEventListener("loadend", (evt) => resolve(evt.target.result as string)));
        reader.readAsDataURL(response.content);
        const value = await task;
        return await translateResponse(response, () => value);
    }

    text(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType || Text_Plain);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetText(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetText(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    object<T>(acceptType?: string | MediaType): Promise<IResponse<T>> {
        this.accept(acceptType || Application_Json);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetObject<T>(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetObject<T>(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    xml(acceptType?: string | MediaType): Promise<IResponse<HTMLElement>> {
        this.accept(acceptType || Text_Xml);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetXml(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetXml(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    imageBitmap(acceptType?: string | MediaType): Promise<IResponse<ImageBitmap>> {
        this.accept(acceptType);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetImageBitmap(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetImageBitmap(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    exec() {
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetNothing(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            throw new Error("GET requests should expect a response type");
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            return this.#fetcher.sendNothingGetNothing(this.#request);
        }
        else {
            assertNever(this.#method);
        }
    }

    async #audioBlob(acceptType: string | MediaType): Promise<IResponse<Blob>> {
        if (isDefined(acceptType)) {
            if (!isString(acceptType)) {
                acceptType = acceptType.value;
            }

            if (!canPlay(acceptType)) {
                throw new Error(`Probably can't play file of type "${acceptType}" at path: ${this.#request.path}`);
            }
        }

        const response = await this.blob(acceptType);
        if (canPlay(response.contentType)) {
            return response;
        }

        throw new Error(`Cannot play file of type "${response.contentType}" at path: ${this.#request.path}`);
    }

    async audioBuffer(context: BaseAudioContext, acceptType?: string | MediaType): Promise<IResponse<AudioBuffer>> {
        return translateResponse(
            await this.#audioBlob(acceptType),
            async (blob) => await context.decodeAudioData(await blob.arrayBuffer()));
    }


    async #htmlElement<ElementT extends HTMLAudioElement | HTMLVideoElement | HTMLImageElement | HTMLScriptElement | HTMLLinkElement, EventsT extends HTMLElementEventMap>(element: ElementT, resolveEvt: keyof EventsT & string, acceptType: string | MediaType): Promise<IResponse<ElementT>> {
        const response = await this.file(acceptType);
        const task = once(element, resolveEvt, "error");
        if (element instanceof HTMLLinkElement) {
            element.href = response.content;
        }
        else {
            element.src = response.content;
        }
        await task;

        return await translateResponse(response, () => element);
    }

    image(acceptType?: string | MediaType): Promise<IResponse<HTMLImageElement>> {
        return this.#htmlElement(
            Img(),
            "load",
            acceptType
        );
    }

    async htmlCanvas(acceptType?: string | MediaType): Promise<IResponse<HTMLCanvasElement>> {
        if (IS_WORKER) {
            throw new Error("HTMLCanvasElement not supported in Workers.");
        }

        const canvas = createCanvas(1, 1);
        if (this.#method === "GET") {
            if (hasOffscreenCanvas) {
                this.accept(acceptType);
                const response = await this.#fetcher.drawImageToCanvas(this.#request, canvas.transferControlToOffscreen(), this.#prog);
                return await translateResponse<void, HTMLCanvasElement>(response, () => canvas);
            }
            else {
                const response: IResponse<HTMLImageElement | ImageBitmap> = await (IS_WORKER
                    ? this.imageBitmap(acceptType)
                    : this.image(acceptType));

                return await translateResponse(response, (img) => {
                    canvas.width = img.width;
                    canvas.height = img.height;
                    drawImageToCanvas(canvas, img);
                    dispose(img);
                    return canvas;
                });
            }
        }
        else if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE"
            || this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    canvas(acceptType?: string | MediaType): Promise<IResponse<CanvasTypes>> {
        if (hasOffscreenCanvas) {
            return this.offscreenCanvas(acceptType);
        }
        else {
            return this.htmlCanvas(acceptType);
        }
    }

    async offscreenCanvas(acceptType?: string | MediaType): Promise<IResponse<OffscreenCanvas>> {
        if (!hasOffscreenCanvas) {
            throw new Error("This system does not support OffscreenCanvas");
        }

        if (this.#method === "GET") {
            const response: IResponse<HTMLImageElement | ImageBitmap> = await (IS_WORKER
                ? this.imageBitmap(acceptType)
                : this.image(acceptType));

            return await translateResponse(response, (img) => {
                const canvas = createOffscreenCanvas(img.width, img.height);
                drawImageToCanvas(canvas, img);
                dispose(img);
                return canvas;
            });
        }
        else if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE"
            || this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }

    async style(): Promise<IResponse> {
        const tag = Link(
            Type(Text_Css),
            Rel("stylesheet")
        );
        document.head.append(tag);
        const response = await this.#htmlElement(
            tag,
            "load",
            Text_Css);
        return translateResponse(response);
    }

    async #getScript(): Promise<IResponse> {
        const tag = Script(Type(Application_Javascript));
        document.head.append(tag);
        const response = await this.#htmlElement(
            tag,
            "load",
            Application_Javascript);
        return translateResponse(response);
    }

    async script(test: () => boolean): Promise<IResponse> {
        let response: IResponse = null;

        const scriptPath = this.#request.path;

        if (!test) {
            response = await this.#getScript();
        }
        else if (!test()) {
            const scriptLoadTask = waitFor(test);
            response = await this.#getScript();
            await scriptLoadTask;
        }

        if (this.#prog) {
            this.#prog.end(scriptPath);
        }

        return response;
    }

    async module<T>(): Promise<IResponse<T>> {
        const scriptPath = this.#request.path;
        const response = await this.file(Application_Javascript);
        const value = await import(response.content);

        if (this.#prog) {
            this.#prog.end(scriptPath);
        }

        return translateResponse(response, () => value);
    }

    async wasm<T>(imports: Record<string, Record<string, WebAssembly.ImportValue>>): Promise<IResponse<T>> {
        const response = await this.buffer(Application_Wasm);
        if (!Application_Wasm.matches(response.contentType)) {
            throw new Error(`Server did not respond with WASM file. Was: ${response.contentType}`);
        }

        const module = await WebAssembly.compile(response.content);
        const instance = await WebAssembly.instantiate(module, imports);
        return translateResponse(response, () => (instance.exports as any) as T);
    }

    async worker(type: WorkerType = "module"): Promise<IResponse<Worker>> {
        const scriptPath = this.#request.path;
        const response = await this.file(Application_Javascript);
        this.#prog = null;
        this.#request.timeout = null;
        const worker = new Worker(response.content, { type });
        if (this.#prog) {
            this.#prog.end(scriptPath);
        }
        return translateResponse(response, () => worker);
    }
}
