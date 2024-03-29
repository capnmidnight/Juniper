import { BaseFetchedAsset } from "@juniper-lib/fetcher/dist/Asset";
import type { IFetcherBodiedResult } from "@juniper-lib/fetcher/dist/IFetcher";
import type { IResponse } from "@juniper-lib/fetcher/dist/IResponse";
import type { MediaType } from "@juniper-lib/mediatypes";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { GLTF } from "./examples/loaders/GLTFLoader";
export declare class AssetGltfModel extends BaseFetchedAsset<GLTF> {
    private readonly env;
    constructor(env: BaseEnvironment, path: string, type: string | MediaType, useCache?: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<GLTF>>;
}
export declare function isGltfAsset(obj: any): obj is AssetGltfModel;
//# sourceMappingURL=AssetGltfModel.d.ts.map