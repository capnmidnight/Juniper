import { blobToObjectURL } from "@juniper-lib/util";
import { Fetcher, FetchingService, FetchingServiceImpl } from "@juniper-lib/fetcher";
import { Text_Plain } from "@juniper-lib/mediatypes";
import { TestCase } from "@juniper-lib/testing";

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
