import { makeBlobURL } from "@juniper-lib/dom/makeBlobURL";
import { Fetcher } from "@juniper-lib/fetcher/Fetcher";
import { FetchingService, FetchingServiceImplXHR as FetchingServiceImpl } from "@juniper-lib/fetcher/impl";
import { Text_Plain } from "@juniper-lib/mediatypes";
import { TestCase } from "@juniper-lib/testing/tdd/TestCase";

export class FetcherTests extends TestCase {

    async test_Blob() {
        const fetcher = new Fetcher(new FetchingService(new FetchingServiceImpl()));
        const text = "Hello, world";
        const textBlob = new Blob([text], { type: "text/plain" });
        const textURL = makeBlobURL(textBlob);
        const response = await fetcher
            .get(textURL)
            .text();
        this.areExact(response.contentType, Text_Plain.value);
        this.areExact(response.content, text);
    }
}
