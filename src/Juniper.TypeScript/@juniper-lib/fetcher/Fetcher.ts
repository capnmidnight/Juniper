import { getInput } from "@juniper-lib/dom/tags";
import { Asset } from "@juniper-lib/fetcher-base/Asset";
import { HTTPMethods } from "@juniper-lib/fetcher-base/HTTPMethods";
import { IFetcher } from "@juniper-lib/fetcher-base/IFetcher";
import { IFetchingService } from "@juniper-lib/fetcher-base/IFetchingService";
import { IProgress, isDefined, isWorker, progressTasksWeighted } from "@juniper-lib/tslib";
import { RequestBuilder } from "./RequestBuilder";



export class Fetcher implements IFetcher {
    constructor(private readonly service: IFetchingService, private readonly useFileBlobsForModules: boolean = true) {
        if (!isWorker) {
            const antiforgeryToken = getInput("input[name=__RequestVerificationToken]");
            if (antiforgeryToken) {
                this.service.setRequestVerificationToken(antiforgeryToken.value);
            }
        }
    }

    private createRequest(method: HTTPMethods, path: string | URL, base?: string | URL) {
        return new RequestBuilder(this.service, this.useFileBlobsForModules, method, new URL(path, base || location.href));
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

    async assets(progress: IProgress, ...assets: Asset<any, any>[]): Promise<void> {
        assets = assets.filter(isDefined);
        const assetSizes = new Map(await Promise.all(assets.map((asset) => asset.getSize(this))));
        await progressTasksWeighted(
            progress,
            assets.map((asset) => [assetSizes.get(asset), (prog) => asset.getContent(prog)])
        );
    }
}

