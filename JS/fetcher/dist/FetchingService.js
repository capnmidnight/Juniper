import { translateResponse } from "./translateResponse";
export class FetchingService {
    constructor(impl) {
        this.impl = impl;
        this.defaultPostHeaders = new Map();
    }
    setRequestVerificationToken(value) {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }
    clearCache() {
        return this.impl.clearCache();
    }
    evict(path) {
        return this.impl.evict(path);
    }
    sendNothingGetNothing(request) {
        return this.impl.sendNothingGetNothing(request);
    }
    sendNothingGetBlob(request, progress) {
        return this.impl.sendNothingGetSomething("blob", request, progress);
    }
    sendObjectGetBlob(request, progress) {
        return this.impl.sendSomethingGetSomething("blob", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetBuffer(request, progress) {
        return this.impl.sendNothingGetSomething("arraybuffer", request, progress);
    }
    sendObjectGetBuffer(request, progress) {
        return this.impl.sendSomethingGetSomething("arraybuffer", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetText(request, progress) {
        return this.impl.sendNothingGetSomething("text", request, progress);
    }
    sendObjectGetText(request, progress) {
        return this.impl.sendSomethingGetSomething("text", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetObject(request, progress) {
        return this.impl.sendNothingGetSomething("json", request, progress);
    }
    sendObjectGetObject(request, progress) {
        return this.impl.sendSomethingGetSomething("json", request, this.defaultPostHeaders, progress);
    }
    sendObjectGetNothing(request, progress) {
        return this.impl.sendSomethingGetSomething("", request, this.defaultPostHeaders, progress);
    }
    drawImageToCanvas(request, canvas, progress) {
        return this.impl.drawImageToCanvas(request, canvas, progress);
    }
    async sendNothingGetFile(request, progress) {
        return translateResponse(await this.sendNothingGetBlob(request, progress), URL.createObjectURL);
    }
    async sendObjectGetFile(request, progress) {
        return translateResponse(await this.sendObjectGetBlob(request, progress), URL.createObjectURL);
    }
    async sendNothingGetXml(request, progress) {
        return translateResponse(await this.impl.sendNothingGetSomething("document", request, progress), (doc) => doc.documentElement);
    }
    async sendObjectGetXml(request, progress) {
        return translateResponse(await this.impl.sendSomethingGetSomething("document", request, this.defaultPostHeaders, progress), (doc) => doc.documentElement);
    }
    async sendNothingGetImageBitmap(request, progress) {
        return translateResponse(await this.sendNothingGetBlob(request, progress), createImageBitmap);
    }
    async sendObjectGetImageBitmap(request, progress) {
        return translateResponse(await this.sendObjectGetBlob(request, progress), createImageBitmap);
    }
}
//# sourceMappingURL=FetchingService.js.map