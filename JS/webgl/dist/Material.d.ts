/// <reference types="webxr" />
import type { Mat4, Mat4Like } from "gl-matrix/dist/esm";
import { BaseProgram } from "./BaseProgram";
import type { Geometry } from "./Geometry";
import type { BaseTexture } from "./managed/resource/Texture";
export declare abstract class BaseMaterial extends BaseProgram {
    private uGamma;
    private uModel;
    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string);
    setGamma(gamma: number): void;
    setModel(model: Mat4): void;
    abstract setGeometry(geom: Geometry): void;
    abstract setTexture(texture: BaseTexture): void;
}
export declare abstract class Material extends BaseMaterial {
    private uProjection;
    private uView;
    private uEye;
    constructor(gl: WebGL2RenderingContext, vertSrc: string, fragSrc: string);
    setEye(eye: XREye, isStereo: boolean): void;
    setProjection(proj: Mat4Like): void;
    setView(view: Mat4Like): void;
}
//# sourceMappingURL=Material.d.ts.map