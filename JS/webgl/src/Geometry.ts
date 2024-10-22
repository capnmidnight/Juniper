import { IDisposable, dispose } from "@juniper-lib/tslib/dist/using";
import type { GeometryDescription, VertexComponent, VertexComponentDesc } from "./geometry/GeometryDescription";
import { Attrib } from "./managed/object/Attrib";
import { BufferArray, BufferElementArray } from "./managed/resource/Buffer";

export class Geometry implements IDisposable {
    private vertBuffer: BufferArray;
    private idxBuffer: BufferArray;
    private geomType: GLenum;
    private elementType: GLenum;
    private stride: number;
    private layouts: Map<VertexComponent, VertexComponentDesc>;
    private offsets = new Map<VertexComponent, number>();

    constructor(private gl: WebGL2RenderingContext, desc: GeometryDescription) {
        if (desc.indices instanceof Uint8Array) {
            this.elementType = gl.UNSIGNED_BYTE;
        }
        else if (desc.indices instanceof Uint16Array) {
            this.elementType = gl.UNSIGNED_SHORT;
        }
        else {
            throw new Error("Unsupported index data type");
        }

        this.vertBuffer = new BufferArray(gl, "static-draw", desc.verts);
        this.idxBuffer = new BufferElementArray(gl, "static-draw", desc.indices);
        this.geomType = (gl as any)[desc.type];

        this.layouts = new Map(desc.layout);
        this.stride = 0;
        for (const [comp, layout] of desc.layout) {
            this.offsets.set(comp, this.stride);
            this.stride += layout.size;
        }
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            dispose(this.idxBuffer);
            dispose(this.vertBuffer);
            this.disposed = true;
        }
    }

    bind() {
        this.vertBuffer.bind();
        this.idxBuffer.bind();
    }

    setAttribute(attrib: Attrib, component: VertexComponent) {
        const layout = this.layouts.get(component);
        if (layout) {
            attrib.setBuffer(this.vertBuffer,
                layout.size,
                layout.normalized,
                this.stride,
                this.offsets.get(component));
        }
    }

    drawElements(): void {
        this.gl.drawElements(this.geomType, this.idxBuffer.length, this.elementType, 0);
    }

    drawElementsInstanced(instanceCount: number) {
        this.gl.drawElementsInstanced(this.geomType, this.idxBuffer.length, this.elementType, 0, instanceCount);
    }
}
