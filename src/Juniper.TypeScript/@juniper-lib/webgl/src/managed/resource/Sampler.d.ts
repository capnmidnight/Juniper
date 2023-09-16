import { ManagedWebGLResource } from "./ManagedWebGLResource";
export declare class Sampler extends ManagedWebGLResource<WebGLSampler> {
    private unit;
    constructor(gl: WebGL2RenderingContext, unit: GLenum);
    bind(): void;
    protected onDisposing(): void;
}
//# sourceMappingURL=Sampler.d.ts.map