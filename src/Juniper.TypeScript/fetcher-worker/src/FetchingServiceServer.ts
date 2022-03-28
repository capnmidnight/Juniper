import { FetchingService } from "juniper-fetcher-base/FetchingService";
import { IFetchingService } from "juniper-fetcher-base/IFetchingService";
import { IFetchingServiceImpl } from "juniper-fetcher-base/IFetchingServiceImpl";
import { IResponse } from "juniper-fetcher-base/IResponse";
import { WorkerServer } from "juniper-worker-server";

export class FetchingServiceServer extends WorkerServer {
    constructor(self: DedicatedWorkerGlobalScope, impl: IFetchingServiceImpl) {
        super(self);
        const fetcher = new FetchingService(impl);
        addFetcherMethods(this, fetcher);
    }
}

function getContent<T extends Transferable>(response: IResponse<T>): Array<Transferable> {
    return [response.content];
}

export function addFetcherMethods(server: WorkerServer, fetcher: IFetchingService) {
    server.addVoidMethod("setRequestVerificationToken", fetcher, fetcher.setRequestVerificationToken);

    server.addMethod("sendNothingGetNothing", fetcher, fetcher.sendNothingGetNothing);

    server.addMethod("sendNothingGetBuffer", fetcher, fetcher.sendNothingGetBuffer, getContent);
    server.addMethod("sendNothingGetImageBitmap", fetcher, fetcher.sendNothingGetImageBitmap, getContent);
    server.addMethod("sendNothingGetObject", fetcher, fetcher.sendNothingGetObject);
    server.addMethod("sendNothingGetFile", fetcher, fetcher.sendNothingGetFile);
    server.addMethod("sendNothingGetText", fetcher, fetcher.sendNothingGetText);

    server.addMethod("sendObjectGetNothing", fetcher, fetcher.sendObjectGetNothing);

    server.addMethod("sendObjectGetImageBitmap", fetcher, fetcher.sendObjectGetImageBitmap, getContent);
    server.addMethod("sendObjectGetBuffer", fetcher, fetcher.sendObjectGetBuffer, getContent);
    server.addMethod("sendObjectGetObject", fetcher, fetcher.sendObjectGetObject);
    server.addMethod("sendObjectGetFile", fetcher, fetcher.sendObjectGetFile);
    server.addMethod("sendObjectGetText", fetcher, fetcher.sendObjectGetText);
}
