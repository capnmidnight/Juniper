import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class TransformFeedback extends ManagedWebGLResource<WebGLTransformFeedback> {
    constructor(gl: WebGL2RenderingContext, private target: GLenum) {
        super(gl, gl.createTransformFeedback());
    }

    bind() {
        this.gl.bindTransformFeedback(this.target, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteTransformFeedback(this.handle);
    }
}
