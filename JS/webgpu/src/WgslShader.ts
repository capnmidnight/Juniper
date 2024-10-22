import { Exception, IResponse, isGoodNumber } from "@juniper-lib/util";
import { BaseFetchedAsset, IFetcherBodiedResult, translateResponse } from "@juniper-lib/fetcher";
import { Text_Wgsl } from "@juniper-lib/mediatypes";
import { checkShaderCompilationMessages } from "./checkShaderCompilationMessages";

type EntryPointType =
    | "compute"
    | "vertex"
    | "fragment";

class WgslShader {

    readonly constants: Map<string, number>;
    readonly entryPoints = new Map<EntryPointType, string>();
    readonly workGroupSize: number = null;

    readonly #label;

    #code;
    get code() { return this.#code; }

    constructor(label: string, code: string) {
        this.#label = label;
        this.#code = code;

        this.constants = new Map(
            Array.from(code.matchAll(/const (\w+) = ([^;]+);/g))
                .map(match => [
                    match[1],
                    parseFloat(match[2])
                ])
        );

        const computeMatch = code.match(/@compute\s+@workgroup_size\s*\(\s*([^)]+)\s*\)\s+fn\s+(\w+)/);
        if (computeMatch) {
            this.entryPoints.set("compute", computeMatch[2]);
            const keyOrValue = computeMatch[2];
            if (this.constants.has(keyOrValue)) {
                this.workGroupSize = this.constants.get(keyOrValue);
            }
            else {
                const value = parseFloat(keyOrValue);
                if (isGoodNumber(value)) {
                    this.workGroupSize = value;
                }
            }
        }

        for (const match of code.matchAll(/@(vertex|fragment)\s+fn\s+(\w+)/g)) {
            this.entryPoints.set(match[1] as EntryPointType, match[2]);
        }
    }

    changeConstant(key: string, value: number) {
        if (!this.constants.has(key)) {
            throw new Exception(`The constant "${key}" does not exist in the shader source.`);
        }

        const replacePattern = new RegExp(`const ${key} = ([^;]+);`);
        this.#code = this.#code.replace(replacePattern, `const ${key} = ${value};`);
    }

    async compile(device: GPUDevice, checkMessages = false) {
        const shaderModule = device.createShaderModule({
            label: this.#label,
            code: this.#code
        });

        if (checkMessages) {
            await checkShaderCompilationMessages("createShaderModule", shaderModule);
        }

        return shaderModule;
    }
}

export class AssetWgslShader extends BaseFetchedAsset<WgslShader> {

    readonly #label: string;

    constructor(path: string, useCache?: boolean) {
        super(path, Text_Wgsl, useCache);
        this.#label = path;
    }

    protected override async getResponse(request: IFetcherBodiedResult): Promise<IResponse<WgslShader>> {
        return translateResponse(
            await request.text(this.type),
            code => new WgslShader(this.#label, code)
        );
    }
}