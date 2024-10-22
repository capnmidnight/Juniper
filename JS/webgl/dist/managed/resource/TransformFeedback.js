import { ManagedWebGLResource } from "./ManagedWebGLResource";
export class TransformFeedback extends ManagedWebGLResource {
    constructor(gl, target) {
        super(gl, gl.createTransformFeedback());
        this.target = target;
    }
    bind() {
        this.gl.bindTransformFeedback(this.target, this.handle);
    }
    onDisposing() {
        this.gl.deleteTransformFeedback(this.handle);
    }
}
//# sourceMappingURL=TransformFeedback.js.map