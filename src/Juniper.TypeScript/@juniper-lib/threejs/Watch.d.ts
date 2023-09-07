import { IDisposable } from "@juniper-lib/tslib/using";
import { Mesh } from "three";
import { AssetGltfModel } from "./AssetGltfModel";
import type { Environment } from "./environment/Environment";
import { ErsatzObject } from "./objects";
export declare class Watch implements ErsatzObject, IDisposable {
    readonly asset: AssetGltfModel;
    private _model;
    get object(): Mesh<import("three").BufferGeometry, import("three").Material | import("three").Material[]>;
    constructor(env: Environment, modelPath: string);
    private disposed;
    dispose(): void;
}
//# sourceMappingURL=Watch.d.ts.map