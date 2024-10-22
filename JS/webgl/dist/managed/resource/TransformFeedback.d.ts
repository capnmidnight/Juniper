import { ManagedWebGLResource } from "./ManagedWebGLResource";
export declare class TransformFeedback extends ManagedWebGLResource<WebGLTransformFeedback> {
    private target;
    constructor(gl: WebGL2RenderingContext, target: GLenum);
    bind(): void;
    protected onDisposing(): void;
}
//# sourceMappingURL=TransformFeedback.d.ts.map