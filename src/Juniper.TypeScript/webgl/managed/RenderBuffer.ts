import { isNullOrUndefined } from "juniper-tslib";
import { FrameAndRenderBuffers } from "../GLEnum";
import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class BaseRenderBuffer extends ManagedWebGLResource<WebGLRenderbuffer> {
    format: FrameAndRenderBuffers;
    constructor(gl: WebGL2RenderingContext, private _attachment: GLenum) {
        super(gl, gl.createRenderbuffer());

        this.format = _attachment === gl.DEPTH_ATTACHMENT
            ? FrameAndRenderBuffers.DEPTH_COMPONENT16
            : FrameAndRenderBuffers.RGBA8;

        this.bind();
    }

    fbBind(target: GLenum) {
        this.gl.framebufferRenderbuffer(target, this.attachment, this.gl.RENDERBUFFER, this.handle);
    }

    get attachment() {
        return this._attachment;
    }

    bind() {
        this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.handle);
    }

    onDisposing(): void {
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