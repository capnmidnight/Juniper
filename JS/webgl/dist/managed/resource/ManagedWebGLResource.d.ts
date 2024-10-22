import type { IDisposable } from "@juniper-lib/tslib/dist/using";
import { ManagedWebGLObject } from "../object/ManagedWebGLObject";
export declare abstract class ManagedWebGLResource<PointerT> extends ManagedWebGLObject<PointerT> implements IDisposable {
    private disposed;
    constructor(gl: WebGL2RenderingContext, handle: PointerT);
    dispose(): void;
    protected abstract onDisposing(): void;
}
//# sourceMappingURL=ManagedWebGLResource.d.ts.map