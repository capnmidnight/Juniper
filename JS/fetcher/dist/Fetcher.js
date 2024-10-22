import { progressTasksWeighted } from "@juniper-lib/progress";
import { isDefined, isNullOrUndefined } from "@juniper-lib/util";
import { isAsset } from "./Asset";
import { RequestBuilder } from "./RequestBuilder";
import { Input, Query } from "@juniper-lib/dom";
export class Fetcher {
    #service;
    #useBLOBs;
    constructor(service, useBLOBs = false) {
        this.#service = service;
        this.#useBLOBs = useBLOBs;
        if (!IS_WORKER) {
            const antiforgeryToken = Input(Query("[name=__RequestVerificationToken]"));
            if (antiforgeryToken) {
                this.#service.setRequestVerificationToken(antiforgeryToken.value);
            }
        }
    }
    clearCache() {
        return this.#service.clearCache();
    }
    evict(path, base) {
        return this.#service.evict(new URL(path, base || location.href).href);
    }
    request(method, path, base) {
        return new RequestBuilder(this.#service, method, new URL(path, base || location.href), this.#useBLOBs);
    }
    head(path, base) {
        return this.request("HEAD", path, base);
    }
    options(path, base) {
        return this.request("OPTIONS", path, base);
    }
    get(path, base) {
        return this.request("GET", path, base);
    }
    post(path, base) {
        return this.request("POST", path, base);
    }
    put(path, base) {
        return this.request("PUT", path, base);
    }
    patch(path, base) {
        return this.request("PATCH", path, base);
    }
    delete(path, base) {
        return this.request("DELETE", path, base);
    }
    async assets(progressOrAsset, firstAsset, ...assets) {
        if (isNullOrUndefined(assets)) {
            assets = [];
        }
        assets.unshift(firstAsset);
        let progress;
        if (isAsset(progressOrAsset)) {
            assets.unshift(progressOrAsset);
        }
        else if (isDefined(progressOrAsset)) {
            progress = progressOrAsset;
        }
        assets = assets.filter(isDefined);
        const sizes = await Promise.all(assets
            .map((asset) => asset.getSize(this)));
        const assetSizes = new Map(sizes);
        await progressTasksWeighted(progress, assets.map((asset) => [assetSizes.get(asset), (prog) => asset.fetch(this, prog)]));
    }
}
//# sourceMappingURL=Fetcher.js.map