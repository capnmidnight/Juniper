import { WorkerServer } from "juniper-tslib";
import { FetchingServiceImpl } from "./FetchingServiceImpl";
import type { IFetchingService, IResponse } from "./IFetcher";

export class FetchingServiceServer extends WorkerServer {
    constructor(self: DedicatedWorkerGlobalScope) {
        super(self);

        const fetcher = new FetchingServiceImpl();
        addFetcherMethods(this, fetcher);
    }
}

function getContent<T extends Transferable>(response: IResponse<T>): Array<Transferable> {
    return [response.content];
}

export function addFetcherMethods(server: WorkerServer, fetcher: IFetchingService) {
    server.addVoidMethod("setRequestVerificationToken", fetcher, fetcher.setRequestVerificationToken);

    server.addMethod("getBuffer", fetcher, fetcher.getBuffer, getContent);
    server.addMethod("postObjectForBuffer", fetcher, fetcher.postObjectForBuffer, getContent);
    server.addMethod("getImageBitmap", fetcher, fetcher.getImageBitmap, getContent);
    server.addMethod("postObjectForImageBitmap", fetcher, fetcher.postObjectForImageBitmap, getContent);

    server.addMethod("getObject", fetcher, fetcher.getObject);
    server.addMethod("getFile", fetcher, fetcher.getFile);
    server.addMethod("getText", fetcher, fetcher.getText);
    server.addMethod("postObject", fetcher, fetcher.postObject);
    server.addMethod("postObjectForObject", fetcher, fetcher.postObjectForObject);
    server.addMethod("postObjectForFile", fetcher, fetcher.postObjectForFile);
    server.addMethod("postObjectForText", fetcher, fetcher.postObjectForText);

    server.addMethod("head", fetcher, fetcher.head);
}
