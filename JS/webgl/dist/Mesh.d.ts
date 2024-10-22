import { Camera } from "@juniper-lib/three-dee";
import { Mat4 } from "gl-matrix";
import type { Geometry } from "./Geometry";
import type { BaseTexture } from "./managed/resource/Texture";
import type { Material } from "./Material";
export declare class Mesh {
    private gl;
    private geom;
    private texture;
    readonly material: Material;
    model: Mat4;
    visible: boolean;
    constructor(gl: WebGL2RenderingContext, geom: Geometry, texture: BaseTexture, material: Material);
    render(cam: Camera, frame?: XRFrame, baseRefSpace?: XRReferenceSpace): void;
}
//# sourceMappingURL=Mesh.d.ts.map