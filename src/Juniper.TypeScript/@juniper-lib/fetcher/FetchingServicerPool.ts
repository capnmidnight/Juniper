import { IFetchingService } from "@juniper-lib/fetcher-base/IFetchingService";
import { IRequest, IRequestWithBody } from "@juniper-lib/fetcher-base/IRequest";
import { IResponse } from "@juniper-lib/fetcher-base/IResponse";
import { IProgress } from "@juniper-lib/tslib";
import type { FullWorkerClientOptions, WorkerClient, WorkerConstructorT } from "@juniper-lib/worker-client";
import { WorkerPool } from "@juniper-lib/worker-client";
import { FetchingServiceClient } from "./FetchingServiceClient";

export abstract class BaseFetchingServicePool<
    EventsT,
    FetcherWorkerClientT extends WorkerClient<EventsT> & IFetchingService>
    extends WorkerPool<EventsT, FetcherWorkerClientT>
    implements IFetchingService {

    constructor(
        options: FullWorkerClientOptions,
        WorkerClientClass: WorkerConstructorT<EventsT, FetcherWorkerClientT>,
        private readonly fetcher: IFetchingService) {
        super(options, WorkerClientClass);
    }

    private getFetcher(obj: any): IFetchingService {
        if (obj instanceof FormData) {
            return this.fetcher;
        }
        else {
            return this.nextWorker();
        }
    }

    setRequestVerificationToken(value: string): void {
        this.fetcher.setRequestVerificationToken(value);
        for (const worker of this.workers) {
            worker.setRequestVerificationToken(value);
        }
    }

    async clearCache(): Promise<void> {
        await Promise.all(this.workers.map(w => w.clearCache()));
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

    sendNothingGetObject<T>(request: IRequest, progress: IProgress): Promise<T> {
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
        return this.getFetcher(request.body).sendObjectGetBlob(request, progress);
    }

    sendObjectGetBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.getFetcher(request.body).sendObjectGetBuffer(request, progress);
    }

    sendObjectGetFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.getFetcher(request.body).sendObjectGetFile(request, progress);
    }

    sendObjectGetText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.getFetcher(request.body).sendObjectGetText(request, progress);
    }

    sendObjectGetNothing(request: IRequestWithBody, progress: IProgress): Promise<IResponse> {
        return this.getFetcher(request.body).sendObjectGetNothing(request, progress);
    }

    sendObjectGetObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.getFetcher(request.body).sendObjectGetObject(request, progress);
    }

    sendObjectGetXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.getFetcher(request.body).sendObjectGetXml(request, progress);
    }

    sendObjectGetImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.getFetcher(request.body).sendObjectGetImageBitmap(request, progress);
    }
}

export class FetchingServicePool
    extends BaseFetchingServicePool<void, FetchingServiceClient>{
    // no additional functionality
}