import { IResponse, isDefined, isObject, isString } from "@juniper-lib/util";
import { IProgress } from "@juniper-lib/progress";
import { IFetchingService } from "./IFetchingService";
import { IFetchingServiceImpl } from "./IFetchingServiceImpl";
import { IRequest, IRequestWithBody } from "./IRequest";
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
function maybeConvertDate(value: string): Date | string {
    let maybeDate: Date | string = value;

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
function marshalData<T>(response: IResponse<T>): IResponse<T> {
    if (isObject(response.content)) {
        if (isString(response.content)) {
            response.content = maybeConvertDate(response.content) as any;
        }
        else {
            const values = new Map<string, object>();
            const visited = new Set<object>();
            const queue = new Array<object>(response.content);
            while (queue.length > 0) {
                const here = queue.shift();
                if (isDefined(here) && !visited.has(here)) {
                    visited.add(here);
                    if (isObject(here) && "$id" in here) {
                        const id = here["$id"] as string;
                        values.set(id, here);
                    }

                    // Object.entires will correctly handle both objects and arrays.
                    for (const [key, value] of Object.entries(here)) {
                        if (isString(value)) {
                            (here as any)[key] = maybeConvertDate(value);
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
                                const id = value["$ref"] as string;
                                (here as any)[key] = value = values.get(id);
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


export class FetchingService implements IFetchingService {

    readonly #impl: IFetchingServiceImpl

    constructor(impl: IFetchingServiceImpl) {
        this.#impl = impl;
    }

    protected readonly defaultPostHeaders = new Map<string, string>();

    setRequestVerificationToken(value: string): void {
        this.defaultPostHeaders.set("RequestVerificationToken", value);
    }

    clearCache(): Promise<void> {
        return this.#impl.clearCache();
    }

    evict(path: string): Promise<void> {
        return this.#impl.evict(path);
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.#impl.sendNothingGetNothing(request);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.#impl.sendNothingGetSomething("blob", request, progress);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.#impl.sendSomethingGetSomething("blob", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.#impl.sendNothingGetSomething("arraybuffer", request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.#impl.sendSomethingGetSomething("arraybuffer", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.#impl.sendNothingGetSomething("text", request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.#impl.sendSomethingGetSomething("text", request, this.defaultPostHeaders, progress);
    }

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.#impl.sendNothingGetSomething<"json", T>("json", request, progress).then(marshalData);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.#impl.sendSomethingGetSomething<"json", T>("json", request, this.defaultPostHeaders, progress).then(marshalData);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.#impl.sendSomethingGetSomething("", request, this.defaultPostHeaders, progress);
    }

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return this.#impl.drawImageToCanvas(request, canvas, progress);
    }

    async sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return translateResponse(
            await this.sendNothingGetBlob(request, progress),
            URL.createObjectURL);
    }

    async sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return translateResponse(
            await this.sendObjectGetBlob(request, progress),
            URL.createObjectURL);
    }

    async sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return translateResponse(
            await this.#impl.sendNothingGetSomething("document", request, progress),
            (doc) => doc.documentElement);
    }

    async sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return translateResponse(
            await this.#impl.sendSomethingGetSomething("document", request, this.defaultPostHeaders, progress),
            (doc) => doc.documentElement);
    }

    async sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendNothingGetBlob(request, progress),
            createImageBitmap);
    }

    async sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return translateResponse(
            await this.sendObjectGetBlob(request, progress),
            createImageBitmap);
    }
}
