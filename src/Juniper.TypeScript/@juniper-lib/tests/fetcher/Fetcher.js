import { Fetcher } from "@juniper-lib/fetcher/Fetcher";
import { FetchingService } from "@juniper-lib/fetcher/FetchingService";
import { FetchingServiceImplXHR as FetchingServiceImpl } from "@juniper-lib/fetcher/FetchingServiceImplXHR";
import { Text_Plain } from "@juniper-lib/mediatypes";
import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { blobToObjectURL } from "@juniper-lib/tslib/blobToObjectURL";
export class FetcherTests extends TestCase {
    async test_Blob() {
        const fetcher = new Fetcher(new FetchingService(new FetchingServiceImpl()));
        const text = "Hello, world";
        const textBlob = new Blob([text], { type: "text/plain" });
        const textURL = blobToObjectURL(textBlob);
        const response = await fetcher
            .get(textURL)
            .text();
        this.areExact(response.contentType, Text_Plain.value);
        this.areExact(response.content, text);
    }
}
//# sourceMappingURL=Fetcher.js.map