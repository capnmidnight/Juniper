import { isNullOrUndefined } from "juniper-tslib";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { FrameAndRenderBuffers } from "../../GLEnum";
import { BaseTexture } from "./Texture";

abstract class BaseFrameBufferTexture extends BaseTexture implements IRenderTargetAttachment {
    private storageSet = false;

    constructor(gl: WebGL2RenderingContext, type: GLenum, public readonly width: number, public readonly height: number) {
        super(gl, type);
    }

    attach(fbType: GLenum, attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers): void {
        if (!this.storageSet) {
            this.bind();
            this.setStorage(format);
            this.storageSet = true;
        }

        this.attachInternal(fbType, attachment, format);
    }

    protected abstract attachInternal(fbType: GLenum, attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers): void;
    protected abstract setStorage(format: FrameAndRenderBuffers): void;
}

export class FrameBufferTexture extends BaseFrameBufferTexture {
    constructor(gl: WebGL2RenderingContext, width: number, height: number) {
        super(gl, gl.TEXTURE_2D, width, height);
    }

    protected override setStorage(format: FrameAndRenderBuffers) {

        this.gl.texParameteri(this.type, this.gl.TEXTURE_MAG_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_MIN_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_S, this.gl.CLAMP_TO_EDGE);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_T, this.gl.CLAMP_TO_EDGE);

        this.gl.texStorage2D(this.type, 1, format, this.width, this.height);
    }

    protected override attachInternal(fbType: GLenum, attachment: FrameAndRenderBuffers, _format: FrameAndRenderBuffers): void {
        this.gl.framebufferTexture2D(fbType, attachment, this.gl.TEXTURE_2D, this.handle, 0);
    }
}


abstract class BaseFrameBufferTextureMultiview<MVExtT extends OVR_multiview2 | OCULUS_multiview> extends BaseFrameBufferTexture {
    constructor(
        gl: WebGL2RenderingContext,
        protected readonly ext: MVExtT,
        width: number,
        height: number,
        protected readonly numViews: number
    ) {
        super(gl, gl.TEXTURE_2D_ARRAY, width, height);
    }

    protected override setStorage(format: FrameAndRenderBuffers) {
        this.gl.texStorage3D(this.gl.TEXTURE_2D_ARRAY, 1, format, this.width, this.height, this.numViews);
    }
}


export class FrameBufferTextureMultiview extends BaseFrameBufferTextureMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, width: number, height: number, numViews: number) {
        super(gl, ext, width, height, numViews);
    }

    protected override attachInternal(fbType: GLenum, attachment: FrameAndRenderBuffers, _format: FrameAndRenderBuffers): void {
        this.ext.framebufferTextureMultiviewOVR(fbType, attachment, this.handle, 0, 0, this.numViews);
    }
}


export class FrameBufferTextureMultiviewMultisampled extends BaseFrameBufferTextureMultiview<OCULUS_multiview> {
    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, width: number, height: number, numViews: number, samples?: number) {
        super(gl, ext, width, height, numViews);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    protected override attachInternal(fbType: GLenum, attachment: FrameAndRenderBuffers, _format: FrameAndRenderBuffers): void {
        this.ext.framebufferTextureMultisampleMultiviewOVR(fbType, attachment, this.handle, 0, this.samples, 0, this.numViews);
    }
}
