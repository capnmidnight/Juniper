import { ManagedWebGLResource } from "./ManagedWebGLResource";
class BaseRenderBuffer extends ManagedWebGLResource {
    constructor(gl, width, height) {
        super(gl, gl.createRenderbuffer());
        this.width = width;
        this.height = height;
    }
    attach(target, attachment) {
        this.gl.framebufferRenderbuffer(target, attachment, this.gl.RENDERBUFFER, this.handle);
    }
    bind() {
        this.gl.bindRenderbuffer(this.gl.RENDERBUFFER, this.handle);
    }
    onDisposing() {
        this.gl.deleteRenderbuffer(this.handle);
    }
}
export class RenderBuffer extends BaseRenderBuffer {
    constructor(gl, format, width, height) {
        super(gl, width, height);
        this.bind();
        this.gl.renderbufferStorage(this.gl.RENDERBUFFER, format, this.width, this.height);
    }
}
export class RenderBufferMultisampled extends BaseRenderBuffer {
    constructor(gl, format, width, height, samples) {
        super(gl, width, height);
        this.bind();
        this.gl.renderbufferStorageMultisample(this.gl.RENDERBUFFER, samples, format, this.width, this.height);
    }
}
//# sourceMappingURL=RenderBuffer.js.map