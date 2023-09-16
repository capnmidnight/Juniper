/// <reference types="webxr" />
import { mat4 } from "gl-matrix";
import type { Camera } from "./Camera";
import type { Geometry } from "./Geometry";
import type { BaseTexture } from "./managed/resource/Texture";
import type { Material } from "./Material";
export declare class Mesh {
    private gl;
    private geom;
    private texture;
    readonly material: Material;
    model: mat4;
    visible: boolean;
    constructor(gl: WebGL2RenderingContext, geom: Geometry, texture: BaseTexture, material: Material);
    render(cam: Camera, frame?: XRFrame, baseRefSpace?: XRReferenceSpace): void;
}
//# sourceMappingURL=Mesh.d.ts.map