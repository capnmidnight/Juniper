import { ManagedWebGLResource } from "./ManagedWebGLResource";
export type BufferArrayType = Int8Array | Uint8Array | Uint8ClampedArray | Int16Array | Uint16Array | Int32Array | Uint32Array | Float32Array;
export declare class Buffer extends ManagedWebGLResource<WebGLBuffer> {
    private type;
    private usage;
    readonly length: number;
    readonly dataSize: number;
    readonly dataType: GLenum;
    constructor(gl: WebGL2RenderingContext, type: string, usage: string, data: BufferArrayType, dataType?: number, dataSize?: number);
    protected onDisposing(): void;
    bind(): void;
}
export declare class BufferArray extends Buffer {
    constructor(gl: WebGL2RenderingContext, usage: string, data: BufferArrayType);
}
export declare class BufferElementArray extends Buffer {
    constructor(gl: WebGL2RenderingContext, usage: string, data: BufferArrayType);
}
//# sourceMappingURL=Buffer.d.ts.map