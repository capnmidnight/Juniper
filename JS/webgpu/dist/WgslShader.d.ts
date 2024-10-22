import { IResponse } from "@juniper-lib/util";
import { BaseFetchedAsset, IFetcherBodiedResult } from "@juniper-lib/fetcher";
type EntryPointType = "compute" | "vertex" | "fragment";
declare class WgslShader {
    #private;
    readonly constants: Map<string, number>;
    readonly entryPoints: Map<EntryPointType, string>;
    readonly workGroupSize: number;
    get code(): string;
    constructor(label: string, code: string);
    changeConstant(key: string, value: number): void;
    compile(device: GPUDevice, checkMessages?: boolean): Promise<GPUShaderModule>;
}
export declare class AssetWgslShader extends BaseFetchedAsset<WgslShader> {
    #private;
    constructor(path: string, useCache?: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<WgslShader>>;
}
export {};
//# sourceMappingURL=WgslShader.d.ts.map