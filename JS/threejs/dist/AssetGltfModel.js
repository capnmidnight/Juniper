import { isDefined, isFunction } from "@juniper-lib/util";
import { BaseFetchedAsset, isAsset, translateResponse } from "@juniper-lib/fetcher";
import { Model_Gltf_Binary, Model_Gltf_Json } from "@juniper-lib/mediatypes";
export class AssetGltfModel extends BaseFetchedAsset {
    constructor(env, path, type, useCache) {
        if (!Model_Gltf_Binary.matches(type)
            && !Model_Gltf_Json.matches(type)) {
            throw new Error("Only GLTF model types are currently supported");
        }
        super(path, type, useCache);
        this.env = env;
    }
    async getResponse(request) {
        const response = await request.file();
        return translateResponse(response, (file) => this.env.loadGltf(file));
    }
}
export function isGltfAsset(obj) {
    return isDefined(obj)
        && isFunction(obj.setEnvironment)
        && isAsset(obj);
}
//# sourceMappingURL=AssetGltfModel.js.map