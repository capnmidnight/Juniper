import { FrameAndRenderBuffers, FramebufferType } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
declare abstract class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> implements IRenderTargetAttachment {
    readonly width: number;
    readonly height: number;
    constructor(gl: WebGL2RenderingContext, width: number, height: number);
    attach(target: FramebufferType, attachment: FrameAndRenderBuffers): void;
    bind(): void;
    protected onDisposing(): void;
}
export declare class RenderBuffer extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number);
}
export declare class RenderBufferMultisampled extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number, samples: number);
}
export {};
//# sourceMappingURL=RenderBuffer.d.ts.map