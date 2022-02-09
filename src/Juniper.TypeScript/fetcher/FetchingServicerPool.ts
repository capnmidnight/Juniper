import { FetchingServiceImpl } from "juniper-fetcher-base/FetchingServiceImpl";
import type { IFetchingService, IRequest, IRequestWithBody, IResponse } from "juniper-fetcher-base/IFetcher";
import { IProgress } from "juniper-tslib";
import type { FullWorkerClientOptions, WorkerClient, WorkerConstructorT } from "juniper-worker-client";
import { WorkerPool } from "juniper-worker-client";
import { FetchingServiceClient } from "./FetchingServiceClient";

export abstract class BaseFetchingServicePool<EventsT, FetcherWorkerClientT extends WorkerClient<EventsT> & IFetchingService>
    extends WorkerPool<EventsT, FetcherWorkerClientT>
    implements IFetchingService {

    private readonly fetcher = new FetchingServiceImpl();

    constructor(options: FullWorkerClientOptions, WorkerClientClass: WorkerConstructorT<EventsT, FetcherWorkerClientT>) {
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

    head(request: IRequest): Promise<IResponse<void>> {
        return this.nextWorker().head(request);
    }

    getBlob(request: IRequest, progress: IProgress): Promise<IResponse<Blob>> {
        return this.nextWorker().getBlob(request, progress);
    }

    getBuffer(request: IRequest, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.nextWorker().getBuffer(request, progress);
    }

    getFile(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.nextWorker().getFile(request, progress);
    }

    getText(request: IRequest, progress: IProgress): Promise<IResponse<string>> {
        return this.nextWorker().getText(request, progress);
    }

    getObject<T>(request: IRequest, progress: IProgress): Promise<T> {
        return this.nextWorker().getObject(request, progress);
    }

    getXml(request: IRequest, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.nextWorker().getXml(request, progress);
    }

    getImageBitmap(request: IRequest, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.nextWorker().getImageBitmap(request, progress);
    }

    postObjectForBlob(request: IRequestWithBody, progress: IProgress): Promise<IResponse<Blob>> {
        return this.getFetcher(request.body).postObjectForBlob(request, progress);
    }

    postObjectForBuffer(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ArrayBuffer>> {
        return this.getFetcher(request.body).postObjectForBuffer(request, progress);
    }

    postObjectForFile(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.getFetcher(request.body).postObjectForFile(request, progress);
    }

    postObjectForText(request: IRequestWithBody, progress: IProgress): Promise<IResponse<string>> {
        return this.getFetcher(request.body).postObjectForText(request, progress);
    }

    postObject(request: IRequestWithBody, progress: IProgress): Promise<IResponse<void>> {
        return this.getFetcher(request.body).postObject(request, progress);
    }

    postObjectForObject<T>(request: IRequestWithBody, progress: IProgress): Promise<T> {
        return this.getFetcher(request.body).postObjectForObject(request, progress);
    }

    postObjectForXml(request: IRequestWithBody, progress: IProgress): Promise<IResponse<HTMLElement>> {
        return this.getFetcher(request.body).postObjectForXml(request, progress);
    }

    postObjectForImageBitmap(request: IRequestWithBody, progress: IProgress): Promise<IResponse<ImageBitmap>> {
        return this.getFetcher(request.body).postObjectForImageBitmap(request, progress);
    }
}

export class FetchingServicePool
    extends BaseFetchingServicePool<void, FetchingServiceClient>{
    // no additional functionality
}