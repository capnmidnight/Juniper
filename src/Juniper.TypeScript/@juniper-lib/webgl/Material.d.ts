/// <reference types="webxr" />
import type { mat4 } from "gl-matrix";
import { BaseProgram } from "./BaseProgram";
import type { Geometry } from "./Geometry";
import type { BaseTexture } from "./managed/resource/Texture";
export declare abstract class BaseMaterial extends BaseProgram {
    private uGamma;
    private uModel;
    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string);
    setGamma(gamma: number): void;
    setModel(model: mat4): void;
    abstract setGeometry(geom: Geometry): void;
    abstract setTexture(texture: BaseTexture): void;
}
export declare abstract class Material extends BaseMaterial {
    private uProjection;
    private uView;
    private uEye;
    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string);
    setEye(eye: XREye, isStereo: boolean): void;
    setProjection(proj: mat4): void;
    setView(view: mat4): void;
}
//# sourceMappingURL=Material.d.ts.map