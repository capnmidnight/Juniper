import { isNullOrUndefined } from "juniper-tslib";
import { FrameAndRenderBuffers, FramebufferType } from "../../GLEnum";
import { IRenderTargetAttachment } from "./FrameBuffer";
import { ManagedWebGLResource } from "./ManagedWebGLResource";


abstract class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> implements IRenderTargetAttachment {
    private storageSet = false;

    constructor(gl: WebGL2RenderingContext, public readonly width: number, public readonly height: number) {
        super(gl, gl.createRenderbuffer());
    }

    attach(target: FramebufferType, attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers) {
        if (!this.storageSet) {
            this.bind();
            this.setStorage(format);
            this.storageSet = true;
        }
        this.gl.framebufferRenderbuffer(target, attachment, this.gl.RENDERBUFFER, this.handle);
    }

    bind() {
        this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.handle);
    }

    protected abstract setStorage(format: FrameAndRenderBuffers): void;

    protected onDisposing(): void {
        this.gl.deleteRenderbuffer(this.handle);
    }
}

export class RenderBuffer extends BaseRenderBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number) {
        super(gl, width, height);
    }

    protected override setStorage(format: FrameAndRenderBuffers): void {
        this.gl.renderbufferStorage(this.gl.RENDERBUFFER, format, this.width, this.height);
    }
}

export class RenderBufferMultisampled extends BaseRenderBuffer {
    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, samples?: number) {
        super(gl, width, height);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }


    protected override setStorage(format: FrameAndRenderBuffers): void {
        this.gl.renderbufferStorageMultisample(this.gl.RENDERBUFFER, this.samples, format, this.width, this.height);
    }
}