/// <reference types="webxr" />
import { FrameAndRenderBuffers } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { BaseTexture } from "./Texture";
declare abstract class BaseFrameBufferTexture extends BaseTexture implements IRenderTargetAttachment {
    readonly width: number;
    readonly height: number;
    constructor(gl: WebGL2RenderingContext, type: GLenum, width: number, height: number);
    abstract attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}
export declare class FrameBufferTexture extends BaseFrameBufferTexture {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number);
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}
declare abstract class BaseFrameBufferTextureMultiview<MVExtT extends OVR_multiview2 | OCULUS_multiview> extends BaseFrameBufferTexture {
    protected readonly ext: MVExtT;
    protected readonly views: XRView[];
    constructor(gl: WebGL2RenderingContext, ext: MVExtT, format: FrameAndRenderBuffers, width: number, height: number, views: XRView[]);
}
export declare class FrameBufferTextureMultiview extends BaseFrameBufferTextureMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, format: FrameAndRenderBuffers, width: number, height: number, views: XRView[]);
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}
export declare class FrameBufferTextureMultiviewMultisampled extends BaseFrameBufferTextureMultiview<OCULUS_multiview> {
    private readonly samples;
    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, format: FrameAndRenderBuffers, width: number, height: number, samples: number, views: XRView[]);
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}
export {};
//# sourceMappingURL=FrameBufferTexture.d.ts.map