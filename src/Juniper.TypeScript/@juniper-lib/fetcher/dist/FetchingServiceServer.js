import { WorkerServer } from "@juniper-lib/workers";
import { FetchingService } from "./FetchingService";
export class FetchingServiceServer extends WorkerServer {
    constructor(self, impl) {
        super(self);
        const fetcher = new FetchingService(impl);
        addFetcherMethods(this, fetcher);
    }
}
function getContent(response) {
    return [response.content];
}
export function addFetcherMethods(server, fetcher) {
    server.addVoidMethod(fetcher, "setRequestVerificationToken", fetcher.setRequestVerificationToken);
    server.addMethod(fetcher, "clearCache", fetcher.clearCache);
    server.addMethod(fetcher, "evict", fetcher.evict);
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
    server.addMethod(fetcher, "drawImageToCanvas", fetcher.drawImageToCanvas);
}
//# sourceMappingURL=FetchingServiceServer.js.map