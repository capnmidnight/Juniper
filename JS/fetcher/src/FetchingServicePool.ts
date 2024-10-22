import { IResponse } from "@juniper-lib/util";
import type { FullWorkerClientOptions } from "@juniper-lib/dom";
import { WorkerPool } from "@juniper-lib/dom";
import { TypedEventMap } from "@juniper-lib/events";
import { IProgress } from "@juniper-lib/progress";
import { FetchingServiceClient } from "./FetchingServiceClient";
import { IFetchingService } from "./IFetchingService";
import { IRequest, IRequestWithBody } from "./IRequest";

export class FetchingServicePool
    extends WorkerPool<TypedEventMap<string>, FetchingServiceClient>
    implements IFetchingService {

    readonly #fetcher: IFetchingService;

    constructor(options: FullWorkerClientOptions, fetcher: IFetchingService) {
        super(options, FetchingServiceClient);
        this.#fetcher = fetcher;
    }

    #getFetcher(obj: any): IFetchingService {
        if (obj instanceof FormData) {
            return this.#fetcher;
        }
        else {
            return this.nextWorker();
        }
    }

    setRequestVerificationToken(value: string): void {
        this.#fetcher.setRequestVerificationToken(value);
        for (const worker of this.workers) {
            worker.setRequestVerificationToken(value);
        }
    }

    async clearCache(): Promise<void> {
        await Promise.all(this.workers.map(w => w.clearCache()));
    }

    async evict(path: string): Promise<void> {
        await Promise.all(this.workers.map(w => w.evict(path)));
    }

    sendNothingGetNothing(request: IRequest): Promise<IResponse> {
        return this.nextWorker().sendNothingGetNothing(request);
    }

    sendNothingGetBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.nextWorker().sendNothingGetBlob(request, progress);
    }

    sendNothingGetBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.nextWorker().sendNothingGetBuffer(request, progress);
    }

    sendNothingGetFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.nextWorker().sendNothingGetFile(request, progress);
    }

    sendNothingGetText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.nextWorker().sendNothingGetText(request, progress);
    }

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<IResponse<T>> {
        return this.nextWorker().sendNothingGetObject(request, progress);
    }

    sendNothingGetXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.nextWorker().sendNothingGetXml(request, progress);
    }

    sendNothingGetImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.nextWorker().sendNothingGetImageBitmap(request, progress);
    }

    drawImageToCanvas(request: IRequest, canvas: OffscreenCanvas, progress: IProgress): Promise<IResponse> {
        return this.nextWorker().drawImageToCanvas(request, canvas, progress);
    }

    sendObjectGetBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.#getFetcher(request.body).sendObjectGetBlob(request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.#getFetcher(request.body).sendObjectGetBuffer(request, progress);
    }

    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.#getFetcher(request.body).sendObjectGetFile(request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.#getFetcher(request.body).sendObjectGetText(request, progress);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.#getFetcher(request.body).sendObjectGetNothing(request, progress);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<IResponse<T>> {
        return this.#getFetcher(request.body).sendObjectGetObject(request, progress);
    }

    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.#getFetcher(request.body).sendObjectGetXml(request, progress);
    }

    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.#getFetcher(request.body).sendObjectGetImageBitmap(request, progress);
    }
}
