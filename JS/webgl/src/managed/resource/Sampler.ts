import { ManagedWebGLResource } from "./ManagedWebGLResource";

export class Sampler extends ManagedWebGLResource<WebGLSampler> {
    constructor(gl: WebGL2RenderingContext, private unit: GLenum) {
        super(gl, gl.createSampler());
    }

    bind() {
        this.gl.bindSampler(this.unit, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteSampler(this.handle);
    }
}
