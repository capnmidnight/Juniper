import { dispose } from "@juniper-lib/util";
import { BufferArray, BufferElementArray } from "./managed/resource/Buffer";
export class Geometry {
    constructor(gl, desc) {
        this.gl = gl;
        this.offsets = new Map();
        this.disposed = false;
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
        this.geomType = gl[desc.type];
        this.layouts = new Map(desc.layout);
        this.stride = 0;
        for (const [comp, layout] of desc.layout) {
            this.offsets.set(comp, this.stride);
            this.stride += layout.size;
        }
    }
    dispose() {
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
    setAttribute(attrib, component) {
        const layout = this.layouts.get(component);
        if (layout) {
            attrib.setBuffer(this.vertBuffer, layout.size, layout.normalized, this.stride, this.offsets.get(component));
        }
    }
    drawElements() {
        this.gl.drawElements(this.geomType, this.idxBuffer.length, this.elementType, 0);
    }
    drawElementsInstanced(instanceCount) {
        this.gl.drawElementsInstanced(this.geomType, this.idxBuffer.length, this.elementType, 0, instanceCount);
    }
}
//# sourceMappingURL=Geometry.js.map