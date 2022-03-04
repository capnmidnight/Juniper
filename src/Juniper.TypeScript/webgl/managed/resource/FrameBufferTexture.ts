import { FrameAndRenderBuffers } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { BaseTexture } from "./Texture";

abstract class BaseFrameBufferTexture extends BaseTexture implements IRenderTargetAttachment {
    constructor(gl: WebGL2RenderingContext, type: GLenum, public readonly width: number, public readonly height: number) {
        super(gl, type);
    }

    abstract attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}

export class FrameBufferTexture extends BaseFrameBufferTexture {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number) {
        super(gl, gl.TEXTURE_2D, width, height);

        this.gl.texParameteri(this.type, this.gl.TEXTURE_MAG_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_MIN_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_S, this.gl.CLAMP_TO_EDGE);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_T, this.gl.CLAMP_TO_EDGE);

        this.gl.texStorage2D(this.type, 1, format, this.width, this.height);
    }

    override attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void {
        this.gl.framebufferTexture2D(fbType, attachment, this.gl.TEXTURE_2D, this.handle, 0);
    }
}


abstract class BaseFrameBufferTextureMultiview<MVExtT extends OVR_multiview2 | OCULUS_multiview> extends BaseFrameBufferTexture {
    constructor(
        gl: WebGL2RenderingContext,
        protected readonly ext: MVExtT,
        format: FrameAndRenderBuffers,
        width: number,
        height: number,
        protected readonly views: XRView[]
    ) {
        super(gl, gl.TEXTURE_2D_ARRAY, width, height);
        this.bind();
        this.gl.texStorage3D(this.gl.TEXTURE_2D_ARRAY, 1, format, this.width, this.height, this.views.length);
    }
}


export class FrameBufferTextureMultiview extends BaseFrameBufferTextureMultiview<OVR_multiview2> {
    constructor(
        gl: WebGL2RenderingContext,
        ext: OVR_multiview2,
        format: FrameAndRenderBuffers,
        width: number,
        height: number,
        views: XRView[]) {
        super(gl, ext, format, width, height, views);
    }

    override attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void {
        this.ext.framebufferTextureMultiviewOVR(fbType, attachment, this.handle, 0, 0, this.views.length);
    }
}


export class FrameBufferTextureMultiviewMultisampled extends BaseFrameBufferTextureMultiview<OCULUS_multiview> {
    private readonly samples: number;

    constructor(
        gl: WebGL2RenderingContext,
        ext: OCULUS_multiview,
        format: FrameAndRenderBuffers,
        width: number,
        height: number,
        samples: number,
        views: XRView[]) {
        super(gl, ext, format, width, height, views);

        this.samples = samples;
    }

    override attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void {
        this.ext.framebufferTextureMultisampleMultiviewOVR(fbType, attachment, this.handle, 0, this.samples, 0, this.views.length);
    }
}
