import { type } from "@juniper-lib/dom/attrs";
import { CanvasTypes, createCanvas, createOffscreenCanvas, drawImageToCanvas, hasOffscreenCanvas } from "@juniper-lib/dom/canvas";
import { BackgroundAudio, BackgroundVideo, Img, Script } from "@juniper-lib/dom/tags";
import { Application_Javascript, Application_Json, Application_Wasm, MediaType, Text_Plain, Text_Xml } from "@juniper-lib/mediatypes";
import { once } from "@juniper-lib/tslib/events/once";
import { waitFor } from "@juniper-lib/tslib/events/waitFor";
import { Exception } from "@juniper-lib/tslib/Exception";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { assertNever, isDefined, isString } from "@juniper-lib/tslib/typeChecks";
import { dispose } from "@juniper-lib/tslib/using";
import { HTTPMethods } from "./HTTPMethods";
import { IFetcherBasic, IFetcherBodiedResult, IFetcherBodilessResult, IFetcherResult, IFetcherSendProgressBodyTimeoutCredentialsGetBodyOrExec, IFetcherSendProgressTimeoutCredentialsCacheGetBody } from "./IFetcher";
import { IFetchingService } from "./IFetchingService";
import { IRequestWithBody } from "./IRequest";
import { IResponse } from "./IResponse";
import { translateResponse } from "./translateResponse";

declare const IS_WORKER: boolean;

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

    private readonly path: URL;
    private readonly request: IRequestWithBody;
    private prog: IProgress = null;

    constructor(private readonly fetcher: IFetchingService, private readonly useFileBlobsForModules: boolean, private readonly method: HTTPMethods, path: URL) {
        this.path = path;
        this.request = {
            method,
            path: this.path.href,
            body: null,
            headers: null,
            timeout: null,
            withCredentials: false,
            useCache: false
        };
    }

    query(name: string, value: string) {
        this.path.searchParams.set(name, value);
        this.request.path = this.path.href;
        return this;
    }

    header(name: string, value: string) {
        if (this.request.headers === null) {
            this.request.headers = new Map<string, string>();
        }

        this.request.headers.set(name.toLowerCase(), value);
        return this;
    }

    headers(headers: Headers) {
        for (const [ name, value ] of headers.entries()) {
            this.header(name, value);
        }
        return this;
    }

    timeout(value: number) {
        this.request.timeout = value;
        return this;
    }

    progress(prog: IProgress) {
        this.prog = prog;
        return this;
    }

    body(body: any, contentType?: string | MediaType) {
        this.request.body = body;
        this.content(contentType);
        return this;
    }

    withCredentials() {
        this.request.withCredentials = true;
        return this;
    }

    useCache(enabled = true) {
        this.request.useCache = enabled;
        return this;
    }

    private media(key: string, mediaType: string | MediaType): void {
        if (isDefined(mediaType)) {
            if (!isString(mediaType)) {
                mediaType = mediaType.value;
            }
            this.header(key, mediaType);
        }
    }

    private content(contentType: string | MediaType): void {
        this.media("content-type", contentType);
    }

    accept(acceptType: string | MediaType): this {
        this.media("accept", acceptType);
        return this;
    }

    blob(acceptType?: string | MediaType): Promise<IResponse<Blob>> {
        this.accept(acceptType);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetBlob(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetBlob(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    buffer(acceptType?: string | MediaType): Promise<IResponse<ArrayBuffer>> {
        this.accept(acceptType);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetBuffer(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetBuffer(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    file(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetFile(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetFile(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    text(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType || Text_Plain);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetText(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetText(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    object<T>(acceptType?: string | MediaType): Promise<T> {
        this.accept(acceptType || Application_Json);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetObject<T>(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetObject<T>(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    xml(acceptType?: string | MediaType): Promise<IResponse<HTMLElement>> {
        this.accept(acceptType || Text_Xml);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetXml(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetXml(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    imageBitmap(acceptType?: string | MediaType): Promise<IResponse<ImageBitmap>> {
        this.accept(acceptType);
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetImageBitmap(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.sendNothingGetImageBitmap(this.request, this.prog);
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    exec() {
        if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE") {
            return this.fetcher.sendObjectGetNothing(this.request, this.prog);
        }
        else if (this.method === "GET") {
            throw new Exception("GET requests should expect a response type");
        }
        else if (this.method === "HEAD"
            || this.method === "OPTIONS") {
            return this.fetcher.sendNothingGetNothing(this.request);
        }
        else {
            assertNever(this.method);
        }
    }

    private async audioBlob(acceptType: string | MediaType): Promise<IResponse<Blob>> {
        if (isDefined(acceptType)) {
            if (!isString(acceptType)) {
                acceptType = acceptType.value;
            }

            if (!canPlay(acceptType)) {
                throw new Error(`Probably can't play file of type "${acceptType}" at path: ${this.request.path}`);
            }
        }

        const response = await this.blob(acceptType);
        if (canPlay(response.contentType)) {
            return response;
        }

        throw new Error(`Cannot play file of type "${response.contentType}" at path: ${this.request.path}`);
    }

    async audioBuffer(audioCtx: BaseAudioContext, acceptType?: string | MediaType): Promise<IResponse<AudioBuffer>> {
        return translateResponse(
            await this.audioBlob(acceptType),
            async (blob) => await audioCtx.decodeAudioData(await blob.arrayBuffer()));
    }


    private async htmlElement<
        ElementT extends HTMLAudioElement | HTMLVideoElement | HTMLImageElement | HTMLScriptElement,
        EventsT extends HTMLElementEventMap>(
            element: ElementT,
            resolveEvt: keyof EventsT & string,
            acceptType: string | MediaType): Promise<IResponse<ElementT>> {

        const response = await this.file(acceptType);
        const task = once<EventsT>(element, resolveEvt, "error");
        element.src = response.content;
        await task;

        return await translateResponse(response, () => element);
    }

    image(acceptType?: string | MediaType): Promise<IResponse<HTMLImageElement>> {
        return this.htmlElement(
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
        if (this.method === "GET") {
            if (hasOffscreenCanvas) {
                this.accept(acceptType);
                const response = await this.fetcher.drawImageToCanvas(this.request, canvas.transferControlToOffscreen(), this.prog);
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
        else if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE"
            || this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
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

        if (this.method === "GET") {
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
        else if (this.method === "POST"
            || this.method === "PUT"
            || this.method === "PATCH"
            || this.method === "DELETE"
            || this.method === "HEAD"
            || this.method === "OPTIONS") {
            throw new Error(`${this.method} responses do not contain bodies`);
        }
        else {
            assertNever(this.method);
        }
    }

    audio(autoPlaying: boolean, looping: boolean, acceptType?: string | MediaType): Promise<IResponse<HTMLAudioElement>> {
        return this.htmlElement(
            BackgroundAudio(autoPlaying, false, looping),
            "canplay",
            acceptType
        );
    }

    video(autoPlaying: boolean, looping: boolean, acceptType?: string | MediaType): Promise<IResponse<HTMLVideoElement>> {
        return this.htmlElement(
            BackgroundVideo(autoPlaying, false, looping),
            "canplay",
            acceptType
        );
    }

    private async getScript(): Promise<void> {
        const tag = Script(type(Application_Javascript));
        document.body.append(tag);
        if (this.useFileBlobsForModules) {
            await this.htmlElement(
                tag,
                "load",
                Application_Javascript);
        }
        else {
            tag.src = this.request.path;
        }
    }

    async script(test: () => boolean): Promise<void> {
        const scriptPath = this.request.path;
        if (!test) {
            await this.getScript();
        }
        else if (!test()) {
            const scriptLoadTask = waitFor(test);
            await this.getScript();
            await scriptLoadTask;
        }
        if (this.prog) {
            this.prog.end(scriptPath);
        }
    }

    async module<T>(): Promise<T> {
        const scriptPath = this.request.path;
        let requestPath = scriptPath;
        if (this.useFileBlobsForModules) {
            const { content: file } = await this.file(Application_Javascript);
            requestPath = file;
        }

        const value = await import(requestPath);
        if (this.prog) {
            this.prog.end(scriptPath);
        }
        return value;
    }

    async wasm<T>(imports: Record<string, Record<string, WebAssembly.ImportValue>>): Promise<T> {
        const { content: buffer, contentType } = await this.buffer(Application_Wasm);
        if (!Application_Wasm.matches(contentType)) {
            throw new Error(`Server did not respond with WASM file. Was: ${contentType}`);
        }

        const module = await WebAssembly.compile(buffer);
        const instance = await WebAssembly.instantiate(module, imports);
        return (instance.exports as any) as T;
    }

    async worker(type: WorkerType = "module"): Promise<Worker> {
        const scriptPath = this.request.path;
        let requestPath = scriptPath;
        if (this.useFileBlobsForModules) {
            const { content: file } = await this.file(Application_Javascript);
            requestPath = file;
        }
        this.prog = null;
        this.request.timeout = null;
        const worker = new Worker(requestPath, { type });
        if (this.prog) {
            this.prog.end(scriptPath);
        }
        return worker;
    }
}
