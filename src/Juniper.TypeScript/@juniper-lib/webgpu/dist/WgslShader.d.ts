/// <reference types="dist" />
import { BaseFetchedAsset } from "@juniper-lib/fetcher/dist/Asset";
import { IFetcherBodiedResult } from "@juniper-lib/fetcher/dist/IFetcher";
import { IResponse } from "@juniper-lib/fetcher/dist/IResponse";
type EntryPointType = "compute" | "vertex" | "fragment";
interface IShaderEntryPoints {
    has(key: EntryPointType): boolean;
    get(key: EntryPointType): string;
}
interface IShaderConstants {
    has(key: string): boolean;
    get(key: string): number;
    set(key: string, value: number): void;
}
declare class WgslShader {
    #private;
    get code(): string;
    readonly constants: IShaderConstants;
    readonly entryPoints: IShaderEntryPoints;
    readonly workGroupSize: number;
    constructor(label: string, code: string);
    compile(device: GPUDevice): Promise<GPUShaderModule>;
}
export declare class AssetWgslShader extends BaseFetchedAsset<WgslShader> {
    #private;
    constructor(path: string, useCache?: boolean);
    protected getResponse(request: IFetcherBodiedResult): Promise<IResponse<WgslShader>>;
}
export {};
//# sourceMappingURL=WgslShader.d.ts.map