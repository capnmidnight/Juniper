import { Fetcher } from "@juniper-lib/fetcher/dist/Fetcher";
import { FetchingService } from "@juniper-lib/fetcher/dist/FetchingService";
import { FetchingServiceImplXHR } from "@juniper-lib/fetcher/dist/FetchingServiceImplXHR";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import { haxClass } from "@juniper-lib/hax/dist/haxClass";
import { haxMethod } from "@juniper-lib/hax/dist/haxMethod";
import { TestCase } from "@juniper-lib/testing/dist/tdd/TestCase";
import { using, usingAsync } from "@juniper-lib/tslib/dist/using";

export class haxTests extends TestCase {
    test_Method() {
        const oldLog = console.log;
        const values: any[] = [];
        using(haxMethod(console, console.log, "log", values.push.bind(values)), () => {
            this.areDifferent(console.log, oldLog);
            console.log("Hello", "world");
            this.areExact("Hello", values[0]);
            this.areExact("world", values[1]);
        });

        this.areExact(console.log, oldLog);
    }

    async test_Class() {
        const fetcher = new Fetcher(new FetchingService(new FetchingServiceImplXHR()));
        const oldURL = window.URL;
        const values = new Array<any>();
        const blob = new Blob(["asdf"], { type: "text/plain" });
        const x = URL.createObjectURL(blob);
        await usingAsync(haxClass(window, URL, "URL", values.push.bind(values)), async () => {
            this.areDifferent(window.URL, oldURL);
            const y = URL.createObjectURL(blob);

            const url = new URL("images", "https://www.seanmcbeth.com/");
            this.areExact(url.href, "https://www.seanmcbeth.com/images");
            this.areExact("images", values[0]);
            this.areExact("https://www.seanmcbeth.com/", values[1]);

            const xText = await fetcher
                .get(x)
                .text()
                .then(unwrapResponse);
            const yText = await fetcher
                .get(y)
                .text()
                .then(unwrapResponse);
            this.areExact(yText, xText);
        });
        this.areSame(window.URL, oldURL);
    }
}