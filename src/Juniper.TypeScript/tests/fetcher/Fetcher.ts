import { Fetcher } from "juniper-fetcher/Fetcher";
import { FetchingService, FetchingServiceImplXHR as FetchingServiceImpl } from "juniper-fetcher/impl";
import { makeBlobURL, makeTextBlob } from "juniper-mediatypes";
import { Text_Plain } from "juniper-mediatypes/text";
import { TestCase } from "juniper-tdd/tdd/TestCase";

export class FetcherTests extends TestCase {

    async test_Blob() {
        const fetcher = new Fetcher(new FetchingService(new FetchingServiceImpl()));
        const text = "Hello, world";
        const textBlob = makeTextBlob(text);
        const textURL = makeBlobURL(textBlob);
        const response = await fetcher
            .get(textURL)
            .text();
        this.areExact(response.contentType, Text_Plain.value);
        this.areExact(response.content, text);
    }
}
