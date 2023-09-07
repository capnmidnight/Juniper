import { IDisposable } from "@juniper-lib/tslib/using";
import type { GeometryDescription, VertexComponent } from "./geometry/GeometryDescription";
import { Attrib } from "./managed/object/Attrib";
export declare class Geometry implements IDisposable {
    private gl;
    private vertBuffer;
    private idxBuffer;
    private geomType;
    private elementType;
    private stride;
    private layouts;
    private offsets;
    constructor(gl: WebGL2RenderingContext, desc: GeometryDescription);
    private disposed;
    dispose(): void;
    bind(): void;
    setAttribute(attrib: Attrib, component: VertexComponent): void;
    drawElements(): void;
    drawElementsInstanced(instanceCount: number): void;
}
//# sourceMappingURL=Geometry.d.ts.map