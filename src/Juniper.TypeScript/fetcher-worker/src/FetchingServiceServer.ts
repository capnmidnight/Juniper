import { FetchingService } from "@juniper-lib/fetcher-base/FetchingService";
import type { IFetchingService } from "@juniper-lib/fetcher-base/IFetchingService";
import type { IFetchingServiceImpl } from "@juniper-lib/fetcher-base/IFetchingServiceImpl";
import type { IResponse } from "@juniper-lib/fetcher-base/IResponse";
import { WorkerServer } from "@juniper-lib/worker-server";

export class FetchingServiceServer extends WorkerServer {
    constructor(self: DedicatedWorkerGlobalScope, impl: IFetchingServiceImpl) {
        super(self);
        const fetcher = new FetchingService(impl);
        addFetcherMethods(this, fetcher);
    }
}

function getContent<T extends Transferable | OffscreenCanvas>(response: IResponse<T>): Array<Transferable | OffscreenCanvas> {
    return [response.content];
}

export function addFetcherMethods(server: WorkerServer, fetcher: IFetchingService) {
    server.addVoidMethod(fetcher, "setRequestVerificationToken", fetcher.setRequestVerificationToken);

    server.addMethod(fetcher, "clearCache", fetcher.clearCache);

    server.addMethod(fetcher, "sendNothingGetNothing", fetcher.sendNothingGetNothing);

    server.addMethod(fetcher, "sendNothingGetBuffer", fetcher.sendNothingGetBuffer, getContent);
    server.addMethod(fetcher, "sendNothingGetImageBitmap", fetcher.sendNothingGetImageBitmap, getContent);
    server.addMethod(fetcher, "sendNothingGetObject", fetcher.sendNothingGetObject);
    server.addMethod(fetcher, "sendNothingGetFile", fetcher.sendNothingGetFile);
    server.addMethod(fetcher, "sendNothingGetText", fetcher.sendNothingGetText);

    server.addMethod(fetcher, "sendObjectGetNothing", fetcher.sendObjectGetNothing);

    server.addMethod(fetcher, "sendObjectGetImageBitmap", fetcher.sendObjectGetImageBitmap, getContent);
    server.addMethod(fetcher, "sendObjectGetBuffer", fetcher.sendObjectGetBuffer, getContent);
    server.addMethod(fetcher, "sendObjectGetObject", fetcher.sendObjectGetObject);
    server.addMethod(fetcher, "sendObjectGetFile", fetcher.sendObjectGetFile);
    server.addMethod(fetcher, "sendObjectGetText", fetcher.sendObjectGetText);
}
