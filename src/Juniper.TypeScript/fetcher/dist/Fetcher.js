import { BackgroundAudio, BackgroundVideo, getInput, hasImageBitmap, Img, Script, type } from "juniper-dom";
import { Application_Javascript, Application_Json, Application_Wasm, mediaTypeGuessByFileName, Text_Plain, Text_Xml } from "juniper-mediatypes";
import { assertNever, Exception, isDefined, isString, once, waitFor } from "juniper-tslib";
import { ResponseTranslator } from "juniper-fetcher-base";
let testAudio = null;
function shouldTry(path) {
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
function identity(v) {
    return v;
}
class RequestBuilder extends ResponseTranslator {
    fetcher;
    useBlobURIs;
    method;
    request;
    prog = null;
    constructor(fetcher, useBlobURIs, method, path, handler) {
        super();
        this.fetcher = fetcher;
        this.useBlobURIs = useBlobURIs;
        this.method = method;
        if (handler) {
            const url = new URL(path, location.href);
            url.searchParams.set("handler", handler);
            path = url.href;
        }
        this.request = {
            path,
            body: null,
            headers: null,
            timeout: null,
            withCredentials: false
        };
    }
    header(name, value) {
        if (this.request.headers === null) {
            this.request.headers = new Map();
        }
        this.request.headers.set(name.toLowerCase(), value);
        return this;
    }
    timeout(value) {
        this.request.timeout = value;
        return this;
    }
    progress(prog) {
        this.prog = prog;
        return this;
    }
    body(body, contentType) {
        this.request.body = body;
        this.content(contentType);
        return this;
    }
    withCredentials() {
        this.request.withCredentials = true;
        return this;
    }
    media(key, mediaType) {
        if (isDefined(mediaType)) {
            if (!isString(mediaType)) {
                mediaType = mediaType.value;
            }
            this.header(key, mediaType);
        }
    }
    content(contentType) {
        this.media("content-type", contentType);
    }
    accept(acceptType) {
        this.media("accept", acceptType);
    }
    blob(acceptType) {
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
    buffer(acceptType) {
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
    file(acceptType) {
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
    text(acceptType) {
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
    object(acceptType) {
        this.accept(acceptType || Application_Json);
        if (this.method === "POST") {
            return this.fetcher.postObjectForObject(this.request, this.prog);
        }
        else if (this.method === "GET") {
            return this.fetcher.getObject(this.request, this.prog);
        }
        else if (this.method === "HEAD") {
            throw new Error("HEAD responses do not contain bodies");
        }
        else {
            assertNever(this.method);
        }
    }
    xml(acceptType) {
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
    imageBitmap(acceptType) {
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
    canvasImage(acceptType) {
        if (hasImageBitmap) {
            return this.imageBitmap(acceptType);
        }
        else {
            return this.image(acceptType);
        }
    }
    async audioBlob(acceptType) {
        let goodBlob = null;
        if (!shouldTry(this.request.path)) {
            if (this.prog) {
                this.prog.report(1, 1, "skip");
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
    audioBuffer(audioCtx, acceptType) {
        return this.translateResponse(this.audioBlob(acceptType), async (blob) => await audioCtx.decodeAudioData(await blob.arrayBuffer()));
    }
    async htmlElement(element, resolveEvt, getResponse, translateContent) {
        let path = this.request.path;
        const r = {
            status: 500,
            content: element,
            contentLength: null,
            contentType: null,
            date: null,
            fileName: this.request.path,
            headers: null
        };
        if (this.useBlobURIs) {
            const response = await getResponse();
            r.status = response.status;
            r.contentType = response.contentType;
            r.contentLength = response.contentLength;
            r.fileName = response.fileName;
            r.headers = response.headers;
            r.date = response.date;
            path = translateContent(response.content);
            this.prog = null;
            this.request.timeout = null;
        }
        const task = once(r.content, resolveEvt, "error", this.request.timeout);
        r.content.src = path;
        await task;
        r.status = r.status || 200;
        if (this.prog) {
            this.prog.report(1, 1, "complete");
        }
        return r;
    }
    image(acceptType) {
        return this.htmlElement(Img(), "load", () => this.file(acceptType), identity);
    }
    audio(autoPlaying, looping, acceptType) {
        return this.htmlElement(BackgroundAudio(autoPlaying, false, looping), "canplay", () => this.audioBlob(acceptType), URL.createObjectURL);
    }
    video(autoPlaying, looping, acceptType) {
        return this.htmlElement(BackgroundVideo(autoPlaying, false, looping), "canplay", () => this.file(acceptType), identity);
    }
    async getScript() {
        const tag = Script(type(Application_Javascript));
        document.body.append(tag);
        await this.htmlElement(tag, "load", () => this.file(Application_Javascript), identity);
    }
    async script(test) {
        if (!test) {
            await this.getScript();
        }
        else if (!test()) {
            const scriptLoadTask = waitFor(test);
            await this.getScript();
            await scriptLoadTask;
        }
    }
    async module() {
        let scriptPath = this.request.path;
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
    async wasm(imports) {
        const { content: buffer, contentType } = await this.buffer(Application_Wasm);
        if (contentType !== "application/wasm") {
            throw new Error(`Server did not respond with WASM file. Was: ${contentType}`);
        }
        const module = await WebAssembly.compile(buffer);
        const instance = await WebAssembly.instantiate(module, imports);
        return instance.exports;
    }
    async worker(type = "module") {
        let path = this.request.path;
        if (this.useBlobURIs) {
            const { content } = await this.file(path);
            path = content;
            this.prog = null;
            this.request.timeout = null;
        }
        return new Worker(path, { type });
    }
}
export class Fetcher {
    fetcher;
    useBlobURIs;
    constructor(fetcher, useBlobURIs) {
        this.fetcher = fetcher;
        this.useBlobURIs = useBlobURIs;
        const antiforgeryToken = getInput("input[name=__RequestVerificationToken]");
        if (antiforgeryToken) {
            this.fetcher.setRequestVerificationToken(antiforgeryToken.value);
        }
    }
    get(path, handler) {
        return new RequestBuilder(this.fetcher, this.useBlobURIs, "GET", path, handler);
    }
    post(path, handler) {
        return new RequestBuilder(this.fetcher, this.useBlobURIs, "POST", path, handler);
    }
    head(path, handler) {
        return new RequestBuilder(this.fetcher, this.useBlobURIs, "HEAD", path, handler);
    }
}
