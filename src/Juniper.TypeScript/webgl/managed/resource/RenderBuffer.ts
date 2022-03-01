import { isNullOrUndefined } from "juniper-tslib";
import { FrameAndRenderBuffers, FramebufferType } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { ManagedWebGLResource } from "./ManagedWebGLResource";

const defaultFormats = new Map([
    [FrameAndRenderBuffers.COLOR_ATTACHMENT0, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT1, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT2, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT3, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT4, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT5, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT6, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT7, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT8, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT9, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT10, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT11, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT12, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT13, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT14, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT15, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.DEPTH_ATTACHMENT, FrameAndRenderBuffers.DEPTH_COMPONENT16],
    [FrameAndRenderBuffers.DEPTH_STENCIL_ATTACHMENT, FrameAndRenderBuffers.DEPTH24_STENCIL8]
]);


export class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> implements IRenderTargetAttachment {
    format: FrameAndRenderBuffers;
    constructor(gl: WebGL2RenderingContext, public readonly attachment: FrameAndRenderBuffers) {
        super(gl, gl.createRenderbuffer());
        this.format = defaultFormats.get(this.attachment);
    }

    fbBind(target: FramebufferType) {
        this.gl.framebufferRenderbuffer(target, this.attachment, this.gl.RENDERBUFFER, this.handle);
    }

    bind() {
        this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteRenderbuffer(this.handle);
    }
}

export class RenderBuffer extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, attachment: GLenum) {
        super(gl, attachment);

        this.bind();
        gl.renderbufferStorage(gl.RENDERBUFFER, this.format, width, height);
    }
}

export class RenderBufferMultisampled extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, attachment: GLenum, samples?: number) {
        super(gl, attachment);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.bind();
        gl.renderbufferStorageMultisample(gl.RENDERBUFFER, samples, this.format, width, height);
    }
}