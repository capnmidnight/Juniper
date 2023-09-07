/// <reference types="webxr" />
import { CanvasTypes } from "@juniper-lib/dom/canvas";
import { ClearBits, FrameAndRenderBuffers, FramebufferTypes } from "../../GLEnum";
import { FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./FrameBufferTexture";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBufferMultisampled } from "./RenderBuffer";
export declare const DefaultRenderBufferFormats: Map<FrameAndRenderBuffers, FrameAndRenderBuffers>;
export interface IRenderTargetAttachment extends ManagedWebGLResource<WebGLRenderbuffer | WebGLTexture> {
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}
export declare abstract class BaseRenderTarget extends ManagedWebGLResource<WebGLFramebuffer> {
    readonly width: number;
    readonly height: number;
    private readonly isOwned;
    constructor(gl: WebGL2RenderingContext, width: number, height: number, buffer?: WebGLFramebuffer | CanvasTypes);
    bind(asType: FramebufferTypes): void;
    clear(mask: ClearBits): void;
    protected onDisposing(): void;
    getStatus(): string;
    protected translateStatus(code: number): string;
}
export declare abstract class BaseFrameBuffer extends BaseRenderTarget {
    private readonly attachmentsByLocation;
    private readonly attachments;
    constructor(gl: WebGL2RenderingContext, width: number, height: number, buffer?: WebGLFramebuffer | CanvasTypes);
    onDisposing(): void;
    invalidate(): void;
    attach(attachment: FrameAndRenderBuffers): this;
    attach(attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers): this;
    attach(attachment: FrameAndRenderBuffers, object: IRenderTargetAttachment): this;
    protected createAttachment(format: FrameAndRenderBuffers): IRenderTargetAttachment;
}
export declare class FrameBuffer extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number);
}
export declare class FrameBufferMultisampled extends BaseFrameBuffer {
    private readonly samples;
    constructor(gl: WebGL2RenderingContext, width: number, height: number, samples?: number);
    protected createAttachment(format: FrameAndRenderBuffers): RenderBufferMultisampled;
}
export declare class FrameBufferCanvas extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext);
}
declare abstract class BaseFrameBufferWebXR extends BaseFrameBuffer {
    readonly views: XRView[];
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[]);
    protected createAttachment(format: FrameAndRenderBuffers): IRenderTargetAttachment;
}
export declare class FrameBufferWebXR extends BaseFrameBufferWebXR {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[]);
}
export declare class FrameBufferWebXRMultisampled extends BaseFrameBufferWebXR {
    private readonly samples;
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], samples: number);
    protected createAttachment(format: FrameAndRenderBuffers): RenderBufferMultisampled;
}
declare abstract class BaseFrameBufferWebXRMultiview<ExtType extends OVR_multiview2> extends BaseFrameBufferWebXR {
    protected readonly ext: ExtType;
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], ext: ExtType);
    protected translateStatus(code: number): string;
}
export declare class FrameBufferWebXRMultiview extends BaseFrameBufferWebXRMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], ext: OVR_multiview2);
    protected createAttachment(format: FrameAndRenderBuffers): FrameBufferTextureMultiview;
}
export declare class FrameBufferWebXRMultiviewMultisampled extends BaseFrameBufferWebXRMultiview<OCULUS_multiview> {
    private readonly samples;
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], samples: number, ext: OCULUS_multiview);
    protected createAttachment(format: FrameAndRenderBuffers): FrameBufferTextureMultiviewMultisampled;
}
export {};
//# sourceMappingURL=FrameBuffer.d.ts.map