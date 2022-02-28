import { isNullOrUndefined } from "juniper-tslib";
import { FrameAndRenderBuffers } from "../GLEnum";
import { IRenderTargetBuffer } from "../RenderTarget";
import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> implements IRenderTargetBuffer {
    format: FrameAndRenderBuffers;
    constructor(gl: WebGL2RenderingContext, public readonly attachment: GLenum) {
        super(gl, gl.createRenderbuffer());

        this.format = this.attachment === gl.DEPTH_ATTACHMENT
            ? FrameAndRenderBuffers.DEPTH_COMPONENT16
            : FrameAndRenderBuffers.RGBA8;

        this.bind();
    }

    fbBind(target: GLenum) {
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
        gl.renderbufferStorage(gl.RENDERBUFFER, this.format, width, height);
    }
}

export class RenderBufferMultisampled extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, attachment: GLenum, samples?: number) {
        super(gl, attachment);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        gl.renderbufferStorageMultisample(gl.RENDERBUFFER, samples, this.format, width, height);
    }
}