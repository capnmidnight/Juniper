import { Mat4Like } from "gl-matrix";
import type { BaseTexture } from "../resource/Texture";
import { ManagedWebGLObject } from "./ManagedWebGLObject";
export declare class Uniform extends ManagedWebGLObject<WebGLUniformLocation> {
    readonly name: string;
    constructor(gl: WebGL2RenderingContext, handle: WebGLUniformLocation, name: string);
    set1i(v: number): void;
    set2i(x: number, y: number): void;
    set1f(v: number): void;
    set2f(x: number, y: number): void;
    setMatrix4fv(data: Mat4Like): void;
    setTexture(texture: BaseTexture, slot: number): void;
}
//# sourceMappingURL=Uniform.d.ts.map