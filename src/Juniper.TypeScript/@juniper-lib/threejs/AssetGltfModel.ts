import { BaseFetchedAsset, isAsset } from "@juniper-lib/fetcher/Asset";
import type { IFetcherBodiedResult } from "@juniper-lib/fetcher/IFetcher";
import type { IResponse } from "@juniper-lib/fetcher/IResponse";
import { translateResponse } from "@juniper-lib/fetcher/translateResponse";
import type { MediaType } from "@juniper-lib/mediatypes";
import { Model_Gltf_Binary, Model_Gltf_Json } from "@juniper-lib/mediatypes";
import { isDefined, isFunction } from "@juniper-lib/tslib/typeChecks";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { GLTF } from "./examples/loaders/GLTFLoader";

export class AssetGltfModel<ErrorT = unknown> extends BaseFetchedAsset<GLTF, ErrorT> {

    constructor(private readonly env: BaseEnvironment, path: string, type: string | MediaType, useCache?: boolean) {
        if (!Model_Gltf_Binary.matches(type)
            && !Model_Gltf_Json.matches(type)) {
            throw new Error("Only GLTF model types are currently supported");
        }

        super(path, type, useCache);
    }

    protected async getResponse(request: IFetcherBodiedResult): Promise<IResponse<GLTF>> {
        const response = await request.file();
        return translateResponse(response, (file) =>
            this.env.loadGltf(file));
    }
}

export function isGltfAsset(obj: any): obj is AssetGltfModel {
    return isDefined(obj)
        && isFunction(obj.setEnvironment)
        && isAsset(obj);
}