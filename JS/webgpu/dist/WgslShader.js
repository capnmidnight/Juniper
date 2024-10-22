import { Exception, isGoodNumber } from "@juniper-lib/util";
import { BaseFetchedAsset, translateResponse } from "@juniper-lib/fetcher";
import { Text_Wgsl } from "@juniper-lib/mediatypes";
import { checkShaderCompilationMessages } from "./checkShaderCompilationMessages";
class WgslShader {
    #label;
    #code;
    get code() { return this.#code; }
    constructor(label, code) {
        this.entryPoints = new Map();
        this.workGroupSize = null;
        this.#label = label;
        this.#code = code;
        this.constants = new Map(Array.from(code.matchAll(/const (\w+) = ([^;]+);/g))
            .map(match => [
            match[1],
            parseFloat(match[2])
        ]));
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
            this.entryPoints.set(match[1], match[2]);
        }
    }
    changeConstant(key, value) {
        if (!this.constants.has(key)) {
            throw new Exception(`The constant "${key}" does not exist in the shader source.`);
        }
        const replacePattern = new RegExp(`const ${key} = ([^;]+);`);
        this.#code = this.#code.replace(replacePattern, `const ${key} = ${value};`);
    }
    async compile(device, checkMessages = false) {
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
export class AssetWgslShader extends BaseFetchedAsset {
    #label;
    constructor(path, useCache) {
        super(path, Text_Wgsl, useCache);
        this.#label = path;
    }
    async getResponse(request) {
        return translateResponse(await request.text(this.type), code => new WgslShader(this.#label, code));
    }
}
//# sourceMappingURL=WgslShader.js.map