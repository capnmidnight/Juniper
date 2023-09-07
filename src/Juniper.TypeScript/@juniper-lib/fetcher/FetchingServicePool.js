import { WorkerPool } from "@juniper-lib/workers/WorkerPool";
import { FetchingServiceClient } from "./FetchingServiceClient";
export class FetchingServicePool extends WorkerPool {
    constructor(options, fetcher) {
        super(options, FetchingServiceClient);
        this.fetcher = fetcher;
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
    async clearCache() {
        await Promise.all(this.workers.map(w => w.clearCache()));
    }
    async evict(path) {
        await Promise.all(this.workers.map(w => w.evict(path)));
    }
    sendNothingGetNothing(request) {
        return this.nextWorker().sendNothingGetNothing(request);
    }
    sendNothingGetBlob(request, progress) {
        return this.nextWorker().sendNothingGetBlob(request, progress);
    }
    sendNothingGetBuffer(request, progress) {
        return this.nextWorker().sendNothingGetBuffer(request, progress);
    }
    sendNothingGetFile(request, progress) {
        return this.nextWorker().sendNothingGetFile(request, progress);
    }
    sendNothingGetText(request, progress) {
        return this.nextWorker().sendNothingGetText(request, progress);
    }
    sendNothingGetObject(request, progress) {
        return this.nextWorker().sendNothingGetObject(request, progress);
    }
    sendNothingGetXml(request, progress) {
        return this.nextWorker().sendNothingGetXml(request, progress);
    }
    sendNothingGetImageBitmap(request, progress) {
        return this.nextWorker().sendNothingGetImageBitmap(request, progress);
    }
    drawImageToCanvas(request, canvas, progress) {
        return this.nextWorker().drawImageToCanvas(request, canvas, progress);
    }
    sendObjectGetBlob(request, progress) {
        return this.getFetcher(request.body).sendObjectGetBlob(request, progress);
    }
    sendObjectGetBuffer(request, progress) {
        return this.getFetcher(request.body).sendObjectGetBuffer(request, progress);
    }
    sendObjectGetFile(request, progress) {
        return this.getFetcher(request.body).sendObjectGetFile(request, progress);
    }
    sendObjectGetText(request, progress) {
        return this.getFetcher(request.body).sendObjectGetText(request, progress);
    }
    sendObjectGetNothing(request, progress) {
        return this.getFetcher(request.body).sendObjectGetNothing(request, progress);
    }
    sendObjectGetObject(request, progress) {
        return this.getFetcher(request.body).sendObjectGetObject(request, progress);
    }
    sendObjectGetXml(request, progress) {
        return this.getFetcher(request.body).sendObjectGetXml(request, progress);
    }
    sendObjectGetImageBitmap(request, progress) {
        return this.getFetcher(request.body).sendObjectGetImageBitmap(request, progress);
    }
}
//# sourceMappingURL=FetchingServicePool.js.map