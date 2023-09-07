import { ManagedWebGLResource } from "./ManagedWebGLResource";
export class Sampler extends ManagedWebGLResource {
    constructor(gl, unit) {
        super(gl, gl.createSampler());
        this.unit = unit;
    }
    bind() {
        this.gl.bindSampler(this.unit, this.handle);
    }
    onDisposing() {
        this.gl.deleteSampler(this.handle);
    }
}
//# sourceMappingURL=Sampler.js.map