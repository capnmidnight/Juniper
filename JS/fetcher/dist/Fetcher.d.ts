import { IProgress } from "@juniper-lib/progress";
import { BaseAsset } from "./Asset";
import { HTTPMethods } from "./HTTPMethods";
import { IFetcher } from "./IFetcher";
import { IFetchingService } from "./IFetchingService";
import { RequestBuilder } from "./RequestBuilder";
export declare class Fetcher implements IFetcher {
    #private;
    constructor(service: IFetchingService, useBLOBs?: boolean);
    clearCache(): Promise<void>;
    evict(path: string | URL, base?: string | URL): Promise<void>;
    request<T extends HTTPMethods>(method: T, path: string | URL, base?: string | URL): RequestBuilder;
    head(path: string | URL, base?: string | URL): RequestBuilder;
    options(path: string | URL, base?: string | URL): RequestBuilder;
    get(path: string | URL, base?: string | URL): RequestBuilder;
    post(path: string | URL, base?: string | URL): RequestBuilder;
    put(path: string | URL, base?: string | URL): RequestBuilder;
    patch(path: string | URL, base?: string | URL): RequestBuilder;
    delete(path: string | URL, base?: string | URL): RequestBuilder;
    assets(firstAsset: BaseAsset, ...assets: BaseAsset[]): Promise<void>;
    assets(progress: IProgress, firstAsset: BaseAsset, ...assets: BaseAsset[]): Promise<void>;
}
//# sourceMappingURL=Fetcher.d.ts.map