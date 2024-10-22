import { Text_Wgsl } from "@juniper-lib/mediatypes/dist/text";
import { BaseFetchedAsset } from "@juniper-lib/fetcher/dist/Asset";
import { translateResponse } from "@juniper-lib/fetcher/dist/translateResponse";
import { Exception } from "@juniper-lib/tslib/dist/Exception";
import { isGoodNumber } from "@juniper-lib/tslib/dist/typeChecks";
import { checkMessages } from "./checkMessages";
class WgslShader {
    #label;
    #code;
    get code() { return this.#code; }
    constructor(label, code) {
        this.workGroupSize = null;
        this.#label = label;
        this.#code = code;
        const constants = new Map(Array.from(code.matchAll(/const (\w+) = ([^;]+);/g))
            .map(match => [
            match[1],
            parseFloat(match[2])
        ]));
        this.constants = Object.freeze({
            has(key) { return constants.has(key); },
            get(key) { return constants.get(key); },
            set: ((key, value) => {
                if (!constants.has(key)) {
                    throw new Exception(`The constant "${key}" does not exist in the shader source.`);
                }
                const replacePattern = new RegExp(`const ${key} = ([^;]+);`);
                this.#code = this.#code.replace(replacePattern, `const ${key} = ${value};`);
            }).bind(this)
        });
        const entryPoints = new Map();
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
            entryPoints.set(match[1], match[2]);
        }
        this.entryPoints = Object.freeze({
            has(key) { return entryPoints.has(key); },
            get(key) { return entryPoints.get(key); }
        });
        Object.freeze(this);
    }
    async compile(device) {
        const module = device.createShaderModule({
            label: "Shader Module: " + this.#label,
            code: this.#code
        });
        await checkMessages("At shader creation", module);
        return module;
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