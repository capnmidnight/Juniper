import { Text_Wgsl } from "@juniper-lib/mediatypes/dist/text";
import { BaseFetchedAsset } from "@juniper-lib/fetcher/dist/Asset";
import { IFetcherBodiedResult } from "@juniper-lib/fetcher/dist/IFetcher";
import { IResponse } from "@juniper-lib/fetcher/dist/IResponse";
import { translateResponse } from "@juniper-lib/fetcher/dist/translateResponse";
import { Exception } from "@juniper-lib/tslib/dist/Exception";
import { isGoodNumber } from "@juniper-lib/tslib/dist/typeChecks";
import { checkMessages } from "./checkMessages";

type EntryPointType =
    | "compute"
    | "vertex"
    | "fragment";

interface IShaderEntryPoints {
    has(key: EntryPointType): boolean;
    get(key: EntryPointType): string;
}

interface IShaderConstants {
    has(key: string): boolean;
    get(key: string): number;
    set(key: string, value: number): void;
}

class WgslShader {

    #label: string;
    #code: string;
    get code(){ return this.#code; }

    readonly constants: IShaderConstants;
    readonly entryPoints: IShaderEntryPoints;
    readonly workGroupSize: number = null;

    constructor(label: string, code: string) {
        this.#label = label;
        this.#code = code;

        const constants = new Map(
            Array.from(code.matchAll(/const (\w+) = ([^;]+);/g))
                .map(match => [
                    match[1],
                    parseFloat(match[2])
                ])
        );

        this.constants = Object.freeze({
            has(key: string) { return constants.has(key); },
            get(key: string) { return constants.get(key); },
            set: ((key: string, value: number) => {
                if (!constants.has(key)) {
                    throw new Exception(`The constant "${key}" does not exist in the shader source.`);
                }
        
                const replacePattern = new RegExp(`const ${key} = ([^;]+);`);
                this.#code = this.#code.replace(replacePattern, `const ${key} = ${value};`);
            }).bind(this)
        });

        const entryPoints = new Map<EntryPointType, string>();
        const computeMatch = code.match(/@compute\s+@workgroup_size\s*\(\s*([^)]+)\s*\)\s+fn\s+(\w+)/);
        if (computeMatch) {
            entryPoints.set("compute", computeMatch[2]);
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
            entryPoints.set(match[1] as EntryPointType, match[2]);
        }

        this.entryPoints = Object.freeze({
            has(key: EntryPointType) { return entryPoints.has(key); },
            get(key: EntryPointType) { return entryPoints.get(key); }
        });

        Object.freeze(this);
    }

    async compile(device: GPUDevice) {
        const module = device.createShaderModule({
            label: "Shader Module: " + this.#label,
            code: this.#code
        });

        await checkMessages("At shader creation", module);

        return module;
    }
}

export class AssetWgslShader extends BaseFetchedAsset<WgslShader> {

    #label: string;

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
