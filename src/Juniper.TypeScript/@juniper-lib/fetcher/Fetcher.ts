import { getInput } from "@juniper-lib/dom/tags";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { progressTasksWeighted } from "@juniper-lib/progress/progressTasks";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { BaseAsset, isAsset } from "./Asset";
import { HTTPMethods } from "./HTTPMethods";
import { IFetcher } from "./IFetcher";
import { IFetchingService } from "./IFetchingService";
import { RequestBuilder } from "./RequestBuilder";


declare const IS_WORKER: boolean;

export class Fetcher implements IFetcher {
    constructor(private readonly service: IFetchingService, private readonly useBLOBs = false) {
        if (!IS_WORKER) {
            const antiforgeryToken = getInput("input[name=__RequestVerificationToken]");
            if (antiforgeryToken) {
                this.service.setRequestVerificationToken(antiforgeryToken.value);
            }
        }
    }

    clearCache(): Promise<void> {
        return this.service.clearCache();
    }

    evict(path: string | URL, base?: string | URL) {
        return this.service.evict(new URL(path, base || location.href).href);
    }

    request<T extends HTTPMethods>(method: T, path: string | URL, base?: string | URL) {
        return new RequestBuilder(
            this.service,
            method,
            new URL(path, base || location.href),
            this.useBLOBs);
    }

    head(path: string | URL, base?: string | URL) {
        return this.request("HEAD", path, base);
    }

    options(path: string | URL, base?: string | URL) {
        return this.request("OPTIONS", path, base);
    }

    get(path: string | URL, base?: string | URL) {
        return this.request("GET", path, base);
    }

    post(path: string | URL, base?: string | URL) {
        return this.request("POST", path, base);
    }

    put(path: string | URL, base?: string | URL) {
        return this.request("PUT", path, base);
    }

    patch(path: string | URL, base?: string | URL) {
        return this.request("PATCH", path, base);
    }

    delete(path: string | URL, base?: string | URL) {
        return this.request("DELETE", path, base);
    }

    async assets(firstAsset: BaseAsset, ...assets: BaseAsset[]): Promise<void>;
    async assets(progress: IProgress, firstAsset: BaseAsset, ...assets: BaseAsset[]): Promise<void>;
    async assets(progressOrAsset: IProgress | BaseAsset, firstAsset: BaseAsset, ...assets: BaseAsset[]): Promise<void> {

        if (isNullOrUndefined(assets)) {
            assets = [];
        }

        assets.unshift(firstAsset);

        let progress: IProgress;
        if (isAsset(progressOrAsset)) {
            assets.unshift(progressOrAsset);
        }
        else if(isDefined(progressOrAsset)) {
            progress = progressOrAsset;
        }

        assets = assets.filter(isDefined);
        const sizes = await Promise.all(assets
            .map((asset) =>
                asset.getSize(this)));
        const assetSizes = new Map(sizes);
        await progressTasksWeighted(
            progress,
            assets.map((asset) => [assetSizes.get(asset), (prog) => asset.fetch(this, prog)])
        );
    }
}

