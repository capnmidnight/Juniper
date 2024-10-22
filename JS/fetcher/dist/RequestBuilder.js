import { assertNever, dispose, isDefined, isFunction, isString } from "@juniper-lib/util";
import { Img, Link, Rel, Script, Type, createCanvas, createOffscreenCanvas, drawImageToCanvas, hasOffscreenCanvas } from "@juniper-lib/dom";
import { once, waitFor } from "@juniper-lib/events";
import { Application_Javascript, Application_Json, Application_Wasm, Text_Css, Text_Plain, Text_Xml } from "@juniper-lib/mediatypes";
import { translateResponse } from "./translateResponse";
// This HTML Audio Element is just used to get access to the
// canPlayType method. It is not used for any other functionality.
// This is to get around some of the crappy problems various
// browsers have with being able to report the types of media
// they can play.
let testAudio = null;
function canPlay(type) {
    if (testAudio === null) {
        testAudio = new Audio();
    }
    return testAudio.canPlayType(type) !== "";
}
export class RequestBuilder {
    #fetcher;
    #method;
    #path;
    #useBLOBs;
    #request;
    #prog = null;
    constructor(fetcher, method, path, useBLOBs = false) {
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
    retries(count) {
        this.#request.retryCount = count;
        return this;
    }
    query(name, value) {
        this.#path.searchParams.set(name, value);
        this.#request.path = this.#path.href;
        return this;
    }
    header(name, value) {
        if (this.#request.headers === null) {
            this.#request.headers = new Map();
        }
        this.#request.headers.set(name.toLowerCase(), value);
        return this;
    }
    headers(headers) {
        for (const [name, value] of headers.entries()) {
            this.header(name, value);
        }
        return this;
    }
    timeout(value) {
        this.#request.timeout = value;
        return this;
    }
    progress(prog) {
        this.#prog = prog;
        return this;
    }
    body(body, contentType) {
        if (isDefined(body)) {
            const seen = new Set();
            const queue = new Array();
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
                const fileNames = new Map();
                const toSkip = new Set();
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
    #media(key, mediaType) {
        if (isDefined(mediaType)) {
            if (!isString(mediaType)) {
                mediaType = mediaType.value;
            }
            this.header(key, mediaType);
        }
    }
    #content(contentType) {
        this.#media("content-type", contentType);
    }
    accept(acceptType) {
        this.#media("accept", acceptType);
        return this;
    }
    blob(acceptType) {
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
    buffer(acceptType) {
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
    async file(acceptType) {
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
    async dataUri(acceptType) {
        const response = await this.blob(acceptType);
        const reader = new FileReader();
        const task = new Promise((resolve) => reader.addEventListener("loadend", (evt) => resolve(evt.target.result)));
        reader.readAsDataURL(response.content);
        const value = await task;
        return await translateResponse(response, () => value);
    }
    text(acceptType) {
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
    object(acceptType) {
        this.accept(acceptType || Application_Json);
        if (this.#method === "POST"
            || this.#method === "PUT"
            || this.#method === "PATCH"
            || this.#method === "DELETE") {
            return this.#fetcher.sendObjectGetObject(this.#request, this.#prog);
        }
        else if (this.#method === "GET") {
            return this.#fetcher.sendNothingGetObject(this.#request, this.#prog);
        }
        else if (this.#method === "HEAD"
            || this.#method === "OPTIONS") {
            throw new Error(`${this.#method} responses do not contain bodies`);
        }
        else {
            assertNever(this.#method);
        }
    }
    xml(acceptType) {
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
    imageBitmap(acceptType) {
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
    async #audioBlob(acceptType) {
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
    async audioBuffer(context, acceptType) {
        return translateResponse(await this.#audioBlob(acceptType), async (blob) => await context.decodeAudioData(await blob.arrayBuffer()));
    }
    async #htmlElement(element, resolveEvt, acceptType) {
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
    image(acceptType) {
        return this.#htmlElement(Img(), "load", acceptType);
    }
    async htmlCanvas(acceptType) {
        if (IS_WORKER) {
            throw new Error("HTMLCanvasElement not supported in Workers.");
        }
        const canvas = createCanvas(1, 1);
        if (this.#method === "GET") {
            if (hasOffscreenCanvas) {
                this.accept(acceptType);
                const response = await this.#fetcher.drawImageToCanvas(this.#request, canvas.transferControlToOffscreen(), this.#prog);
                return await translateResponse(response, () => canvas);
            }
            else {
                const response = await (IS_WORKER
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
    canvas(acceptType) {
        if (hasOffscreenCanvas) {
            return this.offscreenCanvas(acceptType);
        }
        else {
            return this.htmlCanvas(acceptType);
        }
    }
    async offscreenCanvas(acceptType) {
        if (!hasOffscreenCanvas) {
            throw new Error("This system does not support OffscreenCanvas");
        }
        if (this.#method === "GET") {
            const response = await (IS_WORKER
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
    async style() {
        const tag = Link(Type(Text_Css), Rel("stylesheet"));
        document.head.append(tag);
        const response = await this.#htmlElement(tag, "load", Text_Css);
        return translateResponse(response);
    }
    async #getScript() {
        const tag = Script(Type(Application_Javascript));
        document.head.append(tag);
        const response = await this.#htmlElement(tag, "load", Application_Javascript);
        return translateResponse(response);
    }
    async script(test) {
        let response = null;
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
    async module() {
        const scriptPath = this.#request.path;
        const response = await this.file(Application_Javascript);
        const value = await import(response.content);
        if (this.#prog) {
            this.#prog.end(scriptPath);
        }
        return translateResponse(response, () => value);
    }
    async wasm(imports) {
        const response = await this.buffer(Application_Wasm);
        if (!Application_Wasm.matches(response.contentType)) {
            throw new Error(`Server did not respond with WASM file. Was: ${response.contentType}`);
        }
        const module = await WebAssembly.compile(response.content);
        const instance = await WebAssembly.instantiate(module, imports);
        return translateResponse(response, () => instance.exports);
    }
    async worker(type = "module") {
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
//# sourceMappingURL=RequestBuilder.js.map