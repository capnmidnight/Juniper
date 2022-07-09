import { IFetcherBodiedResult, IResponse, translateResponse } from "@juniper-lib/fetcher";
import { BaseFetchedAsset } from "@juniper-lib/fetcher/Asset";
import { MediaType, Model_Gltf_Binary, Model_Gltf_Json } from "@juniper-lib/mediatypes";
import { GLTF, GLTFLoader } from "./examples/loaders/GLTFLoader";

export class AssetGltfModel<ErrorT = unknown> extends BaseFetchedAsset<GLTF, ErrorT> {

    private static loader = new GLTFLoader();

    constructor(path: string, type: string | MediaType, useCache?: boolean) {
        if (!Model_Gltf_Binary.matches(type)
            && !Model_Gltf_Json.matches(type)) {
            throw new Error("Only GLTF model types are currently supported");
        }

        super(path, type, useCache);
    }

    protected async getResponse(request: IFetcherBodiedResult): Promise<IResponse<GLTF>> {
        const response = await request.file();
        return translateResponse(response, (file) =>
            AssetGltfModel.loader.loadAsync(file));
    }
}