import { makeBlobURL } from "@juniper/dom/makeBlobURL";
import { Fetcher } from "@juniper/fetcher/Fetcher";
import { FetchingService, FetchingServiceImplXHR as FetchingServiceImpl } from "@juniper/fetcher/impl";
import { TestCase } from "@juniper/tdd/tdd/TestCase";
import { Text_Plain } from "@juniper/tslib/mediatypes/text";

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
