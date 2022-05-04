import { getInput } from "@juniper/dom/tags";
import { HTTPMethods } from "@juniper/fetcher-base/HTTPMethods";
import { IFetcher } from "@juniper/fetcher-base/IFetcher";
import { IFetchingService } from "@juniper/fetcher-base/IFetchingService";
import { RequestBuilder } from "./RequestBuilder";

export class Fetcher implements IFetcher {
    constructor(private readonly service: IFetchingService) {
        const antiforgeryToken = getInput("input[name=__RequestVerificationToken]");
        if (antiforgeryToken) {
            this.service.setRequestVerificationToken(antiforgeryToken.value);
        }
    }

    private createRequest(method: HTTPMethods, path: string | URL, base?: string | URL) {
        return new RequestBuilder(this.service, method, new URL(path, base || location.href));
    }

    clearCache(): Promise<void> {
        return this.service.clearCache();
    }

    head(path: string | URL, base?: string | URL) {
        return this.createRequest("HEAD", path, base);
    }

    options(path: string | URL, base?: string | URL) {
        return this.createRequest("OPTIONS", path, base);
    }

    get(path: string | URL, base?: string | URL) {
        return this.createRequest("GET", path, base);
    }

    post(path: string | URL, base?: string | URL) {
        return this.createRequest("POST", path, base);
    }

    put(path: string | URL, base?: string | URL) {
        return this.createRequest("PUT", path, base);
    }

    patch(path: string | URL, base?: string | URL) {
        return this.createRequest("PATCH", path, base);
    }

    delete(path: string | URL, base?: string | URL) {
        return this.createRequest("DELETE", path, base);
    }
}

