import { FetchingServiceImpl } from "juniper-fetcher-base";
import { WorkerPool } from "juniper-tslib";
export class BaseFetchingServicePool extends WorkerPool {
    fetcher = new FetchingServiceImpl();
    constructor(options, WorkerClientClass) {
        super(options, WorkerClientClass);
    }
    getFetcher(obj) {
        if (obj instanceof FormData) {
            return this.fetcher;
        }
        else {
            return this.nextWorker();
        }
    }
    setRequestVerificationToken(value) {
        this.fetcher.setRequestVerificationToken(value);
        for (const worker of this.workers) {
            worker.setRequestVerificationToken(value);
        }
    }
    head(request) {
        return this.nextWorker().head(request);
    }
    getBlob(request, progress) {
        return this.nextWorker().getBlob(request, progress);
    }
    getBuffer(request, progress) {
        return this.nextWorker().getBuffer(request, progress);
    }
    getFile(request, progress) {
        return this.nextWorker().getFile(request, progress);
    }
    getText(request, progress) {
        return this.nextWorker().getText(request, progress);
    }
    getObject(request, progress) {
        return this.nextWorker().getObject(request, progress);
    }
    getXml(request, progress) {
        return this.nextWorker().getXml(request, progress);
    }
    getImageBitmap(request, progress) {
        return this.nextWorker().getImageBitmap(request, progress);
    }
    postObjectForBlob(request, progress) {
        return this.getFetcher(request.body).postObjectForBlob(request, progress);
    }
    postObjectForBuffer(request, progress) {
        return this.getFetcher(request.body).postObjectForBuffer(request, progress);
    }
    postObjectForFile(request, progress) {
        return this.getFetcher(request.body).postObjectForFile(request, progress);
    }
    postObjectForText(request, progress) {
        return this.getFetcher(request.body).postObjectForText(request, progress);
    }
    postObject(request, progress) {
        return this.getFetcher(request.body).postObject(request, progress);
    }
    postObjectForObject(request, progress) {
        return this.getFetcher(request.body).postObjectForObject(request, progress);
    }
    postObjectForXml(request, progress) {
        return this.getFetcher(request.body).postObjectForXml(request, progress);
    }
    postObjectForImageBitmap(request, progress) {
        return this.getFetcher(request.body).postObjectForImageBitmap(request, progress);
    }
}
export class FetchingServicePool extends BaseFetchingServicePool {
}
