import { isDefined, isObject, isString } from "@juniper-lib/util";
import { translateResponse } from "./translateResponse";
/**
 * Regular expressions used by maybeConvertDate below
 */
const datePatterns = [
    /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(:\d{2}(\.\d+)?)?(Z|[+\-]\d{2}:\d{2})?/,
    /\w+ \w+ \d{2} \d{4} \d{2}:\d{2}:\d{2} GMT[+\-]\d{4} \(\w+( \w+)*\)/
];
/**
 * Tests a string to see if it looks like a Date, returning
 * a parsed Date value if it does, or the original string
 * if it does not.
 * @param value
 * @returns
 */
function maybeConvertDate(value) {
    let maybeDate = value;
    if (datePatterns.some(pattern => pattern.test(value))) {
        maybeDate = new Date(value);
    }
    return maybeDate;
}
/**
 * Finds all the string fields in an object that look like Dates and
 * parses them to Date objects.
 * @param response
 * @returns
 */
function marshalData(response) {
    if (isObject(response.content)) {
        if (isString(response.content)) {
            response.content = maybeConvertDate(response.content);
        }
        else {
            const values = new Map();
            const visited = new Set();
            const queue = new Array(response.content);
            while (queue.length > 0) {
                const here = queue.shift();
                if (isDefined(here) && !visited.has(here)) {
                    visited.add(here);
                    if (isObject(here) && "$id" in here) {
                        const id = here["$id"];
                        values.set(id, here);
                    }
                    // Object.entires will correctly handle both objects and arrays.
                    for (const [key, value] of Object.entries(here)) {
                        if (isString(value)) {
                            here[key] = maybeConvertDate(value);
                        }
                        else if (isObject(value)) {
                            queue.push(value);
                        }
                    }
                }
            }
            visited.clear();
            queue.push(response.content);
            while (queue.length > 0) {
                const here = queue.shift();
                if (isDefined(here) && !visited.has(here)) {
                    visited.add(here);
                    for (let [key, value] of Object.entries(here)) {
                        if (isObject(value)) {
                            if ("$ref" in value) {
                                const id = value["$ref"];
                                here[key] = value = values.get(id);
                            }
                            queue.push(value);
                        }
                    }
                }
            }
        }
    }
    return response;
}
export class FetchingService {
    #impl;
    constructor(impl) {
        this.defaultPostHeaders = new Map();
        this.#impl = impl;
    }
    setRequestVerificationToken(value) {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }
    clearCache() {
        return this.#impl.clearCache();
    }
    evict(path) {
        return this.#impl.evict(path);
    }
    sendNothingGetNothing(request) {
        return this.#impl.sendNothingGetNothing(request);
    }
    sendNothingGetBlob(request, progress) {
        return this.#impl.sendNothingGetSomething("blob", request, progress);
    }
    sendObjectGetBlob(request, progress) {
        return this.#impl.sendSomethingGetSomething("blob", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetBuffer(request, progress) {
        return this.#impl.sendNothingGetSomething("arraybuffer", request, progress);
    }
    sendObjectGetBuffer(request, progress) {
        return this.#impl.sendSomethingGetSomething("arraybuffer", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetText(request, progress) {
        return this.#impl.sendNothingGetSomething("text", request, progress);
    }
    sendObjectGetText(request, progress) {
        return this.#impl.sendSomethingGetSomething("text", request, this.defaultPostHeaders, progress);
    }
    sendNothingGetObject(request, progress) {
        return this.#impl.sendNothingGetSomething("json", request, progress).then(marshalData);
    }
    sendObjectGetObject(request, progress) {
        return this.#impl.sendSomethingGetSomething("json", request, this.defaultPostHeaders, progress).then(marshalData);
    }
    sendObjectGetNothing(request, progress) {
        return this.#impl.sendSomethingGetSomething("", request, this.defaultPostHeaders, progress);
    }
    drawImageToCanvas(request, canvas, progress) {
        return this.#impl.drawImageToCanvas(request, canvas, progress);
    }
    async sendNothingGetFile(request, progress) {
        return translateResponse(await this.sendNothingGetBlob(request, progress), URL.createObjectURL);
    }
    async sendObjectGetFile(request, progress) {
        return translateResponse(await this.sendObjectGetBlob(request, progress), URL.createObjectURL);
    }
    async sendNothingGetXml(request, progress) {
        return translateResponse(await this.#impl.sendNothingGetSomething("document", request, progress), (doc) => doc.documentElement);
    }
    async sendObjectGetXml(request, progress) {
        return translateResponse(await this.#impl.sendSomethingGetSomething("document", request, this.defaultPostHeaders, progress), (doc) => doc.documentElement);
    }
    async sendNothingGetImageBitmap(request, progress) {
        return translateResponse(await this.sendNothingGetBlob(request, progress), createImageBitmap);
    }
    async sendObjectGetImageBitmap(request, progress) {
        return translateResponse(await this.sendObjectGetBlob(request, progress), createImageBitmap);
    }
}
//# sourceMappingURL=FetchingService.js.map