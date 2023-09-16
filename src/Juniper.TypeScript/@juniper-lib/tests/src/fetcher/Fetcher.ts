import { Fetcher } from "@juniper-lib/fetcher/dist/Fetcher";
import { FetchingService } from "@juniper-lib/fetcher/dist/FetchingService";
import { FetchingServiceImplXHR as FetchingServiceImpl } from "@juniper-lib/fetcher/dist/FetchingServiceImplXHR";
import { Text_Plain } from "@juniper-lib/mediatypes/dist";
import { TestCase } from "@juniper-lib/testing/dist/tdd/TestCase";
import { blobToObjectURL } from "@juniper-lib/tslib/dist/blobToObjectURL";

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
