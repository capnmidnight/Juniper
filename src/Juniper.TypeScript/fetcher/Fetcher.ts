import { type } from "juniper-dom/attrs";
import { BackgroundAudio, BackgroundVideo, getInput, Img, Script } from "juniper-dom/tags";
import {
    HTTPMethods,
    IFetcher,
    IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials,
    IFetcherGetResult,
    IFetcherHeadHeadersAndTimeoutAndWithCredentials,
    IFetcherHeadResult,
    IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials,
    IFetcherPostResult,
    IFetchingService,
    IRequestWithBody,
    IResponse
} from "juniper-fetcher-base/IFetcher";
import { ResponseTranslator } from "juniper-fetcher-base/ResponseTranslator";
import { MediaType, mediaTypeGuessByFileName } from "juniper-mediatypes";
import { Application_Javascript, Application_Json, Application_Wasm } from "juniper-mediatypes/application";
import { Text_Plain, Text_Xml } from "juniper-mediatypes/text";
import { assertNever, Exception, IProgress, isDefined, isString, once, waitFor } from "juniper-tslib";


let testAudio: HTMLAudioElement = null;
function shouldTry(path: string): boolean {
    if (testAudio === null) {
        testAudio = new Audio();
    }

    const idx = path.lastIndexOf(".");
    if (idx > -1) {
        const types = mediaTypeGuessByFileName(path);
        for (const type of types) {
            if (testAudio.canPlayType(type.value)) {
                return true;
            }
        }

        return false;
    }

    return true;
}

class RequestBuilder
    extends ResponseTranslator
    implements
    IFetcherGetHeadersAndProgressAndTimeoutAndWithCredentials,
    IFetcherPostHeadersAndProgressAndBodyAndTimeoutAndWithCredentials,
    IFetcherHeadHeadersAndTimeoutAndWithCredentials,
    IFetcherGetResult,
    IFetcherPostResult,
    IFetcherHeadResult {

    private readonly path: URL;
    private readonly request: IRequestWithBody;
    private prog: IProgress = null;

    constructor(private readonly fetcher: IFetchingService, private readonly useBlobURIs: boolean, private readonly method: HTTPMethods, path: URL) {
        super();

        this.path = path;
        this.request = {
            path: this.path.toString(),
            body: null,
            headers: null,
            timeout: null,
            withCredentials: false
        };
    }

    query(name: string, value: string) {
        this.path.searchParams.set(name, value);
        this.request.path = this.path.toString();
        return this;
    }

    header(name: string, value: string) {
        if (this.request.headers === null) {
            this.request.headers = new Map<string, string>();
        }

        this.request.headers.set(name.toLowerCase(), value);
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

    private accept(acceptType: string | MediaType): void {
        this.media("accept", acceptType);
    }

    blob(acceptType?: string | MediaType): Promise<IResponse<Blob>> {
        this.accept(acceptType);
        if (this.method === "POST") {
            return this.fetcher.postObjectForBlob(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getBlob(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    buffer(acceptType?: string | MediaType): Promise<IResponse<ArrayBuffer>> {
        this.accept(acceptType);
        if (this.method === "POST") {
            return this.fetcher.postObjectForBuffer(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getBuffer(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    file(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType);
        if (this.method === "POST") {
            return this.fetcher.postObjectForFile(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getFile(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    text(acceptType?: string | MediaType): Promise<IResponse<string>> {
        this.accept(acceptType || Text_Plain);
        if (this.method === "POST") {
            return this.fetcher.postObjectForText(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getText(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    object<T>(acceptType?: string | MediaType): Promise<T> {
        this.accept(acceptType || Application_Json);
        if (this.method === "POST") {
            return this.fetcher.postObjectForObject<T>(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getObject<T>(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    xml(acceptType?: string | MediaType): Promise<IResponse<HTMLElement>> {
        this.accept(acceptType || Text_Xml);
        if (this.method === "POST") {
            return this.fetcher.postObjectForXml(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getXml(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    imageBitmap(acceptType?: string | MediaType): Promise<IResponse<ImageBitmap>> {
        this.accept(acceptType);
        if (this.method === "POST") {
            return this.fetcher.postObjectForImageBitmap(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getImageBitmap(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }

    exec() {
        if (this.method === "POST") {
            return this.fetcher.postObject(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            return this.fetcher.head(this.request);
        }
        else if (this.method === "GET") {
            throw new Exception("GET requests should expect a response type");
        }
        else {
            assertNever(this.method);
        }
    }

    private async audioBlob(acceptType: string | MediaType): Promise<IResponse<Blob>> {
        let goodBlob: IResponse<Blob> = null;
        if (!shouldTry(this.request.path.toString())) {
            if (this.prog) {
                this.prog.report(1, 1, "skip " + this.request.path);
            }
        }
        else {
            const response = await this.blob(acceptType);
            if (testAudio.canPlayType(response.contentType)) {
                goodBlob = response;
            }
        }

        if (!goodBlob) {
            throw new Error(`Cannot play file: ${this.request.path}`);
        }

        return goodBlob;
    }

    audioBuffer(audioCtx: BaseAudioContext, acceptType?: string | MediaType): Promise<IResponse<AudioBuffer>> {
        return this.translateResponse(
            this.audioBlob(acceptType),
            async blob => await audioCtx.decodeAudioData(await blob.arrayBuffer()));
    }

    private async htmlElement<
        ElementT extends HTMLAudioElement | HTMLVideoElement | HTMLImageElement | HTMLScriptElement,
        EventsT extends HTMLElementEventMap,
        ResolveEventKeyT extends keyof EventsT & string>(
            element: ElementT,
            resolveEvt: ResolveEventKeyT,
            acceptType: string | MediaType): Promise<IResponse<ElementT>> {
        const response = await this.file(acceptType);
        const task = once<EventsT, ResolveEventKeyT>(element, resolveEvt, "error");
        element.src = response.content;
        await task;

        return this.translateResponse(Promise.resolve(response), () => element);
    }

    image(acceptType?: string | MediaType): Promise<IResponse<HTMLImageElement>> {
        return this.htmlElement(
            Img(),
            "load",
            acceptType
        );
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

    private async getScript() {
        const tag = Script(type(Application_Javascript));
        document.body.append(tag);
        await this.htmlElement(
            tag,
            "load",
            Application_Javascript);
    }

    async script(test: () => boolean): Promise<void> {
        if (!test) {
            await this.getScript();
        }
        else if (!test()) {
            const scriptLoadTask = waitFor(test);
            await this.getScript();
            await scriptLoadTask;
        }
    }

    async module<T>(): Promise<T> {
        let scriptPath = this.request.path.toString();
        if (this.useBlobURIs) {
            const { content: file } = await this.file(Application_Javascript);
            scriptPath = file;
        }

        const value = await import(scriptPath);
        if (this.prog) {
            this.prog.report(1, 1, scriptPath);
        }
        return value;
    }

    async wasm<T>(imports: Record<string, Record<string, WebAssembly.ImportValue>>): Promise<T> {
        const { content: buffer, contentType } = await this.buffer(Application_Wasm);
        if (contentType !== "application/wasm") {
            throw new Error(`Server did not respond with WASM file. Was: ${contentType}`);
        }

        const module = await WebAssembly.compile(buffer);
        const instance = await WebAssembly.instantiate(module, imports);
        return (instance.exports as any) as T;
    }

    async worker(type: WorkerType = "module"): Promise<Worker> {
        let path = this.request.path.toString();

        if (this.useBlobURIs) {
            const { content } = await this.file(path);
            path = content;
            this.prog = null;
            this.request.timeout = null;
        }

        return new Worker(path, { type });
    }
}

export class Fetcher implements IFetcher {
    constructor(private readonly fetcher: IFetchingService, private readonly useBlobURIs: boolean) {
        const antiforgeryToken = getInput("input[name=__RequestVerificationToken]");
        if (antiforgeryToken) {
            this.fetcher.setRequestVerificationToken(antiforgeryToken.value);
        }
    }

    private createRequest(method: HTTPMethods, path: string, base?: string) {
        return new RequestBuilder(this.fetcher, this.useBlobURIs, method, new URL(path, base || location.href));
    }

    get(path: string, base?: string) {
        return this.createRequest("GET", path, base);
    }

    post(path: string, base?: string) {
        return this.createRequest("POST", path, base);
    }

    head(path: string, base?: string) {
        return this.createRequest("HEAD", path, base);
    }
}

