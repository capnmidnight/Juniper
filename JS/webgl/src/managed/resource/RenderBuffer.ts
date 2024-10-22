import { FrameAndRenderBuffers, FramebufferType } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { ManagedWebGLResource } from "./ManagedWebGLResource";


abstract class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> implements IRenderTargetAttachment {
    constructor(gl: WebGL2RenderingContext, public readonly width: number, public readonly height: number) {
        super(gl, gl.createRenderbuffer());
    }

    attach(target: FramebufferType, attachment: FrameAndRenderBuffers) {
        this.gl.framebufferRenderbuffer(target, attachment, this.gl.RENDERBUFFER, this.handle);
    }

    bind() {
        this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteRenderbuffer(this.handle);
    }
}

export class RenderBuffer extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number) {
        super(gl, width, height);
        this.bind();
        this.gl.renderbufferStorage(this.gl.RENDERBUFFER, format, this.width, this.height);
    }
}

export class RenderBufferMultisampled extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, format: FrameAndRenderBuffers, width: number, height: number, samples: number) {
        super(gl, width, height);
        this.bind();
        this.gl.renderbufferStorageMultisample(this.gl.RENDERBUFFER, samples, format, this.width, this.height);
    }
}